using CCL.Types.Proxies;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Simulation;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class CabooseSimCreator : SimCreator
    {
        public CabooseSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "Caboose" };

        public override IEnumerable<string> GetSimFeatures(int basisIndex)
        {
            yield return "Dummy Fusebox";
            yield return "Dummy Traction";
        }

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRICS_MAIN", true)
            };

            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);

            _root.AddComponent<CabooseControllerProxy>();

            Object.DestroyImmediate(_baseControls);
        }
    }
}
