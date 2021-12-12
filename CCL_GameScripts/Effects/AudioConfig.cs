using CCL_GameScripts.Attributes;
using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public abstract class AudioConfig : MonoBehaviour, IProxyScript
    {
        public abstract string TargetTypeName { get; }
        public abstract bool IsOverrideSet(int index);
    }

    public class DieselAudioConfig : AudioConfig, IProxyScript
    {
        public override string TargetTypeName => "DVCustomCarLoader.LocoComponents.CustomLocoAudioDiesel";

        public override bool IsOverrideSet(int index)
        {
            // only override if field isn't null
            switch (index)
            {
                case 1:
                    return playEngineAt;
                case 2:
                    return playReverserAt;
                case 3:
                    return playSandAt;
                default:
                    return false;
            }
        }

        [ProxyField(overrideFlag: 1)]
        public Transform playEngineAt;

        [ProxyField(overrideFlag: 2)]
        public Transform playReverserAt;

        [ProxyField(overrideFlag: 3)]
        public Transform playSandAt;
    }
}