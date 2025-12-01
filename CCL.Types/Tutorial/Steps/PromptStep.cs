using UnityEngine;

namespace CCL.Types.Tutorial.Steps
{
    [AddComponentMenu("CCL/Tutorial/Steps/Prompt Step")]
    public class PromptStep : TutorialStep
    {
        public string Key = string.Empty;
        public bool Pause = false;
    }
}
