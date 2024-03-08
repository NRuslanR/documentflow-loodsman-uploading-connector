using System;
using System.Collections.Generic;
using System.Linq;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.Services;
using UMP.Loodsman.Dtos.SUPR;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman
{
    public class LoodsmanTaskDtoConverter
    {
        private const long TaskIssuedStateId = 1L;
        private const long TaskPerformedStateId = 4L;
        private readonly IUserLoginService userLoginService;

        public LoodsmanTaskDtoConverter(IUserLoginService userLoginService)
        {
            this.userLoginService = userLoginService;
        }

        public IEnumerable<NewTaskDto> ConvertDocumentChargeSheetInfoDtos(
            IEnumerable<DocumentChargeSheetInfoDTO> documentChargeSheetInfoDtos)
        {
            var chargeSheetInfoDtoList = documentChargeSheetInfoDtos.ToList();

            var newTaskHolderDictionary = new Dictionary<object, NewTaskDtoHolder>(chargeSheetInfoDtoList.Count);

            var newTaskDtos = new List<NewTaskDto>();

            foreach (var chargeSheetInfoDto in chargeSheetInfoDtoList)
            {
                var newTaskDto = ConvertDocumentChargeSheetInfoDto(chargeSheetInfoDto);

                // Case 1: Child charge sheet is taken earlier than parent,
                // create NewTaskDtoHolder for parent, 
                // add newTaskDto as unhandled sub task dto
                if (!newTaskHolderDictionary.ContainsKey(chargeSheetInfoDto.TopLevelChargeSheetId))
                {
                    newTaskHolderDictionary[chargeSheetInfoDto.TopLevelChargeSheetId] =
                        new NewTaskDtoHolder(new[] { newTaskDto });
                }

                // Case 2: NewTaskDtoHolder already exists for parent charge sheet,
                // it is mean that this holder created by another sub task dto or parent task dto as itself,
                // add newTaskDto as unhandled sub task at former case or
                // add in parent task dto's sub task dto list at last case
                else
                {
                    var newTaskHolder = newTaskHolderDictionary[chargeSheetInfoDto.TopLevelChargeSheetId];

                    if (newTaskHolder.NewTask is null)
                        newTaskHolder.UnhandledSubTasks.Add(newTaskDto);

                    else newTaskHolder.NewTask.SubTasks.Add(newTaskDto);
                }

                // Create task dto holder for continue using by next sub or parent task dtos 
                if (!newTaskHolderDictionary.ContainsKey(chargeSheetInfoDto.Id))
                {
                    newTaskHolderDictionary[chargeSheetInfoDto.Id] = new NewTaskDtoHolder(newTaskDto);
                }

                // Case 3: task dto holder already exists for current charge sheet,
                // extract all unhandled sub task dtos to newTaskDto 
                else
                {
                    var newTaskHolder = newTaskHolderDictionary[chargeSheetInfoDto.Id];

                    newTaskHolder.NewTask = newTaskDto;

                    foreach (var unhandledSubTask in newTaskHolder.UnhandledSubTasks)
                        newTaskDto.SubTasks.Add(unhandledSubTask);

                    newTaskHolder.UnhandledSubTasks.Clear();
                }

                if (chargeSheetInfoDto.Id is null)
                    newTaskDtos.Add(newTaskDto);
            }

            return newTaskDtos;
        }

        private NewTaskDto ConvertDocumentChargeSheetInfoDto(DocumentChargeSheetInfoDTO chargeSheetInfoDto)
        {
            var newTaskDto =
                new NewTaskDto
                {
                    Author = ConvertUserDto(chargeSheetInfoDto.IssuerInfoDTO),
                    Description = chargeSheetInfoDto.ChargeText,
                    PlanDateStart = chargeSheetInfoDto.TimeFrameStart ?? default(DateTime),
                    PlanDateFinish = chargeSheetInfoDto.TimeFrameDeadline ?? default(DateTime),
                    State = ConvertTaskState(chargeSheetInfoDto),
                    Worker = ConvertUserDto(chargeSheetInfoDto.PerformerInfoDTO)
                };

            return newTaskDto;
        }

        private UserDto ConvertUserDto(DocumentFlowEmployeeInfoDTO employeeInfoDto)
        {
            return new UserDto
            {
                Name = userLoginService.GetUserLoginByPersonnelNumber(employeeInfoDto.PersonnelNumber),
                FullName = employeeInfoDto.FullName
            };
        }

        private long ConvertTaskState(DocumentChargeSheetInfoDTO chargeSheetInfoDto)
        {
            return chargeSheetInfoDto.PerformingDateTime.HasValue ? TaskPerformedStateId : TaskIssuedStateId;
        }

        private class NewTaskDtoHolder
        {
            public NewTaskDtoHolder(NewTaskDto newTask) : this()
            {
                NewTask = newTask;
            }

            public NewTaskDtoHolder(IEnumerable<NewTaskDto> subTasks)
            {
                UnhandledSubTasks = new List<NewTaskDto>(subTasks);
            }

            private NewTaskDtoHolder()
            {
                UnhandledSubTasks = new List<NewTaskDto>();
            }

            public NewTaskDto NewTask { get; set; }

            public ICollection<NewTaskDto> UnhandledSubTasks { get; }
        }
    }
}