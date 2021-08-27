using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents
{
    public interface IFusedLocoController
    {
        bool EngineRunning { get; set; }

        bool CanEngineStart { get; }

        bool AutoStart { get; }
    }
}
