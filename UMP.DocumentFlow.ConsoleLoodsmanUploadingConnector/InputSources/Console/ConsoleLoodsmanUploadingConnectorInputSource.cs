using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector.InputSources.Console
{
    internal class ConsoleLoodsmanUploadingConnectorInputSource : ILoodsmanUploadingConnectorInputSource
    {
        private readonly string[] args;

        public ConsoleLoodsmanUploadingConnectorInputSource(string[] args)
        {
            this.args = args;
        }

        public LoodsmanUploadingConnectorInput GetLoodsmanUploadingConnectorInput()
        {
            var consoleInput = GetConsoleLoodsmanUploadingConnectorInput(args);

            return MapLoodsmanUploadingConnectorInput(consoleInput);
        }

        private ConsoleLoodsmanUploadingConnectorInput GetConsoleLoodsmanUploadingConnectorInput(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<ConsoleLoodsmanUploadingConnectorInput>(args);

            if (parseResult.Errors.Any())
            {
                throw new InvalidOperationException($@"Input command-line arguments aren't correct");
            }

            return parseResult.Value;
        }

        private LoodsmanUploadingConnectorInput MapLoodsmanUploadingConnectorInput(
            ConsoleLoodsmanUploadingConnectorInput consoleInput) =>
            new LoodsmanUploadingConnectorInput
            {
                DbTableDataSubscriberInput = new DbTableDataSubscriberInput
                {
                    Host = consoleInput.DocumentUploadingChangesSubscriberDbHost,
                    Port = consoleInput.DocumentUploadingChangesSubscriberPort,
                    Database = consoleInput.DocumentUploadingChangesSubscriberDatabase,
                    UserName = consoleInput.DocumentUploadingChangesSubscriberUserName,
                    Password = consoleInput.DocumentUploadingChangesSubscriberPassword,
                    SubscriptionName = consoleInput.DocumentUploadingChangesSubscriptionName,
                    SubscriptionSlotName = consoleInput.DocumentUploadingChangesSubscriptionSlotName,
                    ClientEncoding = consoleInput.DocumentUploadingChangesSubscriberEncoding
                },
                MessageBrokerInput = new MessageBrokerInput
                {
                    ConnectionInput = new ConnectionInput
                    {
                        Host = consoleInput.RabbitMQHost,
                        Port = consoleInput.RabbitMQPort,
                        UserName = consoleInput.RabbitMQUsername,
                        Password = consoleInput.RabbitMQPassword,
                    },
                    ExchangeInput = new ExchangeInput
                    {
                        Name = consoleInput.ExchangeName
                    },
                    RequestQueueInput = new QueueInput
                    {
                        Name = consoleInput.DocumentUploadingRequestQueueName,
                        RoutingKey = consoleInput.DocumentUploadingRequestRoutingKey
                    },
                    ReplyQueueInput = new QueueInput
                    {
                        Name = consoleInput.DocumentUploadingReplyQueueName,
                        RoutingKey = consoleInput.DocumentUploadingReplyRoutingKey
                    }
                }
            };
    }
}
