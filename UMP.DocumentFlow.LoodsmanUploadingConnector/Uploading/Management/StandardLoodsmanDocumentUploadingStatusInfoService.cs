using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management
{
    public class StandardLoodsmanDocumentUploadingStatusInfoService : ILoodsmanDocumentUploadingStatusInfoService
    {
        private class NextUploadingStatusInfo
        {
            public LoodsmanDocumentUploadingStatus UploadingStatus { get; set; }

            public string StatusDateTimePropName { get; set; }
        }

        private IEnumerable<LoodsmanDocumentUploadingStatus> activeStatusList =
            new List<LoodsmanDocumentUploadingStatus>
            {
                LoodsmanDocumentUploadingStatus.UploadingRequested,
                LoodsmanDocumentUploadingStatus.CancelationRequested
            };

        private readonly IDictionary<LoodsmanDocumentUploadingStatus, NextUploadingStatusInfo> nextUploadingStatusInfos;

        public StandardLoodsmanDocumentUploadingStatusInfoService()
        {
            nextUploadingStatusInfos = new Dictionary<LoodsmanDocumentUploadingStatus, NextUploadingStatusInfo>();

            InitializeNextUploadingStatusInfos(nextUploadingStatusInfos);
        }

        private void InitializeNextUploadingStatusInfos(IDictionary<LoodsmanDocumentUploadingStatus, NextUploadingStatusInfo> target)
        {
            SetNextUploadingStatusInfo(target, LoodsmanDocumentUploadingStatus.UploadingRequested,
                LoodsmanDocumentUploadingStatus.Uploading, nameof(LoodsmanDocumentUploadingRecordInfo.UploadingDateTime)
            );

            SetNextUploadingStatusInfo(target, LoodsmanDocumentUploadingStatus.Uploading,
                LoodsmanDocumentUploadingStatus.Uploaded, nameof(LoodsmanDocumentUploadingRecordInfo.UploadedDateTime)
            );

            SetNextUploadingStatusInfo(target, LoodsmanDocumentUploadingStatus.CancelationRequested,
                LoodsmanDocumentUploadingStatus.Canceling, nameof(LoodsmanDocumentUploadingRecordInfo.CancelingDateTime)
            );

            SetNextUploadingStatusInfo(target, LoodsmanDocumentUploadingStatus.Canceling,
                LoodsmanDocumentUploadingStatus.Canceled, nameof(LoodsmanDocumentUploadingRecordInfo.CanceledDateTime)
            );
        }

        private void SetNextUploadingStatusInfo(
            IDictionary<LoodsmanDocumentUploadingStatus, NextUploadingStatusInfo> target, 
            LoodsmanDocumentUploadingStatus sourceUploadingStatus, 
            LoodsmanDocumentUploadingStatus destUploadingStatus, string statusDateTimePropName)
        {
            target[sourceUploadingStatus] = new NextUploadingStatusInfo
                { UploadingStatus = destUploadingStatus, StatusDateTimePropName = statusDateTimePropName };
        }

        public LoodsmanDocumentUploadingStatus GetNextStatus(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            GetNextUploadingStatusInfo(uploadingRecordInfo)?.UploadingStatus ?? LoodsmanDocumentUploadingStatus.Unknown;

        public bool IsCurrentStatusActive(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            activeStatusList.Any(status => uploadingRecordInfo.UploadingStatus == status);

        private NextUploadingStatusInfo GetNextUploadingStatusInfo(
            LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            IsNextStatusExists(uploadingRecordInfo)
                ? nextUploadingStatusInfos[uploadingRecordInfo.UploadingStatus]
                : null;

        public bool IsNextStatusExists(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo) =>
            nextUploadingStatusInfos.ContainsKey(uploadingRecordInfo.UploadingStatus);

        public void ChangeNextStatusInfo(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo)
        {
            var nextUploadingStatus = GetNextStatus(uploadingRecordInfo);

            if (nextUploadingStatus == LoodsmanDocumentUploadingStatus.Unknown)
            {
                throw new InvalidOperationException($@"Next uploading status for status {uploadingRecordInfo.UploadingStatus} not found");
            }

            ChangeStatusInfo(uploadingRecordInfo, nextUploadingStatus);
        }

        public void ChangeStatusInfo(LoodsmanDocumentUploadingRecordInfo uploadingRecordInfo,
            LoodsmanDocumentUploadingStatus uploadingStatus)
        {
            var nextUploadingStatus = GetNextUploadingStatusInfo(uploadingRecordInfo);

            if (nextUploadingStatus is null)
            {
                throw new ArgumentException($@"Next uploading status for status {uploadingRecordInfo.UploadingStatus} not found");
            }

            if (nextUploadingStatus?.UploadingStatus != uploadingStatus)
            {
                throw new ArgumentException($@"Specified uploading status {uploadingStatus} isn't same as {nextUploadingStatus}");
            }

            uploadingRecordInfo.UploadingStatus = uploadingStatus;
            
            uploadingRecordInfo.GetType().GetProperty(nextUploadingStatus.StatusDateTimePropName)?.SetValue(uploadingRecordInfo, DateTime.Now);
        }
    }
}
