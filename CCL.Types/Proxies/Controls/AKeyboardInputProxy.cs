using CCL.Types.Json;
using System;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class AKeyboardInputProxy : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class ActionReference
        {
            [ControlActionField]
            public string name = string.Empty;
            public bool flip;

            public ActionReference() { }

            public ActionReference(string name) : this(name, false) { }

            public ActionReference(string name, bool flip)
            {
                this.name = name;
                this.flip = flip;
            }
        }

        public abstract void OnValidate();

        public abstract void AfterImport();
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Analog Set Value Joystick Input Proxy")]
    public class AnalogSetValueJoystickInputProxy : AKeyboardInputProxy
    {
        public ActionReference action = new ActionReference();
        public bool compressZeroToOne;

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(action);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref action);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Binary Decode Value Input Proxy")]
    public class BinaryDecodeValueInputProxy : AKeyboardInputProxy
    {
        public ActionReference action = new ActionReference();
        public ControlSpecProxy targetLeastSignificant = null!;
        public ControlSpecProxy targetMostSignificant = null!;

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(action);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref action);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Button Use Keyboard Input Proxy")]
    public class ButtonUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference useAction = new ActionReference();

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(useAction);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref useAction);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Button Set Value From Axis Input Proxy")]
    public class ButtonSetValueFromAxisInputProxy : AKeyboardInputProxy
    {
        public ActionReference useAction = new ActionReference();
        public bool useReturnToZero;

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(useAction);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref useAction);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Firebox Keyboard Input Proxy")]
    public class FireboxKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference lightFireAction = new ActionReference();
        public ActionReference shovelCoalAction = new ActionReference();
        
        [SerializeField, HideInInspector]
        private string? _jsonLight;
        [SerializeField, HideInInspector]
        private string? _jsonShovel;

        public override void OnValidate()
        {
            _jsonLight = JSONObject.ToJson(lightFireAction);
            _jsonShovel = JSONObject.ToJson(shovelCoalAction);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_jsonLight, ref lightFireAction);
            JSONObject.FromJson(_jsonShovel, ref shovelCoalAction);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Mouse Scroll Keyboard Input Proxy")]
    public class MouseScrollKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference scrollAction = new ActionReference();
        public bool onlyScrollOnce;

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(scrollAction);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref scrollAction);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Physics Force Input Proxy")]
    public class PhysicsForceKeyboardInputProxy : AKeyboardInputProxy
    {
        public Vector3 forceVector;
        public ActionReference applyAction = new ActionReference();

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(applyAction);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref applyAction);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Physics Torque Keyboard Input Proxy")]
    public class PhysicsTorqueKeyboardInputProxy : PhysicsForceKeyboardInputProxy { }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Toggle Switch Use Keyboard Input Proxy")]
    public class ToggleSwitchUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference useAction = new ActionReference();

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(useAction);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref useAction);
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Input/Toggle Value Keyboard Input Proxy")]
    public class ToggleValueKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference useAction = new ActionReference();

        [SerializeField, HideInInspector]
        private string? _json;

        public override void OnValidate()
        {
            _json = JSONObject.ToJson(useAction);
        }

        public override void AfterImport()
        {
            JSONObject.FromJson(_json, ref useAction);
        }
    }
}
