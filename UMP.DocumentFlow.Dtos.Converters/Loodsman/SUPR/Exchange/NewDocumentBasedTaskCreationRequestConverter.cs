using System.Collections.Generic;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange.Generics;
using UMP.Loodsman.Dtos;
using UMP.Loodsman.Dtos.SUPR;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange
{
    public class NewDocumentBasedTaskCreationRequestConverter : TaskManagingRequestConverter<NewDocumentBasedTasksCreationRequest>
    {
        private readonly LoodsmanDocumentFullInfoDtoConverter documentFullInfoDtoConverter;
        private readonly LoodsmanTaskDtoConverter taskDtoConverter;

        public NewDocumentBasedTaskCreationRequestConverter(
            LoodsmanDocumentFullInfoDtoConverter documentFullInfoDtoConverter,
            LoodsmanTaskDtoConverter taskDtoConverter)
        {
            this.documentFullInfoDtoConverter = documentFullInfoDtoConverter;
            this.taskDtoConverter = taskDtoConverter;
        }

        public override NewDocumentBasedTasksCreationRequest ConvertDocumentFullInfoDto(DocumentFullInfoDTO documentFullInfoDto)
        {
            return new NewDocumentBasedTasksCreationRequest
            {
                DocumentFullInfo = ConvertDocumentFullInfoDtoToLoodsmanDocumentDto(documentFullInfoDto),
                NewTasks = ConvertDocumentChargeSheetInfoDtosToTaskDtos(documentFullInfoDto.DocumentChargeSheetsInfoDTO)
            };
        }

        private DocumentFullInfoDto ConvertDocumentFullInfoDtoToLoodsmanDocumentDto(
            DocumentFullInfoDTO documentFullInfoDto)
        {
            return documentFullInfoDtoConverter.ConvertDocumentFullInfoDtoToLoodsmanDocumentFullInfoDto(documentFullInfoDto);
        }

        private IEnumerable<NewTaskDto> ConvertDocumentChargeSheetInfoDtosToTaskDtos(
            IEnumerable<DocumentChargeSheetInfoDTO> chargeSheetInfoDtos)
        {
            return taskDtoConverter.ConvertDocumentChargeSheetInfoDtos(chargeSheetInfoDtos);
        }
    }
}