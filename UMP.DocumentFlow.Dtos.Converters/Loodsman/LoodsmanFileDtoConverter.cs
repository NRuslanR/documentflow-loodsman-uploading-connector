using System.Collections.Generic;
using System.Linq;
using UMP.Loodsman.Dtos;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman
{
    public class LoodsmanFileDtoConverter
    {
        public FileDtos ConvertDocumentFileInfoDtos(IEnumerable<DocumentFileInfoDTO> documentFileInfoDTOs)
        {
            return new FileDtos(documentFileInfoDTOs.Select(ConvertDocumentFileInfoDto));
        }

        private FileDto ConvertDocumentFileInfoDto(DocumentFileInfoDTO documentFileInfoDto)
        {
            return new FileDto
            {
                Name = documentFileInfoDto.FileName,
                LocalName = documentFileInfoDto.FilePath
            };
        }
    }
}