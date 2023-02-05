namespace CCL_GameScripts.Effects
{
    public class ContinuousBoundAudioSetup : BoundAudioSetup
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.ContinuousBoundAudioSource";
        public override bool DestroyAfterCreation => true;

        public override void OnValidate()
        {
            if (Pitch == null) Pitch = new ConfigurableBinding();
            if (Volume == null) Volume = new ConfigurableBinding();

            var json = new JSONObject();
            json.AddField("pitch", Pitch.ToJson());
            json.AddField("volume", Volume.ToJson());
            BindingData = json.Print();
        }
    }
}
