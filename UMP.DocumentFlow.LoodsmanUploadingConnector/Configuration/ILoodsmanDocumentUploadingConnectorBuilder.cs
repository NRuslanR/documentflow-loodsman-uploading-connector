using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using DbTableDataChangesSubscriberInterfaces;
using MessagingServicing;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PostgresTableDataChangesSubscribing;
using Serilog;
using UMP.DocumentFlow.Dtos.Converters.Loodsman;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.Services;
using UMP.DocumentFlow.Dtos.Converters.Loodsman.SUPR.Exchange;
using UMP.DocumentFlow.Dtos.Mappers.DataFormats;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Data;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management.EntityFramework;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Mappers;
using UMP.Loodsman.Dtos.Names.Attributes;
using UMP.Loodsman.Dtos.Names.Attributes.Documents;
using UMP.Loodsman.Dtos.Names.Documents;
using UMP.Loodsman.Dtos.Names.States;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Configuration
{
    public interface ILoodsmanDocumentUploadingConnectorBuilder
    {
        ILoodsmanDocumentUploadingConnectorBuilder WithPgOutputTableDataChangesSubscriber(
            DbTableDataChangesSubscriberInputData subscriberInputData);

        ILoodsmanDocumentUploadingConnectorBuilder WithJsonDocumentFullInfoDTOMapper();

        ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanDocumentUploadingStatusInfoService();

        ILoodsmanDocumentUploadingConnectorBuilder WithConstantLoodsmanDocumentUploadingInfoTableFieldNames();

        ILoodsmanDocumentUploadingConnectorBuilder WithNpgsqlDbContext(DbConnectionData connectionData);

        ILoodsmanDocumentUploadingConnectorBuilder WithEFLoodsmanDocumentUploadingInfoService();

        ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanDocumentUploadingRecordInfoMapper();

        ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanDocumentUploadingInfoMapper();

        ILoodsmanDocumentUploadingConnectorBuilder WithStandardLoodsmanTaskManagingRequestConverters();

        ILoodsmanDocumentUploadingConnectorBuilder WithMessagingLoodsmanTaskManagingRequestMessagingService(
            IMessagingService messagingService);

        ILoodsmanDocumentUploadingConnectorBuilder WithMessagingLoodsmanTaskManagingReplyMessagingService(
            IMessagingService messagingService);

        ILoodsmanDocumentUploadingConnectorBuilder AddConsoleLogger();

        ILoodsmanDocumentUploadingConnectorBuilder AddFileLogger();

        ILoodsmanDocumentUploadingConnector Build();
    }
}
