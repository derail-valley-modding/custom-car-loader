using CCL_GameScripts.Attributes;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CCL_GameScripts
{
    public class DummySegmentSetup : ComponentInitSpec, ISimSetup
    {
        public override string TargetTypeName => "DummySegmentController";
        public override bool DestroyAfterCreation => true;

        [HideInInspector]
        public LocoParamsType SimType => LocoParamsType.DummySegment;

        [ProxyField("MainUnitIds")]
        [HideInInspector]
        public string[] mainUnitIds;

        public string MainUnitIds = "";

        [ProxyField]
        public CarDirection AutoCoupleSide = CarDirection.Forward;

        private void OnValidate()
        {
            mainUnitIds = MainUnitIds
                .Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();
        }
    }

    public enum CarDirection
    {
        Forward,
        Reverse
    }
}