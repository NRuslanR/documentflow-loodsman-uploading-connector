namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Data
{
    public interface ILoodsmanDocumentUploadingInfoTableFieldNames
    {
        string TableName { get; }

        string TableNamespace { get; }

        string IdFieldName { get; }

        string InitiatorIdFieldName { get; }

        string DocumentIdFieldName { get; }

        string DocumentJsonFieldName { get; }

        string UploadingStatusFieldName { get; }

        string UploadingRequestedDateTimeFieldName { get; }

        string UploadingDateTimeFieldName { get; }

        string CancelerIdFieldName { get; }

        string CancelationRequestedDateTimeFieldName { get; }

        string CancelingDateTimeFieldName { get; }

        string CanceledDateTimeFieldName { get; }

        string UploadedDateTimeFieldName { get; }

        string ErrorMessageFieldName { get; }
    }
}