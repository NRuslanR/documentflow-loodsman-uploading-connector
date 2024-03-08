using System;
using System.Collections.Generic;
using System.Linq;
using UMP.Loodsman.Dtos;
using UMP.Loodsman.Dtos.Names.Attributes.Documents;
using UMP.Loodsman.Dtos.Names.Documents;
using UMP.Loodsman.Dtos.Names.States;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman
{
    public class LoodsmanDocumentFullInfoDtoConverter
    {
        private readonly IBaseDocumentNames baseDocumentNames;
        private readonly IBaseStateNames baseStateNames;
        private readonly IBaseDocumentAttributeNames baseDocumentAttributeNames;

        private readonly LoodsmanFileDtoConverter fileDtoConverter;

        public LoodsmanDocumentFullInfoDtoConverter(
            IBaseDocumentNames baseDocumentNames,
            IBaseStateNames baseStateNames,
            IBaseDocumentAttributeNames baseDocumentAttributeNames,
            LoodsmanFileDtoConverter fileDtoConverter
        )
        {
            this.baseDocumentNames = baseDocumentNames;
            this.baseStateNames = baseStateNames;
            this.baseDocumentAttributeNames = baseDocumentAttributeNames;
            this.fileDtoConverter = fileDtoConverter ?? new LoodsmanFileDtoConverter();
        }

        public DocumentFullInfoDto ConvertDocumentFullInfoDtoToLoodsmanDocumentFullInfoDto(
            DocumentFullInfoDTO documentFullInfoDto)
        {
            var documentDto = documentFullInfoDto.DocumentDTO;

            return new DocumentFullInfoDto
            {
                Document = new DocumentDto
                {
                    TypeName = ConvertDocumentKindToLoodsmanTypeName(documentDto.KindId, documentDto.Kind),
                    ProductValue = documentDto.FullName,
                    State = ConvertCurrentWorkCycleStageNumberToState(documentDto.CurrentWorkCycleStageNumber)
                },
                AttributeValueList = ConvertDocumentInfoToAttributeValueList(documentDto),
                Files = ConvertDocumentFileInfoDtosToFileDtos(documentFullInfoDto.DocumentFilesInfoDTO)
            };
        }

        private string ConvertDocumentKindToLoodsmanTypeName(object documentKindId, string documentKind)
        {
            return Convert.ToInt64(documentKindId) == 2
                ? baseDocumentNames.ServiceNoteName
                : throw new ArgumentException($"Document kind \"{documentKind}\" is not accounted");
        }

        private string ConvertCurrentWorkCycleStageNumberToState(int documentDtoCurrentWorkCycleStageNumber)
        {
            return documentDtoCurrentWorkCycleStageNumber == 1 ? baseStateNames.DesigningStateName :
                 documentDtoCurrentWorkCycleStageNumber == 2 ? baseStateNames.ApprovingStateName :
                documentDtoCurrentWorkCycleStageNumber == 3 ? baseStateNames.ApprovedStateName :
                documentDtoCurrentWorkCycleStageNumber == 4 ? baseStateNames.ReworkingStateName :
                documentDtoCurrentWorkCycleStageNumber == 5 ? baseStateNames.DesigningStateName :
                documentDtoCurrentWorkCycleStageNumber == 6 ? baseStateNames.ReworkingStateName : baseStateNames.AffirmedStateName;
        }

        private AttributeValueDtos ConvertDocumentInfoToAttributeValueList(DocumentDTO documentDto)
        {
            return new AttributeValueDtos(
                Tuple.Create(baseDocumentAttributeNames.Author, documentDto.AuthorDTO.FullName as object),
                Tuple.Create(baseDocumentAttributeNames.AuthorDepartment,
                    documentDto.AuthorDTO.DepartmentInfoDTO.Name as object),
                Tuple.Create(baseDocumentAttributeNames.Note, documentDto.Note as object),
                Tuple.Create(
                    baseDocumentAttributeNames.ReceiverDepartment,
                    string.Join(", ",
                            documentDto.ChargesInfoDTO.Select(charge =>
                                charge.PerformerInfoDTO.DepartmentInfoDTO.Name)) as
                        object),
                Tuple.Create(baseDocumentAttributeNames.Title, documentDto.FullName as object),
                Tuple.Create("external_id", documentDto.Id));
        }

        private FileDtos ConvertDocumentFileInfoDtosToFileDtos(
            IEnumerable<DocumentFileInfoDTO> documentFileInfoDtos)
        {
            return fileDtoConverter.ConvertDocumentFileInfoDtos(documentFileInfoDtos);
        }
    }
}