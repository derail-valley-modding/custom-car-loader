using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL.Types.Proxies.Ports
{
    [Serializable]
    public class PortDefinition
    {
        public PortDefinition() { }

        // Token: 0x0600000A RID: 10 RVA: 0x00002141 File Offset: 0x00000341
        public PortDefinition(DVPortType type, DVPortValueType valueType, string iD)
        {
            this.type = type;
            this.valueType = valueType;
            this.ID = iD;
        }

        // Token: 0x04000014 RID: 20
        public DVPortType type;

        // Token: 0x04000015 RID: 21
        public DVPortValueType valueType;

        // Token: 0x04000016 RID: 22
        public string ID;
    }
}
