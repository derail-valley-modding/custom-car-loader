using System;

namespace CCL.Types.Proxies.Ports
{
    [Serializable]
    public class FuseDefinition
    {
        public string id = string.Empty;
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
