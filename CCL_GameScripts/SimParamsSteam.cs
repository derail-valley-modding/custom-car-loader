using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL_GameScripts
{
    public class SimParamsSteam : SimParamsBase
    {
        public override LocoParamsType SimType => LocoParamsType.Steam;

        public override LocoAudioBasis AudioType => LocoAudioBasis.Steam;
    }
}
