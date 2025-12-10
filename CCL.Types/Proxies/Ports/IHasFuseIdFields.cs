using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class FuseIdField : IdFieldBase
    {
        public FuseIdField(MonoBehaviour container, string fieldName, string[] fuseIds, bool required = false)
            : base(container, fieldName, fuseIds, required)
        { }

        public FuseIdField(MonoBehaviour container, string fieldName, string fuseId, bool required = false)
            : base(container, fieldName, new[] { fuseId }, required)
        { }
    }

    public interface IHasFuseIdFields
    {
        IEnumerable<FuseIdField> ExposedFuseIdFields { get; }
    }
}
