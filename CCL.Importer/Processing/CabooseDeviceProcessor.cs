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
        private static GameObject? s_careerManager;
        private static GameObject CareerManager => Extensions.GetCached(ref s_careerManager,
            () => QuickAccess.Wagons.Caboose.prefab.transform.Find(CarPartNames.Caboose.CAREER_MANAGER).gameObject);

        private static GameObject? s_trashBin;
        private static GameObject TrashBin => Extensions.GetCached(ref s_trashBin,
            () => QuickAccess.Wagons.Caboose.interiorPrefab.transform.Find(CarPartNames.Caboose.JOB_TRASH_BIN).gameObject);

        public override void ExecuteStep(ModelProcessor context)
        {
            // CareerManager
            foreach (var cmProxy in context.Car.prefab.GetComponentsInChildren<CareerManagerProxy>(true))
            {
                var newManager = Object.Instantiate(CareerManager, cmProxy.transform, false);
                newManager.transform.ResetLocal();
                Object.Destroy(cmProxy);
            }

            if (context.Car.interiorPrefab != null)
            {
                // Trash bin.
                foreach (var bin in context.Car.interiorPrefab.GetComponentsInChildren<JobTrashBinProxy>(true))
                {
                    var newBin = Object.Instantiate(TrashBin, bin.transform, false);
                    newBin.transform.ResetLocal();
                    Object.Destroy(bin);
                }
            }
        }
    }
}
