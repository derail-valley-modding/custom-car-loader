using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Sound Grabber (Source)")]
    public class SoundGrabberSource : MonoBehaviour
    {
        public AudioSource Source = null!;
        public string ReplacementName = string.Empty;

        private void Reset()
        {
            PickSame();
        }

        public void PickSame()
        {
            Source = GetComponent<AudioSource>();
        }
    }
}
