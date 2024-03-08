using CommandLine;
using System.Threading.Channels;
using System.Xml.Linq;

namespace UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector.InputSources.Console
{
    internal class ConsoleLoodsmanUploadingConnectorInput
    {
        [Option("sub_db_host", HelpText = "Database host to document uploading changes subscription", Required = true)]
        public string DocumentUploadingChangesSubscriberDbHost { get; set; }

        [Option("sub_db_port", HelpText = "Database port to document uploading changes subscription", Required = true)]
        public int DocumentUploadingChangesSubscriberPort { get; set; }

        [Option("sub_db_name", HelpText = "Database name to document uploading changes subscription", Required = true)]
        public string DocumentUploadingChangesSubscriberDatabase { get; set; }

        [Option("sub_db_user", HelpText = "Database user name to document uploading changes subscription", Required = true)]
        public string DocumentUploadingChangesSubscriberUserName { get; set; }

        [Option("sub_db_pass", HelpText = "Database user password to document uploading changes subscription", Required = true)]
        public string DocumentUploadingChangesSubscriberPassword { get; set; }

        [Option("sub_db_encoding", HelpText = "Database client encoding to document uploading changes subscription", Required = false, Default = "WIN1251")]
        public string DocumentUploadingChangesSubscriberEncoding { get; set; }

        [Option("sub_name", HelpText = "Name of the document uploading changes subscription", Required = true)]
        public string DocumentUploadingChangesSubscriptionName { get; set; }

        [Option("sub_slot_name", HelpText = "Name of document uploading changes subscription's slot", Required = true)]
        public string DocumentUploadingChangesSubscriptionSlotName { get; set; }

        [Option("rmq_host", HelpText = "RabbitMQ host name", Required = true)]
        public string RabbitMQHost { get; set; }

        [Option("rmq_port", HelpText = "RabbitMQ host port", Required = false, Default = default(int))]
        public int RabbitMQPort { get; set; }

        [Option("rmq_user", HelpText = "RabbitMQ user name", Required = true)]
        public string RabbitMQUsername { get; set; }

        [Option("rmq_pass", HelpText = "RabbitMQ user password", Required = true)]
        public string RabbitMQPassword { get; set; }

        [Option("rmq_exchange", HelpText = "RabbitMQ exchange name", Required = true)]
        public string ExchangeName { get; set; }

        [Option("rmq_req_queue", HelpText = "Document uploading request RabbitMQ queue name", Required = true)]
        public string DocumentUploadingRequestQueueName { get; set; }

        [Option("rmq_req_rkey", HelpText = "Document uploading request RabbitMQ routing key", Required = true)]
        public string DocumentUploadingRequestRoutingKey { get; set; }

        [Option("rmq_rep_queue", HelpText = "Document uploading reply RabbitMQ queue name", Required = true)]
        public string DocumentUploadingReplyQueueName { get; set; }

        [Option("rmq_rep_rkey", HelpText = "Document uploading reply RabbitMQ routing key", Required = true)]
        public string DocumentUploadingReplyRoutingKey { get; set; }
    }
}
