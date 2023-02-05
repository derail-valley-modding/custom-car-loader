using CCL_GameScripts.CabControls;
using System;
using UnityEngine;

namespace CCL_GameScripts
{
    [System.SerializableAttribute]
    public abstract class ConfigurableBindingBase
    {
        public BindingType Type;

        public float ConstantValue = 1;
        public SimEventType OutputBinding;
        public CabInputType ControlBinding;

        public abstract void Reset();
        public abstract void Apply(float value);

        public void HandleEvent(object _, float val) => Apply(val);

        public void Normalize()
        {
            if (Type != BindingType.SimOutput) OutputBinding = SimEventType.None;
            if (Type != BindingType.ControlInput) ControlBinding = CabInputType.None;
        }

        public bool ControlMatches(CabInputType inputType) => (ControlBinding != CabInputType.None) && (inputType == ControlBinding);
        public bool OutputMatches(SimEventType outputType) => (OutputBinding != SimEventType.None) && (outputType == OutputBinding);

        public virtual JSONObject ToJSON()
        {
            JSONObject json = new JSONObject();
            json.AddField("type", (int)Type);
            json.AddField("constant", ConstantValue);
            json.AddField("output", (int)OutputBinding);
            json.AddField("input", (int)ControlBinding);
            return json;
        }

        public virtual void ApplyJSON(JSONObject json)
        {
            Type = (BindingType)json["type"].i;
            ConstantValue = json["constant"].f;
            OutputBinding = (SimEventType)json["output"].i;
            ControlBinding = (CabInputType)json["input"].i;
        }

        public static T FromJSON<T>(JSONObject json)
            where T : ConfigurableBindingBase, new()
        {
            var binding = new T();
            binding.ApplyJSON(json);
            return binding;
        }
    }

    [System.SerializableAttribute]
    public class ConfigurableBinding : ConfigurableBindingBase
    {
        public float BoundMin = 0;
        public float BoundMax = 1;

        public float LocalMin = 0;
        public float LocalMax = 1;

        [NonSerialized]
        public Action<float> ApplyFunc;

        public override void Reset()
        {
            if (Type == BindingType.Constant)
            {
                ApplyFunc?.Invoke(ConstantValue);
            }
            else
            {
                ApplyFunc?.Invoke(LocalMin);
            }
        }

        public override void Apply(float value)
        {
            float normed = Mathf.InverseLerp(BoundMin, BoundMax, value);
            float mapped = Mathf.Lerp(LocalMin, LocalMax, normed);
            ApplyFunc?.Invoke(mapped);
        }

        public override string ToString()
        {
            return $"({ConstantValue}, {OutputBinding}, {ControlBinding})";
        }

        public JSONObject ToJson()
        {
            var json = base.ToJSON();
            json.AddField("min", BoundMin);
            json.AddField("max", BoundMax);
            json.AddField("localMin", LocalMin);
            json.AddField("localMax", LocalMax);
            return json;
        }

        public override void ApplyJSON(JSONObject json)
        {
            base.ApplyJSON(json);
            BoundMin = json["min"].f;
            BoundMax = json["max"].f;
            LocalMin = json["localMin"].f;
            LocalMax = json["localMax"].f;
        }
    }

    public enum BindingType
    {
        Constant,
        SimOutput,
        ControlInput,
    }
}
