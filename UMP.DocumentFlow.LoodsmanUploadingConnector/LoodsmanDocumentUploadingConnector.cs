using System.Threading;
using System.Threading.Tasks;
using DbTableDataChangesSubscriberInterfaces;
using MessagingServicing;
using Serilog;
using UMP.DocumentFlow.Dtos.Mappers.DataFormats;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector
{
    public abstract class LoodsmanDocumentUploadingConnector : ILoodsmanDocumentUploadingConnector
    {
        protected LoodsmanDocumentUploadingConnector()
        {
            LoodsmanTaskManagingRequestConverters = new LoodsmanTaskManagingRequestConverters();
        }

        public ILogger Logger { get; set; }

        public IDbTableDataChangesSubscriber DbUploadingTableSubscriber { get; set; }

        public IDataFormatDocumentFullInfoDTOMapper DocumentFullInfoDTOMapper { get; set; }

        public ILoodsmanDocumentUploadingInfoService UploadingInfoService { get; set; }

        public LoodsmanDocumentUploadingRecordInfoMapper UploadingRecordInfoMapper { get; set; }

        public LoodsmanDocumentUploadingInfoMapper UploadingInfoMapper { get; set; }

        public LoodsmanDocumentUploadingInfoStatusChangingRequestMapper UploadingInfoStatusChangingRequestMapper
        {
            get;
            set;
        }

        public LoodsmanTaskManagingRequestConverters LoodsmanTaskManagingRequestConverters { get; set; }

        public IMessagingService LoodsmanTaskManagingRequestMessagingService { get; set; }

        public IMessagingService LoodsmanTaskManagingReplyMessagingService { get; set; }

        public void Run()
        {
            StartNecessaryServices();

            InternalRun();
        }

        protected virtual void StartNecessaryServices()
        {
            StartNecessaryServicesAsync().Wait();
        }

        protected virtual void InternalRun()
        {
            InternalRunAsync().Wait();
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await StartNecessaryServicesAsync(cancellationToken);

            await InternalRunAsync(cancellationToken);
        }

        private async Task StartNecessaryServicesAsync(CancellationToken cancellationToken = default)
        { 
            await Task.Run(async () =>
            {
                await DbUploadingTableSubscriber.OpenConnectionAsync(cancellationToken);
                await LoodsmanTaskManagingRequestMessagingService.OpenConnectionAsync(cancellationToken);
                await LoodsmanTaskManagingReplyMessagingService.OpenConnectionAsync(cancellationToken);

            }, cancellationToken);
        }

        protected abstract Task InternalRunAsync(CancellationToken cancellationToken = default);
    }
}