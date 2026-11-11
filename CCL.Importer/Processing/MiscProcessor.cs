using CCL.Importer.Types;
using CCL.Types;
using CCL.Types.Components;
using DV.CabControls.VRTK;
using DV.UI;
using System.ComponentModel.Composition;
using UnityEngine;

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

            ProcessVehicleAOShadow(context.Car.prefab);

            foreach (var prefab in context.Car.AllPrefabs)
            {
                ProcessSetPhysicsMaterial(prefab);
                ProcessBedButtonProperties(prefab);
            }

            ProcessDemoStuff(context.Car);
        }

        private static PhysicMaterial? s_asphalt;
        private static PhysicMaterial? s_ballast;
        private static PhysicMaterial? s_liquid;
        private static PhysicMaterial Asphalt => Extensions.GetCached(ref s_asphalt, () => null!);
        private static PhysicMaterial Ballast => Extensions.GetCached(ref s_ballast, () =>
            QuickAccess.Locomotives.S282B.externalInteractablesPrefab.transform.Find(
                "Coal&Water/I_TenderCoal/coal1/coal1_walkable_collider/coal_full_walkable")
            .GetComponent<BoxCollider>().sharedMaterial);
        private static PhysicMaterial Liquid => Extensions.GetCached(ref s_liquid, () => null!);

        private static void ProcessSetPhysicsMaterial(GameObject prefab)
        {
            foreach (var comp in prefab.GetComponentsInChildren<SetPhysicsMaterial>(true))
            {
                foreach (var collider in comp.Colliders)
                {
                    collider.sharedMaterial = comp.Material switch
                    {
                        //SetPhysicsMaterial.PhysicsMaterial.Asphalt => Asphalt,
                        SetPhysicsMaterial.PhysicsMaterial.Ballast => Ballast,
                        //SetPhysicsMaterial.PhysicsMaterial.Liquid => Liquid,
                        _ => null!
                    };
                }
            }
        }

        private static GameObject? s_aoRectangle;
        private static GameObject AORectangle => Extensions.GetCached(ref s_aoRectangle,
            () => QuickAccess.Locomotives.DE2.prefab.transform.Find("AORectangleBlobPrefab").gameObject);

        private static void ProcessVehicleAOShadow(GameObject prefab)
        {
            var comp = prefab.GetComponentInChildren<VehicleAOShadow>();

            if (comp == null) return;

            Object.Instantiate(AORectangle, comp.transform).transform.ResetLocal();
            Object.Destroy(comp);
        }

        private static void ProcessBedButtonProperties(GameObject prefab)
        {
            var comps = prefab.GetComponentsInChildren<BedButtonProperties>();

            foreach (var comp in comps)
            {
                comp.gameObject.AddComponent<VRTK_InteractablePriorityOverride>().priority = -1;
                comp.gameObject.AddComponent<InfoAreaBed>().infoType = InteractionInfoType.Bed;
                Object.Destroy(comp);
            }
        }

        private static void ProcessDemoStuff(CCL_CarVariant livery)
        {
            if (livery.LockedGaragePrefab != null)
            {
                ModelProcessor.DoBasicProcessing(livery.LockedGaragePrefab);
            }

            if (livery.PartsModel != PartsCargoModel.Custom) return;

            ProcessPrefab(livery.PartsCargoPrefabDM1U);
            ProcessPrefab(livery.PartsCargoPrefabFlatbed);

            static void ProcessPrefab(GameObject? prefab)
            {
                if (prefab == null) return;

                ModelProcessor.DoBasicProcessing(prefab);

                if (prefab.transform.TryFind(CarPartNames.Colliders.ROOT, out var colliderRoot))
                {
                    ColliderProcessor.FixLayers(colliderRoot);
                }
            }
        }
    }
}
