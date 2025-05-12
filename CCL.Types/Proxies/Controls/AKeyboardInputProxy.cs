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

    public class ButtonUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference useAction = new ActionReference();
        public BaseKeyType useKey;

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

    public class ButtonSetValueFromAxisInputProxy : AKeyboardInputProxy
    {
        public ActionReference useAction = new ActionReference();
        public BaseKeyType useKey;

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

    public class FireboxKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference lightFireAction = new ActionReference();
        public ActionReference shovelCoalAction = new ActionReference();
        public BaseKeyType lightFireKey = BaseKeyType.LightFire;
        public BaseKeyType shovelCoalKey = BaseKeyType.Shovel;
        
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
    
    public class MouseScrollKeyboardInputProxy : AKeyboardInputProxy
    {
        public ActionReference scrollAction = new ActionReference();
        public bool onlyScrollOnce;
        public BaseKeyType scrollUpKey;
        public BaseKeyType scrollDownKey;

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
    
    public class PhysicsForceKeyboardInputProxy : AKeyboardInputProxy
    {
        public Vector3 forceVector;
        public ActionReference applyAction = new ActionReference();
        public BaseKeyType positiveApplyKey;
        public BaseKeyType negativeApplyKey;

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
    
    public class PhysicsTorqueKeyboardInputProxy : PhysicsForceKeyboardInputProxy { }

    public class ToggleSwitchUseKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType useKey;
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

    public class ToggleValueKeyboardInputProxy : AKeyboardInputProxy
    {
        public BaseKeyType toggleKey;
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
