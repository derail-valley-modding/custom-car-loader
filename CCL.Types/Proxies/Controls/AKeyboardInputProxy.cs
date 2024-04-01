using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class AKeyboardInputProxy : MonoBehaviour { }

    public class ButtonUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType useKey;
    }
    
    public class FireboxKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType lightFireKey = BaseKeyType.LightFire;
        public BaseKeyType shovelCoalKey = BaseKeyType.Shovel;
    }
    
    public class MouseScrollKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType scrollUpKey;
        public BaseKeyType scrollDownKey;
    }
    
    public class NotchedPortKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType positiveApplyKey;
        public BaseKeyType negativeApplyKey;
    }
    
    public class PhysicsForceKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType positiveApplyKey;
        public BaseKeyType negativeApplyKey;
    }
    
    public class PhysicsTorqueKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType positiveApplyKey;
        public BaseKeyType negativeApplyKey;
    }

    public class ToggleSwitchUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType useKey;
    }

    public class ToggleValueKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType toggleKey;
    }
}
