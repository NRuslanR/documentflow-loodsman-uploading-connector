using System;
using System.Collections.Generic;
using System.Text;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange.Generics
{
    public interface ITaskManagingRequestConverter<out TRequest> : ITaskManagingRequestConverter where TRequest : TaskManagingRequest
    {
        new TRequest ConvertDocumentFullInfoDto(DocumentFullInfoDTO documentFullInfoDto);
    }
}
