using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class FuseIdField : IdFieldBase
    {
        public FuseIdField(MonoBehaviour container, string fieldName, string[] fuseIds)
            : base(container, fieldName, fuseIds)
        { }

        public FuseIdField(MonoBehaviour container, string fieldName, string fuseId)
            : base(container, fieldName, new[] { fuseId })
        { }
    }

    public interface IHasFuseIdFields
    {
        IEnumerable<FuseIdField> ExposedFuseIdFields { get; }
    }
}
