namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading
{
    public enum LoodsmanDocumentUploadingStatus
    {
        NotUploaded,
        UploadingRequested,
        Uploading,
        CancelationRequested,
        Canceling,
        Canceled,
        Uploaded,
        Unknown
    }
}