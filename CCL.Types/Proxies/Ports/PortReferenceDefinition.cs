using System;

namespace CCL.Types.Proxies.Ports
{
    [Serializable]
    public class PortReferenceDefinition
    {
        public DVPortValueType valueType;

        public string ID;

        public bool writeAllowed;

        public PortReferenceDefinition(DVPortValueType valueType, string iD, bool writeAllowed = false)
        {
            this.valueType = valueType;
            this.writeAllowed = writeAllowed;
            ID = iD;
        }
    }
}
