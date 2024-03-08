using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingServicing;
using SimpleRabbitMQMessagingServicing;
using UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector.InputSources;
using UMP.DocumentFlow.LoodsmanUploadingConnector;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Configuration;

namespace UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector
{
    internal partial class LoodsmanUploadingConnectorApp
    {
        private readonly ILoodsmanUploadingConnectorInputSource connectorInputSource;
        private readonly ILoodsmanDocumentUploadingConnectorBuilder connectorBuilder;

        public LoodsmanUploadingConnectorApp(
            ILoodsmanUploadingConnectorInputSource connectorInputSource,
            ILoodsmanDocumentUploadingConnectorBuilder connectorBuilder)
        {
            this.connectorInputSource = connectorInputSource;
            this.connectorBuilder = connectorBuilder;
        }

        public void Run()
        {
            var connectorInput = connectorInputSource.GetLoodsmanUploadingConnectorInput();

            var connector = BuildConnector(connectorInput);

            connector.Run();
        }

        private ILoodsmanDocumentUploadingConnector BuildConnector(LoodsmanUploadingConnectorInput connectorInput)
        {
            var dbTableDataSubscriberInput = connectorInput.DbTableDataSubscriberInput;

            var requestMessagingService = CreateMessagingService(connectorInput.MessageBrokerInput.ConnectionInput,
                connectorInput.MessageBrokerInput.ExchangeInput, connectorInput.MessageBrokerInput.RequestQueueInput);

            var replyMessagingService = CreateMessagingService(connectorInput.MessageBrokerInput.ConnectionInput,
                connectorInput.MessageBrokerInput.ExchangeInput, connectorInput.MessageBrokerInput.ReplyQueueInput);

            var connectionData = new DbConnectionData
            {
                Host = dbTableDataSubscriberInput.Host,
                Port = dbTableDataSubscriberInput.Port,
                Database = dbTableDataSubscriberInput.Database,
                UserName = dbTableDataSubscriberInput.UserName,
                Password = dbTableDataSubscriberInput.Password,
                ClientEncoding = dbTableDataSubscriberInput.ClientEncoding,
            };

            return connectorBuilder
                    .WithConstantLoodsmanDocumentUploadingInfoTableFieldNames()
                    .WithJsonDocumentFullInfoDTOMapper()
                    .WithMessagingLoodsmanTaskManagingRequestMessagingService(requestMessagingService)
                    .WithMessagingLoodsmanTaskManagingReplyMessagingService(replyMessagingService)
                    .WithNpgsqlDbContext(connectionData)
                    .WithPgOutputTableDataChangesSubscriber(
                        new DbTableDataChangesSubscriberInputData
                        {
                            ConnectionData = connectionData,
                            SubscriptionName = dbTableDataSubscriberInput.SubscriptionName,
                            SubscriptionSlotName = dbTableDataSubscriberInput.SubscriptionSlotName
                        }
                    )
                    .WithStandardLoodsmanDocumentUploadingInfoMapper()
                    .WithStandardLoodsmanDocumentUploadingRecordInfoMapper()
                    .WithStandardLoodsmanTaskManagingRequestConverters()
                    .WithStandardLoodsmanDocumentUploadingStatusInfoService()
                    .WithEFLoodsmanDocumentUploadingInfoService()
                    .AddConsoleLogger()
                    .AddFileLogger()
                    .Build();
        }

        private IMessagingService CreateMessagingService(ConnectionInput connectionInput, ExchangeInput exchangeInput,
            QueueInput queueInput)
        {
            var connectionOptions = new SimpleRabbitMQConnectionOptions
            {
                HostName = connectionInput.Host,
                Port = connectionInput.Port,
                UserName = connectionInput.UserName,
                Password = connectionInput.Password,
            };

            var queueOptions = SimpleRabbitMQQueueOptions.CreateDefault();

            queueOptions.Name = queueInput.Name;
            queueOptions.RoutingKey = queueInput.RoutingKey;

            var exchangeOptions = SimpleRabbitMQExchangeOptions.CreateDefault();

            exchangeOptions.Name = exchangeInput.Name;

            return new SimpleRabbitMQMessagingService(connectionOptions, queueOptions, exchangeOptions);
        }
    }
}
