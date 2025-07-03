using UnityEngine;

namespace CCL.Types.Proxies.Cargo
{
    [AddComponentMenu("CCL/Proxies/Cargo/Cargo Reaction To Damage Proxy")]
    public class CargoReactionToDamageProxy : MonoBehaviour
    {
        [Header("Model - optional")]
        public GameObject regularModel = null!;
        public GameObject fullyDamagedModelPrefab = null!;

        [Header("Audio - optional")]
        public AudioClip[] idleAudio = new AudioClip[0];
        public float idleAudioPeriodMax = 120f;
        public float idleAudioPeriodMin = 5f;
        public AudioClip[] collisionAudio = new AudioClip[0];
        public AudioClip[] fullyDamagedAudio = new AudioClip[0];
    }
}
