using LocoSim.Implementations;
using System.Reflection;

namespace CCL.Importer
{
    internal static class ReflectionExtensions
    {
        public const BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static Port GetPort(this PortReference portRef)
        {
            var t = typeof(PortReference);
            var f = t.GetField("port", AllInstance);
            return (Port)f.GetValue(portRef);
        }
    }
}
