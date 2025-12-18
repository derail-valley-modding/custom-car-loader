using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Tutorial
{
    [AddComponentMenu("CCL/Tutorial/Tutorial Phase")]
    public class TutorialPhase : MonoBehaviour
    {
        public int Number = 0;
        public string Name = string.Empty;
        public List<TutorialStep> Steps = new List<TutorialStep>();

        [Header("Prerequisites")]
        public bool EngageHandbrakeControl;
        [Space]
        public bool DisengageWaterControls;
        public bool OpenDamperControl;
        public bool OpenBrakeCutout;
        public bool EngageCompressorAndDynamo;
        [Space]
        public bool IncludeStartedEngine;
    }

    public abstract class TutorialStep : MonoBehaviour { }
}
