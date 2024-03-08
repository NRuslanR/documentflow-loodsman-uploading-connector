using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector.InputSources
{
    internal interface ILoodsmanUploadingConnectorInputSource
    {
        LoodsmanUploadingConnectorInput GetLoodsmanUploadingConnectorInput();
    }
}
