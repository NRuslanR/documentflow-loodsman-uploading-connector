using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management
{
    public class SynchronizedLoodsmanDocumentUploadingInfoService : ILoodsmanDocumentUploadingInfoService
    {
        private readonly SemaphoreSlim syncLock;

        private readonly ILoodsmanDocumentUploadingInfoService loodsmanDocumentUploadingInfoService;

        public SynchronizedLoodsmanDocumentUploadingInfoService(
            ILoodsmanDocumentUploadingInfoService loodsmanDocumentUploadingInfoService)
        {
            this.loodsmanDocumentUploadingInfoService = loodsmanDocumentUploadingInfoService;

            syncLock = new SemaphoreSlim(1);
        }

        public async Task<LoodsmanDocumentUploadingRecordInfoProcessingResult>
            SetNextLoodsmanDocumentUploadingInfoStatusIfCurrentIsActiveAsync(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo,
                CancellationToken cancellationToken = default) =>
            await SynchronizeAsync(async (ct) =>
                await loodsmanDocumentUploadingInfoService.SetNextLoodsmanDocumentUploadingInfoStatusIfCurrentIsActiveAsync(
                    uploadingRecordInfo, ct), cancellationToken);

        public async Task<LoodsmanDocumentUploadingRecordInfoProcessingResult> SetLoodsmanDocumentUploadingInfoStatusAsync(
            LoodsmanDocumentUploadingInfoStatusChangingRequest statusChangingRequest,
            CancellationToken cancellationToken = default) =>
            await SynchronizeAsync(
                async (ct) =>
                    await loodsmanDocumentUploadingInfoService.SetLoodsmanDocumentUploadingInfoStatusAsync(
                        statusChangingRequest, ct), cancellationToken);

        public async Task<LoodsmanDocumentUploadingRecordInfo> GetLoodsmanUploadingRecordInfoForDocumentAsync(long documentId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            await SynchronizeAsync(
                async (ct) =>
                    await loodsmanDocumentUploadingInfoService.GetLoodsmanUploadingRecordInfoForDocumentAsync(
                        documentId, ct), cancellationToken);

        private async Task<T> SynchronizeAsync<T>(Func<CancellationToken, Task<T>> asyncCall, CancellationToken cancellationToken)
        {
            await syncLock.WaitAsync(cancellationToken);

            try
            {
                return await asyncCall(cancellationToken);
            }
            finally
            {
                syncLock.Release();
            }
        }
    }
}
