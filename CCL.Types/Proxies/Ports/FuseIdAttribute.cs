using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class FuseIdAttribute : PropertyAttribute
    {
        public bool required;

        public FuseIdAttribute(bool required = false)
        {
            this.required = required;
        }
    }
}
