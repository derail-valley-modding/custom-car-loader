using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL.Types
{
    public interface ICustomSerialized
    {
        /// <summary>
        /// Implement custom serialization of fields here (runs in editor after modifying object)
        /// </summary>
        void OnValidate();

        /// <summary>
        /// Implement custom deserialization of fields here
        /// </summary>
        void AfterImport();
    }
}
