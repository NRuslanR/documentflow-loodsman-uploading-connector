using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management
{
    public class LoodsmanDocumentUploadingInfoStatusChangingRequest
    {
        public object RequestId { get; set; }

        public long DocumentId { get; set; }

        public LoodsmanDocumentUploadingStatus UploadingStatus { get; set; }

        public string ErrorMessage { get; set; }
    }
}
