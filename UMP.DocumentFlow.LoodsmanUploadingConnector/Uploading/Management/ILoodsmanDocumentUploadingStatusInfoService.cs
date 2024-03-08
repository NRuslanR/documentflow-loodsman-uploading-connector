namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management
{
    public interface ILoodsmanDocumentUploadingStatusInfoService
    {
        LoodsmanDocumentUploadingStatus GetNextStatus(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo);

        bool IsCurrentStatusActive(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo);

        bool IsNextStatusExists(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo);

        void ChangeNextStatusInfo(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo);

        void ChangeStatusInfo(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo, LoodsmanDocumentUploadingStatus uploadingStatus);
    }   
}