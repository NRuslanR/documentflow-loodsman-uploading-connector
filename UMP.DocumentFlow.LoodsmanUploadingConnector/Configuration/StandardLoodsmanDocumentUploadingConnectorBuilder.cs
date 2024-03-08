using Autofac.Core;
using Autofac;
using DbTableDataChangesSubscriberInterfaces;
using MessagingServicing;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PostgresTableDataChangesSubscribing;
using Serilog;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.Services;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange;
using UMP.DocumentFlow.Dtos.Converters.Loodsman;
using UMP.DocumentFlow.Dtos.Mappers.DataFormats;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Data;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management.EntityFramework;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading;
using UMP.Loodsman.Dtos.Names.Attributes.Documents;
using UMP.Loodsman.Dtos.Names.Attributes;
using UMP.Loodsman.Dtos.Names.Documents;
using UMP.Loodsman.Dtos.Names.States;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Configuration
{
    public class StandardLoodsmanDocumentUploadingConnectorBuilder<TConnector> : ILoodsmanDocumentUploadingConnectorBuilder
        where TConnector : ILoodsmanDocumentUploadingConnector, new()
    {
        private ContainerBuilder containerBuilder;

        private readonly LoggerConfiguration loggerConfiguration;

        public StandardLoodsmanDocumentUploadingConnectorBuilder()
        {
            containerBuilder = new ContainerBuilder();
            loggerConfiguration = new LoggerConfiguration().MinimumLevel.Information();
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithPgOutputTableDataChangesSubscriber(
            DbTableDataChangesSubscriberInputData subscriberInputData)
        {
            var connectionString = CreatePostgresConnection(subscriberInputData.ConnectionData);

            var slotName = subscriberInputData.SubscriptionSlotName;

            containerBuilder.RegisterType<PgOutputTableDataChangesSubscriber>().As<IDbTableDataChangesSubscriber>()
                .WithParameters(
                    new Parameter[]
                    {
                        new NamedParameter(nameof(connectionString), connectionString),
                        new NamedParameter(nameof(slotName), slotName)
                    }
                ).WithProperty(
                    subscriber => subscriber.Subscriptions,
                    new DbTableDataChangeSubscriptions
                    {
                        new DbTableDataChangeSubscription(subscriberInputData.SubscriptionName)
                    }
                );

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithJsonDocumentFullInfoDTOMapper()
        {
            containerBuilder.RegisterType<JsonDocumentFullInfoDTOMapper>().As<IDataFormatDocumentFullInfoDTOMapper>();

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanDocumentUploadingStatusInfoService()
        {
            containerBuilder.RegisterType<StandardLoodsmanDocumentUploadingStatusInfoService>()
                .As<ILoodsmanDocumentUploadingStatusInfoService>();

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithConstantLoodsmanDocumentUploadingInfoTableFieldNames()
        {
            containerBuilder.RegisterType<ConstantLoodsmanDocumentUploadingInfoTableFieldNames>()
                .As<ILoodsmanDocumentUploadingInfoTableFieldNames>();

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithNpgsqlDbContext(DbConnectionData connectionData)
        {
            containerBuilder.RegisterType<DbLoodsmanDocumentUploadingInfoContext>().WithParameter(
                new TypedParameter(typeof(DbContextOptions<DbLoodsmanDocumentUploadingInfoContext>),
                    CreateNpgsqlDbContextOptions(connectionData)));

            return this;
        }

        private DbContextOptions<DbLoodsmanDocumentUploadingInfoContext> CreateNpgsqlDbContextOptions(DbConnectionData connectionData)
        {
            var connectionString = CreatePostgresConnection(connectionData);

            return new DbContextOptionsBuilder<DbLoodsmanDocumentUploadingInfoContext>().UseNpgsql(connectionString).Options;
        }

        private string CreatePostgresConnection(DbConnectionData connectionData) =>
            new NpgsqlConnectionStringBuilder
            {
                Host = connectionData.Host,
                Port = connectionData.Port,
                Database = connectionData.Database,
                Username = connectionData.UserName,
                Password = connectionData.Password,
                ClientEncoding = "WIN1251"

            }.ToString();

    public ILoodsmanDocumentUploadingConnectorBuilder WithEFLoodsmanDocumentUploadingInfoService()
        {
            WithOriginalEFLoodsmanDocumentUploadingInfoService();
            WithSynchronizedLoodsmanDocumentUploadingInfoService();

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanDocumentUploadingRecordInfoMapper()
        {
            containerBuilder.RegisterType<LoodsmanDocumentUploadingRecordInfoMapper>();

            return this;
        }

        private ILoodsmanDocumentUploadingConnectorBuilder WithOriginalEFLoodsmanDocumentUploadingInfoService()
        {
            containerBuilder.RegisterType<EFLoodsmanDocumentUploadingInfoService>()
                .Named<ILoodsmanDocumentUploadingInfoService>("original");

            return this;
        }

        private ILoodsmanDocumentUploadingConnectorBuilder WithSynchronizedLoodsmanDocumentUploadingInfoService()
        {
            containerBuilder.RegisterType<SynchronizedLoodsmanDocumentUploadingInfoService>()
                .Named<ILoodsmanDocumentUploadingInfoService>("synchronized").WithParameter(
                    (p, c) => p.ParameterType == typeof(ILoodsmanDocumentUploadingInfoService),
                    (p, c) => c.ResolveNamed<ILoodsmanDocumentUploadingInfoService>("original")
                );

            containerBuilder.Register((c, p) =>
                c.ResolveNamed<ILoodsmanDocumentUploadingInfoService>("synchronized"));

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanDocumentUploadingInfoMapper()
        {
            containerBuilder.RegisterType<LoodsmanDocumentUploadingInfoMapper>();

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanTaskManagingRequestConverters()
        {
            var converters = new LoodsmanTaskManagingRequestConverters
            {
                [LoodsmanDocumentUploadingStatus.UploadingRequested] = CreateStandardNewDocumentBasedTaskCreationRequestConverter()
            };

            containerBuilder.Register((c, p) => converters);

            return this;
        }

        /// <summary>
        /// refactor: create builder's method WithNewDocumentBasedTaskCreationRequestConverter and similar converters  rather than hard-coded implementation creating
        /// </summary>
        /// <returns></returns>
        private ITaskManagingRequestConverter CreateStandardNewDocumentBasedTaskCreationRequestConverter() =>
            new NewDocumentBasedTaskCreationRequestConverter(
                new LoodsmanDocumentFullInfoDtoConverter(
                    new ConstantBaseDocumentNames(),
                    new ConstantBaseStateNames(),
                    new StandardBaseDocumentAttributeNames(new ConstantBaseAttributeNames()),
                    new LoodsmanFileDtoConverter()), new LoodsmanTaskDtoConverter(new DomainUserLoginService()));

        public ILoodsmanDocumentUploadingConnectorBuilder WithMessagingLoodsmanTaskManagingRequestMessagingService(
            IMessagingService messagingService)
        {
            containerBuilder.RegisterInstance(messagingService).Named<IMessagingService>("request").SingleInstance();

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder WithMessagingLoodsmanTaskManagingReplyMessagingService(
            IMessagingService messagingService)
        {
            containerBuilder.RegisterInstance(messagingService).Named<IMessagingService>("reply").SingleInstance();

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder AddConsoleLogger()
        {
            loggerConfiguration.WriteTo.Console();

            containerBuilder.Register<ILogger>((c, p) => loggerConfiguration.CreateLogger());

            return this;
        }

        public ILoodsmanDocumentUploadingConnectorBuilder AddFileLogger()
        {
            loggerConfiguration.Enrich.FromLogContext().WriteTo.File(
                "loodsman_document_uploading_connector.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 10_000_000,
                retainedFileCountLimit: 7
            );

            containerBuilder.Register<ILogger>((c, p) => loggerConfiguration.CreateLogger());

            return this;
        }

        public ILoodsmanDocumentUploadingConnector Build()
        {
            var container = BuildDependencyContainer();

            var connector = container.Resolve<ILoodsmanDocumentUploadingConnector>();

            ClearContainerBuilder();

            return connector;
        }

        private IContainer BuildDependencyContainer()
        {
            containerBuilder.Register<ILoodsmanDocumentUploadingConnector>((c) =>
            {
                var connector = new TConnector();

                c.InjectProperties(connector);

                connector.LoodsmanTaskManagingRequestMessagingService = c.ResolveNamed<IMessagingService>("request");
                connector.LoodsmanTaskManagingReplyMessagingService = c.ResolveNamed<IMessagingService>("reply");

                return connector;
            });

            return containerBuilder.Build();
        }

        private void ClearContainerBuilder()
        {
            containerBuilder = new ContainerBuilder();
        }
    }
}
