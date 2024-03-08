using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector.InputSources;
using UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector.InputSources.Console;
using UMP.DocumentFlow.LoodsmanUploadingConnector;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Configuration;

namespace UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector
{
    internal partial class LoodsmanUploadingConnectorApp
    {
        internal class LoodsmanUploadingConnectorAppBuilder
        {
            private ILoodsmanUploadingConnectorInputSource loodsmanUploadingConnectorInputSource;
            private ILoodsmanDocumentUploadingConnectorBuilder loodsmanDocumentUploadingConnectorBuilder;

            public LoodsmanUploadingConnectorAppBuilder WithConsoleLoodsmanDocumentUploadingConnectorInputSource(string[] args)
            {
                loodsmanUploadingConnectorInputSource = new ConsoleLoodsmanUploadingConnectorInputSource(args);

                return this;
            }

            public LoodsmanUploadingConnectorAppBuilder WithStandardLoodsmanDocumentUploadingConnectorBuilder<
                TConnector>() where TConnector : ILoodsmanDocumentUploadingConnector, new()
            {
                loodsmanDocumentUploadingConnectorBuilder =
                    new StandardLoodsmanDocumentUploadingConnectorBuilder<TConnector>();

                return this;
            }

            public LoodsmanUploadingConnectorApp Build() =>
                new LoodsmanUploadingConnectorApp(loodsmanUploadingConnectorInputSource,
                    loodsmanDocumentUploadingConnectorBuilder);
        }

        public static readonly LoodsmanUploadingConnectorAppBuilder
            Builder = new LoodsmanUploadingConnectorAppBuilder();
    }
}
