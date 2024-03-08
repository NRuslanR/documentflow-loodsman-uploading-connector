using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading
{
    public class LoodsmanTaskManagingRequestConverters
    {
        private readonly ICollection<LoodsmanDocumentUploadingStatus> validUploadingStatuses;
        private readonly IDictionary<LoodsmanDocumentUploadingStatus, ITaskManagingRequestConverter> uploadingStatusConverterMap;

        public LoodsmanTaskManagingRequestConverters()
        {
            uploadingStatusConverterMap =
                new Dictionary<LoodsmanDocumentUploadingStatus, ITaskManagingRequestConverter>();

            validUploadingStatuses = new List<LoodsmanDocumentUploadingStatus>();       

            InitializeValidUploadingStatuses(validUploadingStatuses);
        }

        private void InitializeValidUploadingStatuses(ICollection<LoodsmanDocumentUploadingStatus> uploadingStatusCollection)
        {
            uploadingStatusCollection.Clear();

            uploadingStatusCollection.Add(LoodsmanDocumentUploadingStatus.UploadingRequested);
            uploadingStatusCollection.Add(LoodsmanDocumentUploadingStatus.CancelationRequested);
        }

        public void AddConverter(LoodsmanDocumentUploadingStatus uploadingStatus,
            ITaskManagingRequestConverter converter)
        {
            ThrowIfUploadingStatusNotValid(uploadingStatus);

            AddUploadingStatusConverterMapping(uploadingStatus, converter);
        }

        private void ThrowIfUploadingStatusNotValid(LoodsmanDocumentUploadingStatus uploadingStatus)
        {
            if (!IsUploadingStatusValid(uploadingStatus))
            {
                throw new ArgumentException(
                    $"For uploading status {uploadingStatus} isn't allowed to specify converters");
            }
        }

        private bool IsUploadingStatusValid(LoodsmanDocumentUploadingStatus uploadingStatus) => validUploadingStatuses.Contains(uploadingStatus);

        private void AddUploadingStatusConverterMapping(LoodsmanDocumentUploadingStatus uploadingStatus, ITaskManagingRequestConverter converter)
        {
            uploadingStatusConverterMap[uploadingStatus] = converter;
        }

        public ITaskManagingRequestConverter GetConverter(LoodsmanDocumentUploadingStatus uploadingStatus)
        {
            ThrowIfUploadingStatusNotValid(uploadingStatus);
            
            if (!uploadingStatusConverterMap.ContainsKey(uploadingStatus))
            {
                throw new ArgumentException($"Converter for uploading status {uploadingStatus} not found");
            }

            return uploadingStatusConverterMap[uploadingStatus];
        }

        public ITaskManagingRequestConverter this[LoodsmanDocumentUploadingStatus status]
        {
            get => GetConverter(status);
            set => AddConverter(status, value);
        }
    }
}
