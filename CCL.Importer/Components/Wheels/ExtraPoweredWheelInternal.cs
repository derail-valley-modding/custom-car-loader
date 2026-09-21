using LocoSim.Implementations.Wheels;

using static CCL.Types.Components.Wheels.ExtraPoweredWheel;

namespace CCL.Importer.Components.Wheels
{
    internal class ExtraPoweredWheelInternal : PoweredWheel
    {
        public CoupledAxle[] CoupledAxles = new CoupledAxle[0];
    }
}
