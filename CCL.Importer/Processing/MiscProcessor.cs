using CCL.Importer.Components;
using CCL.Types.Components;
using System.ComponentModel.Composition;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class MiscProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            //if (context.Car.prefab.TryGetComponent<TiltingMechanism>(out var tilting))
            //{
            //    TiltingInternal.Create(tilting);
            //}
        }
    }
}
