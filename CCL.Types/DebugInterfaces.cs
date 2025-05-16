using System.Collections.Generic;

namespace CCL.Types
{
    /// <summary>
    /// Provide optional recommended ports for debugging purposes.
    /// </summary>
    public interface IRecommendedDebugPorts
    {
        public IEnumerable<string> GetDebugPorts();
    }

    /// <summary>
    /// Provide optional recommended port references for debugging purposes.
    /// </summary>
    public interface IRecommendedDebugPortReferences
    {
        public IEnumerable<string> GetDebugPortReferences();
    }
}
