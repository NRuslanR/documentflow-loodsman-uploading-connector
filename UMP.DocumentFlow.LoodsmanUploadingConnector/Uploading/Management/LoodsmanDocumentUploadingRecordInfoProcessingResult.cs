namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management
{
    public class LoodsmanDocumentUploadingRecordInfoProcessingResult
    {
        public enum Status
        {
            NextUploadingStatusNotFound,
            FailedAsCurrentStatusNotActive,
            Processed,
            Failed
        }

        public LoodsmanDocumentUploadingRecordInfoProcessingResult()
        {

        }

        public LoodsmanDocumentUploadingRecordInfo UploadingRecordInfo { get; set; }

        public Status ResultStatus { get; set; }

        public string ResultStatusMessage { get; set; }

        public static LoodsmanDocumentUploadingRecordInfoProcessingResult AsProcessed(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            new LoodsmanDocumentUploadingRecordInfoProcessingResult
            {
                ResultStatus = Status.Processed,
                UploadingRecordInfo = uploadingRecordInfo
            };

        public static LoodsmanDocumentUploadingRecordInfoProcessingResult AsFailedAsCurrentStatusNotActive(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            new LoodsmanDocumentUploadingRecordInfoProcessingResult
            {
                ResultStatus = Status.FailedAsCurrentStatusNotActive,
                ResultStatusMessage =
                    $@"Failed to set next document (id={uploadingRecordInfo.DocumentId}) uploading status as current status isn't active",
                UploadingRecordInfo = uploadingRecordInfo
            };

        public static LoodsmanDocumentUploadingRecordInfoProcessingResult WithNextUploadingStatusNotFound(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            new LoodsmanDocumentUploadingRecordInfoProcessingResult
            {
                ResultStatus = Status.NextUploadingStatusNotFound,
                ResultStatusMessage =
                    $@"Next document (id={uploadingRecordInfo.DocumentId}) uploading status not found",
                UploadingRecordInfo = uploadingRecordInfo
            };

        public static LoodsmanDocumentUploadingRecordInfoProcessingResult AsFailed(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            new LoodsmanDocumentUploadingRecordInfoProcessingResult
            {
                ResultStatus = Status.Failed,
                ResultStatusMessage = "Failed to process the document uploading info. Unknown error",
                UploadingRecordInfo = uploadingRecordInfo
            };
    }
}