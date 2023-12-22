using CCL.Types.Proxies.Controls;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL.Importer.Proxies.Controls
{
    [Export(typeof(IProxyReplacer))]
    public class ExternalControlDefinitionReplacer : ProxyReplacer<ExternalControlDefinitionProxy, ExternalControlDefinition>
    {
    }
}
