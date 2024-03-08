using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management.EntityFramework
{
    public class EFLoodsmanDocumentUploadingInfoService : ILoodsmanDocumentUploadingInfoService
    {
        private readonly DbLoodsmanDocumentUploadingInfoContext uploadingInfoContext;
        private readonly ILoodsmanDocumentUploadingStatusInfoService uploadingStatusInfoService;

        public EFLoodsmanDocumentUploadingInfoService(
            DbLoodsmanDocumentUploadingInfoContext uploadingInfoContext,
            ILoodsmanDocumentUploadingStatusInfoService uploadingStatusInfoService)
        {
            this.uploadingInfoContext = uploadingInfoContext;
            this.uploadingStatusInfoService = uploadingStatusInfoService;
        }

        public Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>
            SetNextLoodsmanDocumentUploadingInfoStatusIfCurrentIsActiveAsync(
                LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo,
                CancellationToken cancellationToken = default) =>
            RunDocumentUploadingRecordInfoProcessorInTransactionAsync(
                GetAsyncNewStatusSettingIfCurrentActiveProcessor(uploadingRecordInfo, cancellationToken),
                cancellationToken
            );

        public Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>
            SetLoodsmanDocumentUploadingInfoStatusAsync(
                LoodsmanDocumentUploadingInfoStatusChangingRequest statusChangingRequest,
                CancellationToken cancellationToken = default) =>
            RunDocumentUploadingRecordInfoProcessorInTransactionAsync(
                GetAsyncStatusChangingByDocumentFromRequestProcessor(statusChangingRequest, cancellationToken),
                cancellationToken
            );

        private Func<Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>> GetAsyncNewStatusSettingIfCurrentActiveProcessor(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo, CancellationToken cancellationToken) =>
            async () =>
            {
                var currentUploadingRecordInfo =
                    await uploadingInfoContext.UploadingRecordInfos.FindAsync(uploadingRecordInfo.Id, cancellationToken);

                if (!uploadingStatusInfoService.IsCurrentStatusActive(uploadingRecordInfo))
                {
                    return LoodsmanDocumentUploadingRecordInfoProcessingResult.AsFailedAsCurrentStatusNotActive(uploadingRecordInfo);
                }

                if (!uploadingStatusInfoService.IsNextStatusExists(uploadingRecordInfo))
                {
                    return LoodsmanDocumentUploadingRecordInfoProcessingResult.WithNextUploadingStatusNotFound(
                        uploadingRecordInfo);
                }

                var newUploadingStatus = uploadingStatusInfoService.GetNextStatus(uploadingRecordInfo);

                uploadingStatusInfoService.ChangeNextStatusInfo(uploadingRecordInfo);

                return LoodsmanDocumentUploadingRecordInfoProcessingResult.AsProcessed(uploadingRecordInfo);
            };

        private Func<Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>>
            GetAsyncStatusChangingByDocumentFromRequestProcessor(
                LoodsmanDocumentUploadingInfoStatusChangingRequest request, CancellationToken cancellationToken) =>
            async () =>
            {
                var documentUploadingRecordInfo =
                    await GetLoodsmanUploadingRecordInfoForDocumentAsync(request.DocumentId, cancellationToken);

                if (uploadingStatusInfoService.GetNextStatus(documentUploadingRecordInfo) != request.UploadingStatus)
                {
                    return LoodsmanDocumentUploadingRecordInfoProcessingResult.WithNextUploadingStatusNotFound(documentUploadingRecordInfo);
                }

                uploadingStatusInfoService.ChangeStatusInfo(documentUploadingRecordInfo, request.UploadingStatus);

                documentUploadingRecordInfo.ErrorMessage = request.ErrorMessage;

                return LoodsmanDocumentUploadingRecordInfoProcessingResult.AsProcessed(documentUploadingRecordInfo);
            };

        private Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>
            RunDocumentUploadingRecordInfoProcessorInTransactionAsync(
                Func<Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>> processor, CancellationToken cancellationToken)
        {
            var asyncUpdateTransactionWrapper = GetAsyncUpdateTransactionWrapper();

            return asyncUpdateTransactionWrapper(processor, cancellationToken);
        }

        private Func<Func<Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>>, CancellationToken,
            Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>> GetAsyncUpdateTransactionWrapper() =>

            async (Func<Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>> uploadingRecordInfoProcessor,
                CancellationToken cancellationToken) =>
            {
                var transactionDone = false;

                LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo = null;

                while (!transactionDone)
                {
                    await using var transaction =
                        await uploadingInfoContext.Database.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        var uploadingRecordInfoProcessingResult = await uploadingRecordInfoProcessor();

                        uploadingRecordInfo = uploadingRecordInfoProcessingResult.UploadingRecordInfo;

                        uploadingInfoContext.UploadingRecordInfos.Update(uploadingRecordInfo);

                        await uploadingInfoContext.SaveChangesAsync(cancellationToken);

                        await transaction.CommitAsync(cancellationToken);

                        transactionDone = true;

                        return uploadingRecordInfoProcessingResult;
                    }
                    catch (Exception exception)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        if (!(exception is DbUpdateConcurrencyException))
                            throw;
                    }
                }

                return LoodsmanDocumentUploadingRecordInfoProcessingResult.AsFailed(uploadingRecordInfo);
            };

        public async Task<LoodsmanDocumentUploadingRecordInfo> GetLoodsmanUploadingRecordInfoForDocumentAsync(
            long documentId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await uploadingInfoContext.UploadingRecordInfos.FirstOrDefaultAsync(
                recordInfo => recordInfo.DocumentId == documentId, cancellationToken);

            return result ?? throw new ArgumentException($@"Loodsman uploading info not found for required document");
        }
    }
}