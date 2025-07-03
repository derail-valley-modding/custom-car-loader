using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Sound Grabber (Source)")]
    public class SoundGrabberSource : MonoBehaviour
    {
        public AudioSource Source = null!;
        public string ReplacementName = string.Empty;

        public void PickSame()
        {
            Source = GetComponent<AudioSource>();
        }
    }
}
