using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Data;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management;
using UMP.Loodsman.Dtos;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers
{
    public class LoodsmanDocumentUploadingRecordInfoMapper
    {
        private readonly ILoodsmanDocumentUploadingInfoTableFieldNames uploadingInfoFieldNames;
  
        public LoodsmanDocumentUploadingRecordInfoMapper(
            ILoodsmanDocumentUploadingInfoTableFieldNames uploadingInfoFieldNames) 
        {
            this.uploadingInfoFieldNames = uploadingInfoFieldNames;
        }

        public LoodsmanDocumentUploadingRecordInfo MapLoodsmanUploadingRecordInfo(DataRow dataRow)
        {
            /*
            return new LoodsmanDocumentUploadingRecordInfo
            {
                UploadingStatus =
                    MapDocumentUploadingStatus(dataRow[uploadingInfoFieldNames.UploadingStatusFieldName] as string),
                CancelationRequestedDateTime =
                    dataRow[uploadingInfoFieldNames.CancelationRequestedDateTimeFieldName] as DateTime?,
                UploadingDateTime = dataRow[uploadingInfoFieldNames.UploadingDateTimeFieldName] as DateTime?,
                CancelingDateTime = dataRow[uploadingInfoFieldNames.CancelingDateTimeFieldName] as DateTime?,
                CanceledDateTime = dataRow[uploadingInfoFieldNames.CanceledDateTimeFieldName] as DateTime?,
                CancelerId = dataRow[uploadingInfoFieldNames.CancelerIdFieldName] as long?,
                DocumentId = (long)dataRow[uploadingInfoFieldNames.DocumentIdFieldName],
                DocumentJson = dataRow[uploadingInfoFieldNames.DocumentJsonFieldName] as string,
                ErrorMessage = dataRow[uploadingInfoFieldNames.ErrorMessageFieldName] as string,
                Id = (long)dataRow[uploadingInfoFieldNames.IdFieldName],
                InitiatorId = (long)dataRow[uploadingInfoFieldNames.InitiatorIdFieldName],
                UploadedDateTime = dataRow[uploadingInfoFieldNames.UploadedDateTimeFieldName] as DateTime?,
                UploadingRequestedDateTime =
                    dataRow[uploadingInfoFieldNames.UploadingRequestedDateTimeFieldName] as DateTime?
            };
            */
            var tmp = new LoodsmanDocumentUploadingRecordInfo();

            tmp.UploadingStatus =
                MapDocumentUploadingStatus(dataRow[uploadingInfoFieldNames.UploadingStatusFieldName] as string);
            tmp.CancelationRequestedDateTime =
                dataRow[uploadingInfoFieldNames.CancelationRequestedDateTimeFieldName] as DateTime?;
            tmp.UploadingDateTime = dataRow[uploadingInfoFieldNames.UploadingDateTimeFieldName] as DateTime?;
            tmp.CancelingDateTime = dataRow[uploadingInfoFieldNames.CancelingDateTimeFieldName] as DateTime?;
            tmp.CanceledDateTime = dataRow[uploadingInfoFieldNames.CanceledDateTimeFieldName] as DateTime?;
            tmp.CancelerId = dataRow[uploadingInfoFieldNames.CancelerIdFieldName] as long?;
            tmp.DocumentJson = dataRow[uploadingInfoFieldNames.DocumentJsonFieldName] as string;
            tmp.ErrorMessage = dataRow[uploadingInfoFieldNames.ErrorMessageFieldName] as string;
            tmp.UploadedDateTime = dataRow[uploadingInfoFieldNames.UploadedDateTimeFieldName] as DateTime?;
            tmp.UploadingRequestedDateTime =
                dataRow[uploadingInfoFieldNames.UploadingRequestedDateTimeFieldName] as DateTime?;
            tmp.InitiatorId = (long)dataRow[uploadingInfoFieldNames.InitiatorIdFieldName];
            tmp.Id = (long)dataRow[uploadingInfoFieldNames.IdFieldName];
            tmp.DocumentId = (long)dataRow[uploadingInfoFieldNames.DocumentIdFieldName];

            return tmp;
        }

        private LoodsmanDocumentUploadingStatus MapDocumentUploadingStatus(string statusName)
        {
            return statusName == "not_uploaded" ? LoodsmanDocumentUploadingStatus.NotUploaded :
                statusName == "uploading_requested" ? LoodsmanDocumentUploadingStatus.UploadingRequested :
                statusName == "uploading" ? LoodsmanDocumentUploadingStatus.Uploading :
                statusName == "cancelation_requested" ? LoodsmanDocumentUploadingStatus.CancelationRequested :
                statusName == "canceling" ? LoodsmanDocumentUploadingStatus.Canceling :
                statusName == "canceled" ? LoodsmanDocumentUploadingStatus.Canceled :
                statusName == "uploaded" ? LoodsmanDocumentUploadingStatus.Uploaded :
                LoodsmanDocumentUploadingStatus.Unknown;
        }
    }
}