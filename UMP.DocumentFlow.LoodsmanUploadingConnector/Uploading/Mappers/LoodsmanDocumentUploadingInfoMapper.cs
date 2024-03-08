using System;
using UMP.DocumentFlow.Dtos.Mappers.DataFormats;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers
{
    public class LoodsmanDocumentUploadingInfoMapper
    {
        private readonly IDataFormatDocumentFullInfoDTOMapper documentFullInfoDtoMapper;

        public LoodsmanDocumentUploadingInfoMapper(IDataFormatDocumentFullInfoDTOMapper documentFullInfoDtoMapper)
        {
            this.documentFullInfoDtoMapper = documentFullInfoDtoMapper;
        }

        public LoodsmanDocumentUploadingInfo MapLoodsmanDocumentUploadingInfo(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo)
        {
            return new LoodsmanDocumentUploadingInfo
            {
                StatusInfo = MapLoodsmanDocumentUploadingStatusInfo(uploadingRecordInfo),
                DocumentFullInfo = documentFullInfoDtoMapper.MapDocumentFullInfoDTO(uploadingRecordInfo.DocumentJson)
            };
        }

        private LoodsmanDocumentUploadingStatusInfo MapLoodsmanDocumentUploadingStatusInfo(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo)
        {
            return new LoodsmanDocumentUploadingStatusInfo
            {
                Status = uploadingRecordInfo.UploadingStatus,
                StatusDateTime = MapUploadingStatusDateTime(uploadingRecordInfo),
                ErrorMessage = uploadingRecordInfo.ErrorMessage
            };
        }

        private DateTime? MapUploadingStatusDateTime(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo)
        {
            var statusDateTime =
                uploadingRecordInfo.UploadingStatus == LoodsmanDocumentUploadingStatus.CancelationRequested ? uploadingRecordInfo.CancelationRequestedDateTime : 
                    uploadingRecordInfo.UploadingStatus == LoodsmanDocumentUploadingStatus.Canceled ? uploadingRecordInfo.CanceledDateTime : 
                        uploadingRecordInfo.UploadingStatus == LoodsmanDocumentUploadingStatus.Canceling ? uploadingRecordInfo.CancelingDateTime : 
                            uploadingRecordInfo.UploadingStatus == LoodsmanDocumentUploadingStatus.Uploaded ? uploadingRecordInfo.UploadedDateTime : 
                                uploadingRecordInfo.UploadingStatus == LoodsmanDocumentUploadingStatus.Uploading ? uploadingRecordInfo.UploadingDateTime : 
                                    uploadingRecordInfo.UploadingStatus == LoodsmanDocumentUploadingStatus.UploadingRequested ? uploadingRecordInfo.UploadingRequestedDateTime : DateTime.MaxValue;

            if (statusDateTime != DateTime.MaxValue)
                return statusDateTime;

            return null;
        }
    }
}