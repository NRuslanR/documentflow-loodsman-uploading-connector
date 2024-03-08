using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Configuration
{
    public class DbConnectionData
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ClientEncoding { get; set; }
    }
}
