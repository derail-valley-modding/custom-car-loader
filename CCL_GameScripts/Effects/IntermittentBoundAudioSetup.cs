using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class IntermittentBoundAudioSetup : BoundAudioSetup
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.IntermittentBoundAudioSource";
        public override bool DestroyAfterCreation => true;

        public ThresholdBinding Activation;

        [Header("Boundaries")]
        [ProxyField]
        public AudioClip EndingClip;
        [ProxyField]
        public float EndClipVolume = 1;

        public override void OnValidate()
        {
            if (Pitch == null) Pitch = new ConfigurableBinding();
            if (Volume == null) Volume = new ConfigurableBinding();
            if (Activation == null) Activation = new ThresholdBinding();

            var json = new JSONObject();
            json.AddField("pitch", Pitch.ToJson());
            json.AddField("volume", Volume.ToJson());
            json.AddField("active", Activation.ToJson());
            BindingData = json.Print();
        }
    }
}
