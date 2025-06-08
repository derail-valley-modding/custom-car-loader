using CCL.Importer.Components;
using CCL.Types.Components;
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

            foreach (var prefab in context.Car.AllPrefabs)
            {
                ProcessSetPhysicsMaterial(prefab);
            }
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
    }
}
