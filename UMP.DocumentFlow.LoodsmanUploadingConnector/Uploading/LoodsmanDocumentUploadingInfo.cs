using UMP.DocumentFlow.Dtos;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading
{
    public class LoodsmanDocumentUploadingInfo
    {
        public LoodsmanDocumentUploadingStatusInfo StatusInfo { get; set; }

        public DocumentFullInfoDTO DocumentFullInfo { get; set; }
    }
}