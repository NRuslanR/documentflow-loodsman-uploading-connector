using System.Threading;
using System.Threading.Tasks;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management
{
    public interface ILoodsmanDocumentUploadingInfoService
    {
        Task<LoodsmanDocumentUploadingRecordInfoProcessingResult> SetNextLoodsmanDocumentUploadingInfoStatusIfCurrentIsActiveAsync(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo, CancellationToken cancellationToken = default);

        Task<LoodsmanDocumentUploadingRecordInfoProcessingResult> SetLoodsmanDocumentUploadingInfoStatusAsync(
            LoodsmanDocumentUploadingInfoStatusChangingRequest statusChangingRequest,
            CancellationToken cancellationToken = default);

        Task<LoodsmanDocumentUploadingRecordInfo> GetLoodsmanUploadingRecordInfoForDocumentAsync(long documentId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}