using System;
using System.Collections.Generic;
using System.Text;
using MessagingServicing;
using UMP.Loodsman.Dtos.SUPR.Exchange;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers
{
    public class LoodsmanTaskManagingReplyMapper
    {
        public TaskManagingReply MapTaskManagingReply(IMessage message)
        {
            return
                message.TryGetContent(out NewDocumentBasedTasksCreationReply newDocumentBasedTasksCreationReply) ? (TaskManagingReply)newDocumentBasedTasksCreationReply :
                message.TryGetContent(out DocumentBasedTasksRemovingReply documentBasedTasksRemovingReply) ? documentBasedTasksRemovingReply :
                    throw new ArgumentException($"Unexpected Loodsman task managing reply type was encountered");
        }
    }
}
