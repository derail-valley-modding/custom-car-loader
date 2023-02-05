using CCL_GameScripts.CabControls;
using System;
using UnityEngine;

namespace CCL_GameScripts
{
    [System.SerializableAttribute]
    public class ThresholdBinding : ConfigurableBindingBase
    {
        public float Threshold = 0.5f;

        [NonSerialized]
        public Action<bool> ApplyFunc;

        public override void Reset()
        {
            ApplyFunc?.Invoke(false);
        }

        public override void Apply(float value)
        {
            ApplyFunc?.Invoke(value >= Threshold);
        }

        public override string ToString()
        {
            return $"({ConstantValue}, {OutputBinding}, {ControlBinding})";
        }

        public JSONObject ToJson()
        {
            JSONObject json = base.ToJSON();
            json.AddField("threshold", Threshold);
            return json;
        }

        public override void ApplyJSON(JSONObject json)
        {
            base.ApplyJSON(json);
            Threshold = json["threshold"].f;
        }
    }
}
