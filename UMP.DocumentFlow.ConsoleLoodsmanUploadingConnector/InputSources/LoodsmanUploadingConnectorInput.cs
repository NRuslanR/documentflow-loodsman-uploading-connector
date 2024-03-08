using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector.InputSources
{
    internal class LoodsmanUploadingConnectorInput
    {
        public DbTableDataSubscriberInput DbTableDataSubscriberInput { get; set; }

        public MessageBrokerInput MessageBrokerInput { get; set; }
    }

    internal class DbTableDataSubscriberInput
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string SubscriptionName { get; set; }

        public string SubscriptionSlotName { get; set; }

        public string ClientEncoding { get; set; }
    }

    internal class MessageBrokerInput
    {
        public ConnectionInput ConnectionInput { get; set; }

        public ExchangeInput ExchangeInput { get; set; }

        public QueueInput RequestQueueInput { get; set; }

        public QueueInput ReplyQueueInput { get; set; }
    }

    internal class ConnectionInput
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }

    internal class ExchangeInput
    {
        public string Name { get; set; }
    }

    internal class QueueInput   
    {
        public string Name { get; set; }

        public string RoutingKey { get; set; }
    }
}
