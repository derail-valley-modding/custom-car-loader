using CCL.Types;
using CCL.Types.Devices;
using DV;
using DV.ThingTypes;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    [Export(typeof(IProxyReplacer))]
    public class CabooseDeviceReplacer : IProxyReplacer
    {
        private static GameObject? _cabooseInterior;
        private static GameObject CabooseInterior =>
            Extensions.GetCached(ref _cabooseInterior, () => Globals.G.Types.TrainCarType_to_v2[TrainCarType.CabooseRed].interiorPrefab);


        private static GameObject? _careerManager;
        private static GameObject CareerManager =>
            Extensions.GetCached(ref _careerManager, () => CabooseInterior.transform.Find(CarPartNames.CABOOSE_CAREER_MANAGER).gameObject);


        private static GameObject? _remoteCharger;
        private static GameObject RemoteCharger =>
            Extensions.GetCached(ref _remoteCharger, () => CabooseInterior.transform.Find(CarPartNames.CABOOSE_REMOTE_CHARGER).gameObject);


        private static GameObject? _remoteAntenna;
        private static GameObject RemoteAntenna =>
            Extensions.GetCached(ref _remoteAntenna, () => CabooseInterior.transform.Find(CarPartNames.CABOOSE_REMOTE_ANTENNA).gameObject);


        public void ReplaceProxiesUncached(GameObject prefab)
        {
            foreach (var cmProxy in prefab.GetComponentsInChildren<CareerManagerProxy>(true))
            {
                var newManager = Object.Instantiate(CareerManager, cmProxy.transform, false);
                newManager.transform.localPosition = Vector3.zero;
                Object.Destroy(cmProxy);
            }

            foreach (var chargerProxy in prefab.GetComponentsInChildren<RemoteChargerProxy>(true))
            {
                var newCharger = Object.Instantiate(RemoteCharger, chargerProxy.transform, false);
                newCharger.transform.localPosition = Vector3.zero;
                Object.Destroy(chargerProxy);
            }

            foreach (var antennaProxy in prefab.GetComponentsInChildren<RemoteExtenderAntennaProxy>(true))
            {
                var newAntenna = Object.Instantiate(RemoteAntenna, antennaProxy.transform, false);
                newAntenna.transform.localPosition = Vector3.zero;
                Object.Destroy(antennaProxy);
            }
        }

        public void CacheAndReplaceProxies(GameObject prefab)
        {
            return; //Not needed
        }

        public void MapProxies(GameObject prefab)
        {
            return; //Not needed
        }
    }
}
