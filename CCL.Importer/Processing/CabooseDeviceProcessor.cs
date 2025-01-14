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
            var interior = context.Car.interiorPrefab;
            if (!interior) return;

            // CareerManager
            foreach (var cmProxy in interior.GetComponentsInChildren<CareerManagerProxy>(true))
            {
                var newManager = Object.Instantiate(CareerManager, cmProxy.transform, false);
                newManager.transform.localPosition = Vector3.zero;
                Object.Destroy(cmProxy);
            }

            // Remote charger
            foreach (var chargerProxy in interior.GetComponentsInChildren<RemoteChargerProxy>(true))
            {
                var newCharger = Object.Instantiate(RemoteCharger, chargerProxy.transform, false);
                newCharger.transform.localPosition = Vector3.zero;
                Object.Destroy(chargerProxy);
            }

            // Remote range extender
            float rangeExtensionDistance = 0;
            foreach (var antennaProxy in interior.GetComponentsInChildren<RemoteExtenderAntennaProxy>(true))
            {
                rangeExtensionDistance = Mathf.Max(rangeExtensionDistance, antennaProxy.Range);

                if (!antennaProxy.HideAntennaModel)
                {
                    var newAntenna = Object.Instantiate(RemoteAntenna, antennaProxy.transform, false);
                    newAntenna.transform.localPosition = Vector3.zero;
                    Object.Destroy(antennaProxy);
                }
            }

            if (rangeExtensionDistance > 0)
            {
                var extender = context.Car.prefab.AddComponent<RemoteControllerSignalBooster>();
                extender.range = rangeExtensionDistance;
            }
        }

        private static GameObject? _cabooseInterior;
        private static GameObject CabooseInterior =>
            Extensions.GetCached(ref _cabooseInterior, () => QuickAccess.Wagons.Caboose.interiorPrefab);


        private static GameObject? _careerManager;
        private static GameObject CareerManager =>
            Extensions.GetCached(ref _careerManager, () => CabooseInterior.transform.Find(CarPartNames.Caboose.CAREER_MANAGER).gameObject);


        private static GameObject? _remoteCharger;
        private static GameObject RemoteCharger =>
            Extensions.GetCached(ref _remoteCharger, () => CabooseInterior.transform.Find(CarPartNames.Caboose.REMOTE_CHARGER).gameObject);


        private static GameObject? _remoteAntenna;
        private static GameObject RemoteAntenna =>
            Extensions.GetCached(ref _remoteAntenna, () => CabooseInterior.transform.Find(CarPartNames.Caboose.REMOTE_ANTENNA).gameObject);

    }
}
