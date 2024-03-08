using System;
using System.Collections.Generic;
using System.Text;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management;
using UMP.Loodsman.Dtos;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers
{
    public class LoodsmanDocumentUploadingInfoStatusChangingRequestMapper
    {
        private delegate LoodsmanDocumentUploadingInfoStatusChangingRequest MappingMethod(TaskManagingReply reply);

        private IDictionary<Type, MappingMethod> mappingMethods;

        public LoodsmanDocumentUploadingInfoStatusChangingRequestMapper()
        {
            mappingMethods = new Dictionary<Type, MappingMethod>();

            InitializeMappingMethods(mappingMethods);
        }

        private void InitializeMappingMethods(IDictionary<Type, MappingMethod> methods)
        {
            
        }

        public LoodsmanDocumentUploadingInfoStatusChangingRequest MapLoodsmanDocumentUploadingInfoStatusChangingRequest(
            TaskManagingReply reply) =>
            GetMappingMethodFor(reply)(reply);
        
        private MappingMethod GetMappingMethodFor(TaskManagingReply reply)
        {
            if (!mappingMethods.ContainsKey(reply.GetType()))
            {
                throw new ArgumentException(
                    $@"Unexpected loodsman task managing reply type to mapping the uploading info status changing request");
            }

            return mappingMethods[reply.GetType()];
        }

        private LoodsmanDocumentUploadingInfoStatusChangingRequest
            MapFromNewDocumentBasedTasksCreationReply(TaskManagingReply reply)
        {
            var documentFullInfo = ((NewDocumentBasedTasksCreationReply)reply).DocumentFullInfo;

            return CreateLoodsmanDocumentUploadingInfoStatusChangingRequest(documentFullInfo, LoodsmanDocumentUploadingStatus.Uploaded, reply);
        }

        private LoodsmanDocumentUploadingInfoStatusChangingRequest MapFromDocumentBasedTasksRemovingReply(
            TaskManagingReply reply)
        {
            var documentFullInfo = ((DocumentBasedTasksRemovingReply)reply).DocumentFullInfo;

            return CreateLoodsmanDocumentUploadingInfoStatusChangingRequest(documentFullInfo, LoodsmanDocumentUploadingStatus.Canceled, reply);
        }

        private LoodsmanDocumentUploadingInfoStatusChangingRequest
            CreateLoodsmanDocumentUploadingInfoStatusChangingRequest(
                DocumentFullInfoDto documentFullInfo, LoodsmanDocumentUploadingStatus uploadingStatus,
                TaskManagingReply reply) =>
            new LoodsmanDocumentUploadingInfoStatusChangingRequest
            {
                RequestId = reply.Id,
                DocumentId = ExtractDocumentId(documentFullInfo),
                ErrorMessage = string.Join(Environment.NewLine, reply.Errors),
                UploadingStatus = uploadingStatus
            };

        private long ExtractDocumentId(DocumentFullInfoDto documentFullInfo)
        {
            if (!documentFullInfo.AttributeValueList.Contains("external_id"))
            {
                throw new ArgumentException($@"DocumentFlow's document id not found in loodsman task managing reply");
            }

            return (long)documentFullInfo.AttributeValueList.Values["external_id"];
        }
    }
}
