using CCL.Types;
using CCL.Types.Devices;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(ProxyScriptProcessor))]
    internal class CabooseDeviceProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            // CareerManager
            foreach (var cmProxy in context.Car.prefab.GetComponentsInChildren<CareerManagerProxy>(true))
            {
                var newManager = Object.Instantiate(CareerManager, cmProxy.transform, false);
                newManager.transform.ResetLocal();
                Object.Destroy(cmProxy);
            }
        }


        private static GameObject? s_careerManager;
        private static GameObject CareerManager => Extensions.GetCached(ref s_careerManager,
                () => QuickAccess.Wagons.Caboose.prefab.transform.Find(CarPartNames.Caboose.CAREER_MANAGER).gameObject);

    }
}
