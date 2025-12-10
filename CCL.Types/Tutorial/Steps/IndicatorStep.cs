using UnityEngine;

namespace CCL.Types.Tutorial.Steps
{
    [AddComponentMenu("CCL/Tutorial/Steps/Indicator Step")]
    public class IndicatorStep : TutorialStep
    {
        public enum MonitorMode
        {
            Above,
            Until
        }

        public string TargetId = string.Empty;
        [TutorialNameField]
        public string NameKey = string.Empty;
        [TutorialDescriptionField]
        public string DescriptionKey = string.Empty;
        public string Value = string.Empty;
        public float MinValue = 1.0f;
        public float MaxValue = 1.0f;
        public MonitorMode Mode = MonitorMode.Above;
        public bool ManualDismiss = true;

        public static string ModeToKey(MonitorMode mode) => mode switch
        {
            MonitorMode.Until => "tutorial/monitor_until",
            _ => "tutorial/monitor_above",
        };
    }
}
