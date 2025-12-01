using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Tutorial
{
    [AddComponentMenu("CCL/Tutorial/Tutorial Phase")]
    public class TutorialPhase : MonoBehaviour
    {
        public List<TutorialStep> Steps = new List<TutorialStep>();
        [Space]
        public bool EngageHandbrakeControl;
        [Space]
        public bool DisengageWaterControls;
        public bool OpenDamperControl;
        public bool OpenBrakeCutout;
        public bool EngageCompressorAndDynamo;
        [Space]
        public bool IncludeStartedEngine;
    }

    public class TutorialStep : MonoBehaviour { }
}
