using System;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading
{
    public class LoodsmanDocumentUploadingRecordInfo
    {
        public long Id { get; set; }

        public long InitiatorId { get; set; }

        public long DocumentId { get; set; }

        public string DocumentJson { get; set; }

        public LoodsmanDocumentUploadingStatus UploadingStatus { get; set; }

        public DateTime? UploadingRequestedDateTime { get; set; }

        public DateTime? UploadingDateTime { get; set; }

        public long? CancelerId { get; set; }

        public DateTime? CancelationRequestedDateTime { get; set; }

        public DateTime? CancelingDateTime { get; set; }

        public DateTime? CanceledDateTime { get; set; }

        public DateTime? UploadedDateTime { get; set; }

        public string? ErrorMessage { get; set; }
    }
}