using UnityEngine;

using static CCL.Types.Tutorial.TutorialSetup;

namespace CCL.Types.Tutorial.Steps
{

    [AddComponentMenu("CCL/Tutorial/Steps/Control Step")]
    public class ControlStep : TutorialStep
    {
        public string TargetId = string.Empty;
        public QTSemantic Semantic = QTSemantic.Engage;
        public float TargetValueMin = 1.0f;
        public float TargetValueMax = 1.0f;
        public string NameKey = string.Empty;
        public string DescriptionKey = string.Empty;
        public bool ShouldRecheck = true;
    }
}
