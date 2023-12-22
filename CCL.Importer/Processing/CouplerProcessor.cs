using CCL.Types;
using System;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class CouplerProcessor : ModelProcessorStep
    {
        public override Type[] RequiredSteps => new[] { typeof(BufferProcessor) };

        public override void ExecuteStep(ModelProcessor context)
        {
            var buffers = context.GetCompletedStep<BufferProcessor>();
            var basePrefab = context.BaseLivery.prefab;

            GameObject copiedObject;
            GameObject frontCoupler = basePrefab.transform.Find(CarPartNames.COUPLER_FRONT).gameObject;
            copiedObject = UnityEngine.Object.Instantiate(frontCoupler, context.Car.prefab.transform);
            copiedObject.name = CarPartNames.COUPLER_FRONT;
            var frontCouplerPosition = buffers.FrontRigPosition + CarPartOffset.COUPLER_FRONT;
            copiedObject.transform.localPosition = frontCouplerPosition;

            GameObject rearCoupler = basePrefab.transform.Find(CarPartNames.COUPLER_REAR).gameObject;
            copiedObject = UnityEngine.Object.Instantiate(rearCoupler, context.Car.prefab.transform);
            copiedObject.name = CarPartNames.COUPLER_REAR;
            var rearCouplerPosition = buffers.RearRigPosition + CarPartOffset.COUPLER_REAR;
            copiedObject.transform.localPosition = rearCouplerPosition;
        }
    }
}
