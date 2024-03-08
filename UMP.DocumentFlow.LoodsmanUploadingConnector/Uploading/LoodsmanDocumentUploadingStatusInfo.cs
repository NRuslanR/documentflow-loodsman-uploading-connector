using System;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading
{
    public class LoodsmanDocumentUploadingStatusInfo
    {
        public LoodsmanDocumentUploadingStatus Status { get; set; }

        public DateTime? StatusDateTime { get; set; }

        public string ErrorMessage { get; set; }
    }
}