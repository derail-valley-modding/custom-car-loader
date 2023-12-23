using CCL.Types;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class InfoPlateProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            var carRoot = context.Car.prefab.transform;

            // transforms should be found when the traincar script initializes them,
            // but chuck out the placeholder plates
            foreach (string plateName in CarPartNames.INFO_PLATES)
            {
                Transform plateRoot = carRoot.Find(plateName);
                if (plateRoot)
                {
                    foreach (Transform child in plateRoot)
                    {
                        Object.Destroy(child.gameObject);
                    }
                }
            }
        }
    }
}
