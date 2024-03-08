using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Configuration
{
    public class DbTableDataChangesSubscriberInputData
    {
        public DbConnectionData ConnectionData { get; set; }

        public string SubscriptionName { get; set; }

        public string SubscriptionSlotName { get; set; }
    }
}
