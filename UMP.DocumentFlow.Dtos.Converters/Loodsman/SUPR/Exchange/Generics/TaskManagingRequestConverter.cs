using System;
using System.Collections.Generic;
using System.Text;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange.Generics
{
    public abstract class TaskManagingRequestConverter<TRequest> : ITaskManagingRequestConverter<TRequest> where TRequest : TaskManagingRequest
    {
        TaskManagingRequest ITaskManagingRequestConverter.ConvertDocumentFullInfoDto(DocumentFullInfoDTO documentFullInfoDto)
        {
            return ConvertDocumentFullInfoDto(documentFullInfoDto);
        }

        public abstract TRequest ConvertDocumentFullInfoDto(DocumentFullInfoDTO documentFullInfoDto);
    }
}
