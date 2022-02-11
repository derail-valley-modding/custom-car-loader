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

    public class DieselAudioConfig : AudioConfig
    {
        public override string TargetTypeName => "DVCustomCarLoader.LocoComponents.DieselElectric.CustomLocoAudioDiesel";

        public override bool IsOverrideSet(int index) => false;

        [ProxyFieldIfSet]
        public Transform playEngineAt;

        [ProxyFieldIfSet]
        public Transform playReverserAt;

        [ProxyFieldIfSet]
        public Transform playSandAt;
    }

    public class SteamAudioConfig : AudioConfig
    {
        public override string TargetTypeName => "DVCustomCarLoader.LocoComponents.Steam.CustomLocoAudioSteam";

        public override bool IsOverrideSet(int index) => false;

        [ProxyFieldIfSet]
        public Transform playLeftCylAt;

        [ProxyFieldIfSet]
        public Transform playRightCylAt;

        [ProxyFieldIfSet]
        public Transform playChimneyAt;
    }
}