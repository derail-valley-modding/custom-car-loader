using CCL.Types;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(BufferProcessor))]
    internal class CouplerProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            var buffers = context.GetCompletedStep<BufferProcessor>();
            var basePrefab = context.Car.UseCustomBuffers ?
                QuickAccess.Wagons.Flatbed.prefab :
                context.Car.BufferType.ToTypePrefab();

            GameObject copiedObject;
            GameObject frontCoupler = basePrefab.transform.Find(CarPartNames.Couplers.COUPLER_FRONT).gameObject;
            copiedObject = Object.Instantiate(frontCoupler, context.Car.prefab.transform);
            copiedObject.name = CarPartNames.Couplers.COUPLER_FRONT;
            var frontCouplerPosition = buffers.FrontRigPosition + CarPartOffset.COUPLER_FRONT;
            copiedObject.transform.localPosition = frontCouplerPosition;

            GameObject rearCoupler = basePrefab.transform.Find(CarPartNames.Couplers.COUPLER_REAR).gameObject;
            copiedObject = Object.Instantiate(rearCoupler, context.Car.prefab.transform);
            copiedObject.name = CarPartNames.Couplers.COUPLER_REAR;
            var rearCouplerPosition = buffers.RearRigPosition + CarPartOffset.COUPLER_REAR;
            copiedObject.transform.localPosition = rearCouplerPosition;
        }
    }
}
