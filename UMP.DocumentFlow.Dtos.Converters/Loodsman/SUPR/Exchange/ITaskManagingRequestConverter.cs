using System;
using System.Collections.Generic;
using System.Text;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange
{
    public interface ITaskManagingRequestConverter
    {
        TaskManagingRequest ConvertDocumentFullInfoDto(DocumentFullInfoDTO documentFullInfoDto);
    }
}
