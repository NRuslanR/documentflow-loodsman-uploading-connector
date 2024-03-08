using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Data
{
    public class ConstantLoodsmanDocumentUploadingInfoTableFieldNames : ILoodsmanDocumentUploadingInfoTableFieldNames
    {
        public string TableName { get; } = "service_notes_uploading_queue";

        public string TableNamespace { get; } = "loodsman_integration";

        public string IdFieldName { get; } = "id";

        public string InitiatorIdFieldName { get; } = "initiator_id";

        public string DocumentIdFieldName { get; } = "document_id";

        public string DocumentJsonFieldName { get; } = "document_json";

        public string UploadingStatusFieldName { get; } = "status";

        public string UploadingRequestedDateTimeFieldName { get; } = "uploading_requested_timestamp";

        public string UploadingDateTimeFieldName { get; } = "uploading_requested_timestamp";

        public string CancelerIdFieldName { get; } = "canceler_id";

        public string CancelationRequestedDateTimeFieldName { get; } = "cancelation_requested_timestamp";

        public string CancelingDateTimeFieldName { get; } = "canceling_timestamp";

        public string CanceledDateTimeFieldName { get; } = "canceled_timestamp";

        public string UploadedDateTimeFieldName { get; } = "uploaded_timestamp";

        public string ErrorMessageFieldName { get; } = "error_message";
    }
}
