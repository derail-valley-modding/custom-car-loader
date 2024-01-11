using UnityEngine;

namespace CCL.Types.Components
{
    [RequireComponent(typeof(AudioSource))]
    internal class SoundTest : MonoBehaviour
    {
        private AudioSource _source;

        public AudioClip TestClip;

        private void Start ()
        {
            _source = GetComponent<AudioSource>();
            _source.clip = TestClip;
            _source.Play();
        }
    }
}
