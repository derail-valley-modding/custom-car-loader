using System;

namespace CCL.Types.Proxies.Ports
{
    [Serializable]
    public class PortDefinition
    {
        public PortDefinition() { }

        public PortDefinition(DVPortType type, DVPortValueType valueType, string id)
        {
            this.type = type;
            this.valueType = valueType;
            this.ID = id;
        }

        public DVPortType type;
        public DVPortValueType valueType;
        public string ID = string.Empty;
    }
}
