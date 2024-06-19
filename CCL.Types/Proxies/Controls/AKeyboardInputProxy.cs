using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class AKeyboardInputProxy : MonoBehaviour { }

    public class ButtonUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType useKey = BaseKeyType.Empty;
    }
    
    public class FireboxKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType lightFireKey = BaseKeyType.LightFire;
        public BaseKeyType shovelCoalKey = BaseKeyType.Shovel;
    }
    
    public class MouseScrollKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType scrollUpKey = BaseKeyType.Empty;
        public BaseKeyType scrollDownKey = BaseKeyType.Empty;
    }
    
    public class NotchedPortKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType positiveApplyKey = BaseKeyType.Empty;
        public BaseKeyType negativeApplyKey = BaseKeyType.Empty;
    }
    
    public class PhysicsForceKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType positiveApplyKey = BaseKeyType.Empty;
        public BaseKeyType negativeApplyKey = BaseKeyType.Empty;
    }
    
    public class PhysicsTorqueKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType positiveApplyKey = BaseKeyType.Empty;
        public BaseKeyType negativeApplyKey = BaseKeyType.Empty;
    }

    public class ToggleSwitchUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType useKey = BaseKeyType.Empty;
    }

    public class ToggleValueKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType toggleKey = BaseKeyType.Empty;
    }
}
