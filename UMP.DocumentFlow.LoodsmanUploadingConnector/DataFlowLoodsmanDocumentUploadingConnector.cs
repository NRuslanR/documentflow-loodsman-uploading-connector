using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DbTableDataChangesSubscriberInterfaces;
using MessagingServicing;
using UMP.DocumentFlow.Dtos.Mappers.DataFormats;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector
{
    public class DataFlowLoodsmanDocumentUploadingConnector : LoodsmanDocumentUploadingConnector 
    {
        protected override async Task InternalRunAsync(CancellationToken cancellationToken = default)
        {
            var startOutcomingPipelineBlock = CreateOutcomingAsyncLoodsmanDocumentUploadingPipeline(cancellationToken);
            var startIncomingPipelineBlock = CreateIncomingAsyncLoodsmanDocumentUploadingPipeline(cancellationToken);

            var outcomingAsyncLoodsmanDocumentUploadingTask = RunOutcomingAsyncLoodsmanDocumentUploadingPipeline(startOutcomingPipelineBlock, cancellationToken);
            var incomingAsyncLoodsmanDocumentUploadingTask = RunIncomingAsyncLoodsmanDocumentUploadingPipeline(startIncomingPipelineBlock, cancellationToken);

            await outcomingAsyncLoodsmanDocumentUploadingTask;
            await incomingAsyncLoodsmanDocumentUploadingTask;
        }

        private ITargetBlock<DataRow> CreateOutcomingAsyncLoodsmanDocumentUploadingPipeline(CancellationToken cancellationToken)
        {
            var documentUploadingRecordInfoMappingBlock =
                CreateDocumentUploadingRecordInfoMappingBlock(cancellationToken);

            var documentUploadingRecordInfoProcessingBlock =
                CreateDocumentUploadingRecordInfoProcessingBlock(cancellationToken);

            var documentUploadingInfoMappingBlock = CreateDocumentUploadingInfoMappingBlock(cancellationToken);

            var loodsmanTaskManagingRequestConvertingBlock =
                CreateLoodsmanTaskManagingRequestConvertingBlock(cancellationToken);

            var loodsmanTaskManagingRequestMessagingBlock =
                CreateLoodsmanTaskManagingRequestMessagingBlock(cancellationToken);

            documentUploadingRecordInfoMappingBlock.LinkTo(documentUploadingRecordInfoProcessingBlock, uploadingRecordInfo => uploadingRecordInfo != null);

            documentUploadingRecordInfoProcessingBlock.LinkTo(documentUploadingInfoMappingBlock,
                uploadingRecordInfoProcessingResult =>
                    uploadingRecordInfoProcessingResult?.ResultStatus ==
                    LoodsmanDocumentUploadingRecordInfoProcessingResult.Status.Processed);

            documentUploadingInfoMappingBlock.LinkTo(loodsmanTaskManagingRequestConvertingBlock, uploadingInfo => uploadingInfo != null);
            loodsmanTaskManagingRequestConvertingBlock.LinkTo(loodsmanTaskManagingRequestMessagingBlock, taskManagingRequest => taskManagingRequest != null);

            return documentUploadingRecordInfoMappingBlock;
        }

        private TransformBlock<DataRow, LoodsmanDocumentUploadingRecordInfo>
            CreateDocumentUploadingRecordInfoMappingBlock(CancellationToken cancellationToken)
        {
            return new TransformBlock<DataRow, LoodsmanDocumentUploadingRecordInfo>(dataRow =>
                {
                    try
                    {
                        return UploadingRecordInfoMapper.MapLoodsmanUploadingRecordInfo(dataRow);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $@"Error occurred in uploading record info mapping block");

                        return null;
                    }
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken }
            );
        }

        private TransformBlock<LoodsmanDocumentUploadingRecordInfo, LoodsmanDocumentUploadingRecordInfoProcessingResult>
            CreateDocumentUploadingRecordInfoProcessingBlock(CancellationToken cancellationToken)
        {
            return new TransformBlock<LoodsmanDocumentUploadingRecordInfo,
                LoodsmanDocumentUploadingRecordInfoProcessingResult>(
                async uploadingRecordInfo =>
                {
                    try
                    {
                        var processingResult =
                            await UploadingInfoService
                                .SetNextLoodsmanDocumentUploadingInfoStatusIfCurrentIsActiveAsync(
                                    uploadingRecordInfo, cancellationToken
                                );

                        if (processingResult.ResultStatus !=
                            LoodsmanDocumentUploadingRecordInfoProcessingResult.Status.Processed)
                        {
                            Logger.Warning(processingResult.ResultStatusMessage);
                        }

                        return processingResult;
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $@"Error occurred in uploading record info processing block for document (id={uploadingRecordInfo.DocumentId})");

                        return null;
                    }
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken }
            );
        }

        private TransformBlock<LoodsmanDocumentUploadingRecordInfoProcessingResult, LoodsmanDocumentUploadingInfo>
            CreateDocumentUploadingInfoMappingBlock(CancellationToken cancellationToken)
        {
            return new TransformBlock<LoodsmanDocumentUploadingRecordInfoProcessingResult,
                LoodsmanDocumentUploadingInfo>(
                processingResult =>
                {
                    try
                    {
                        return UploadingInfoMapper.MapLoodsmanDocumentUploadingInfo(
                            processingResult.UploadingRecordInfo);

                    }
                    catch (Exception exception)
                    {
                        Logger.Error($@"Error occurred in uploading info mapping block for document (id={processingResult.UploadingRecordInfo.DocumentId})");

                        return null;
                    }
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken }
            );
        }

        private TransformBlock<LoodsmanDocumentUploadingInfo, TaskManagingRequest>
            CreateLoodsmanTaskManagingRequestConvertingBlock(CancellationToken cancellationToken)
        {
            return new TransformBlock<LoodsmanDocumentUploadingInfo, TaskManagingRequest>(
                uploadingInfo =>
                {
                    try
                    {
                        var converter = LoodsmanTaskManagingRequestConverters[uploadingInfo.StatusInfo.Status];

                        return converter.ConvertDocumentFullInfoDto(uploadingInfo.DocumentFullInfo);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $@"Error occurred in task managing request converting block for document (id={uploadingInfo.DocumentFullInfo.DocumentDTO.FullName})");

                        return null;
                    }
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken }
            );
        }

        private ActionBlock<TaskManagingRequest> CreateLoodsmanTaskManagingRequestMessagingBlock(
            CancellationToken cancellationToken)
        {
            return new ActionBlock<TaskManagingRequest>(
                async request =>
                {
                    try
                    {
                        await LoodsmanTaskManagingRequestMessagingService.SendAsync(request, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $@"Error occurred in task managing request messaging block");
                    }
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken }
            );
        }

        private async Task RunOutcomingAsyncLoodsmanDocumentUploadingPipeline(ITargetBlock<DataRow> startPipelineBlock,
            CancellationToken cancellationToken)
        {
            Logger.Information("Start listen the request channel");

            try
            {
                await foreach (var tableDataChange in DbUploadingTableSubscriber.StartSubscribe(cancellationToken))
                {
                    var dataRow =
                        tableDataChange is DbTableDataInsert dataInsert ? dataInsert.Row :
                        tableDataChange is DbTableDataUpdate dataUpdate ? dataUpdate.UpdatedRow : null;

                    if (dataRow != null)
                        await startPipelineBlock.SendAsync(dataRow, cancellationToken);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private ITargetBlock<IMessage> CreateIncomingAsyncLoodsmanDocumentUploadingPipeline(CancellationToken cancellationToken)
        {
            var loodsmanTaskManagingReplyMappingBlock = CreateLoodsmanTaskManagingReplyMappingBlock(cancellationToken);
            var loodsmanDocumentUploadingInfoStatusChangingRequestMappingBlock =
                CreateLoodsmanDocumentUploadingInfoStatusChangingRequestMappingBlock(cancellationToken);
            var loodsmanDocumentUploadingInfoStatusChangingBlock =
                CreateLoodsmanDocumentUploadingInfoStatusChangingBlock(cancellationToken);

            loodsmanTaskManagingReplyMappingBlock.LinkTo(
                loodsmanDocumentUploadingInfoStatusChangingRequestMappingBlock);

            loodsmanDocumentUploadingInfoStatusChangingRequestMappingBlock.LinkTo(
                loodsmanDocumentUploadingInfoStatusChangingBlock);

            return loodsmanTaskManagingReplyMappingBlock;
        }

        private TransformBlock<IMessage, TaskManagingReply> CreateLoodsmanTaskManagingReplyMappingBlock(CancellationToken cancellationToken)
        {
            return new TransformBlock<IMessage, TaskManagingReply>(msg =>
                {
                    try
                    {
                        return new LoodsmanTaskManagingReplyMapper().MapTaskManagingReply(msg);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $@"Error occurred in the task managing reply mapping block");
                    }

                    return null;
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken }
            );
        }

        private TransformBlock<TaskManagingReply, LoodsmanDocumentUploadingInfoStatusChangingRequest> 
            CreateLoodsmanDocumentUploadingInfoStatusChangingRequestMappingBlock(CancellationToken cancellationToken)
        {
            return new TransformBlock<TaskManagingReply, LoodsmanDocumentUploadingInfoStatusChangingRequest>(reply =>
                {
                    try
                    {
                        return UploadingInfoStatusChangingRequestMapper
                            .MapLoodsmanDocumentUploadingInfoStatusChangingRequest(reply);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception,
                            $@"Error occurred in the uploading info status changing request mapping block");
                    }

                    return null;
                },
                new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken }
            );
        }

        private ActionBlock<LoodsmanDocumentUploadingInfoStatusChangingRequest> CreateLoodsmanDocumentUploadingInfoStatusChangingBlock(CancellationToken cancellationToken)
        {
            return new ActionBlock<LoodsmanDocumentUploadingInfoStatusChangingRequest>(async request =>
            {
                try
                {
                    var result = await UploadingInfoService.SetLoodsmanDocumentUploadingInfoStatusAsync(request, cancellationToken);

                    if (result.ResultStatus != LoodsmanDocumentUploadingRecordInfoProcessingResult.Status.Processed)
                    {
                        throw new InvalidOperationException(
                            $@"Failed to change document uploading info status");
                    }

                    await LoodsmanTaskManagingReplyMessagingService.AcknowledgeMessageAsync(request.RequestId, cancellationToken);
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, $@"Error occurred in the uploading info status changing block");
                }
            });
        }

        private async Task RunIncomingAsyncLoodsmanDocumentUploadingPipeline(ITargetBlock<IMessage> startIncomingPipelineBlock, CancellationToken cancellationToken)
        {
            Logger.Information("Start listen the reply channel");

            await foreach (var taskManagingReplyMessage in LoodsmanTaskManagingReplyMessagingService
                               .StartMessageStreamReceiving(AcknowledgeOptions.ManualAcknowledge, cancellationToken))
            {
                await startIncomingPipelineBlock.SendAsync(taskManagingReplyMessage, cancellationToken);
            }
        }
    }
}