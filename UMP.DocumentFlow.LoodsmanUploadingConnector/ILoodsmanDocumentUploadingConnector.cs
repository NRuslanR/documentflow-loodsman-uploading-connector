using System.Threading;
using System.Threading.Tasks;
using DbTableDataChangesSubscriberInterfaces;
using MessagingServicing;
using Serilog;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange;
using UMP.DocumentFlow.Dtos.Mappers.DataFormats;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector
{
    public interface ILoodsmanDocumentUploadingConnector
    {
        ILogger Logger { get; set; }

        IDbTableDataChangesSubscriber DbUploadingTableSubscriber { get; set; }

        IDataFormatDocumentFullInfoDTOMapper DocumentFullInfoDTOMapper { get; set; }

        ILoodsmanDocumentUploadingInfoService UploadingInfoService { get; set; }

        LoodsmanDocumentUploadingRecordInfoMapper UploadingRecordInfoMapper { get; set; }

        LoodsmanDocumentUploadingInfoMapper UploadingInfoMapper { get; set; }

        LoodsmanTaskManagingRequestConverters LoodsmanTaskManagingRequestConverters { get; set; }

        IMessagingService LoodsmanTaskManagingRequestMessagingService { get; set; }

        IMessagingService LoodsmanTaskManagingReplyMessagingService { get; set; }

        void Run();

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}