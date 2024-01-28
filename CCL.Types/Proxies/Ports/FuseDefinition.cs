using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL.Types.Proxies.Ports
{
    [Serializable]
    public class FuseDefinition
    {
        public string id;

        public bool initialState;

        public float offValue;

        public FuseDefinition() { }

        public FuseDefinition(string id, bool initialState, float offValue = 0f)
        {
            this.id = id;
            this.initialState = initialState;
            this.offValue = offValue;
        }
    }
}
