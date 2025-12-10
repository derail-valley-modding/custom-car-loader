using UnityEngine;

namespace CCL.Types.Tutorial.Steps
{
    [AddComponentMenu("CCL/Tutorial/Steps/Look And Acknowledge Step")]
    public class LookAndAcknowledgeStep : TutorialStep
    {
        public string TargetId = string.Empty;
        [TutorialNameField]
        public string NameKey = string.Empty;
        [TutorialDescriptionField]
        public string DescriptionKey = string.Empty;
    }
}
