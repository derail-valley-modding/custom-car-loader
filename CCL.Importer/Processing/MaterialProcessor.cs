using CCL.Types.Components.Materials;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class MaterialProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var item in context.Car.AllPrefabs)
            {
                ProcessAll(item);
            }
        }

        public static void ProcessAll(GameObject prefab)
        {
            foreach (var item in prefab.GetComponentsInChildren<IGeneratedMaterial>())
            {
                ProceduralMaterialGenerator.GenerateMaterial(item);
                item.Assign();
            }
        }
    }
}
