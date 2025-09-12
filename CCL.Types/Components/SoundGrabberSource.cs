using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Sound Grabber (Source)")]
    public class SoundGrabberSource : MonoBehaviour, ICustomGrabberValidation
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

        public bool IsValid(out string error)
        {
            if (Source == null)
            {
                error = $"SoundGrabberSource in {gameObject.GetPath()} does not have an audio source.";
                return false;
            }

            if (!SoundGrabber.SoundNames.Contains(ReplacementName))
            {
                error = $"SoundGrabberSource in {gameObject.GetPath()} does not have a valid replacement ({ReplacementName}).";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
