using CCL_GameScripts;
using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.Effects
{
    public class IntermittentBoundAudioSource : BoundAudioSource
    {
        protected ThresholdBinding activeBinding;
        
        public AudioClip EndingClip;
        public float EndClipVolume = 1;

        protected override void CreateBindings(JSONObject bindings)
        {
            base.CreateBindings(bindings);

            activeBinding = ConfigurableBindingBase.FromJSON<ThresholdBinding>(bindings["active"]);
            activeBinding.ApplyFunc = HandleSetActive;

            _bindings.Add(activeBinding);
        }

        public void Awake()
        {
            Initialize();
            Source.loop = false;
        }

        protected bool wasActive = false;
        protected void HandleSetActive(bool active)
        {
            if (active && !wasActive)
            {
                Source.loop = true;

                if (_endClipCoroutine != null)
                {
                    StopCoroutine(_endClipCoroutine);
                }

                if (!Source.isPlaying)
                {
                    Source.Play();
                }
            }
            else if (!active && wasActive)
            {
                Source.loop = false;
                
                if (EndingClip && (_endClipCoroutine == null))
                {
                    _endClipCoroutine = StartCoroutine(WaitForEndClipPlay());
                }
            }

            wasActive = active;
        }

        protected Coroutine _endClipCoroutine;

        protected IEnumerator WaitForEndClipPlay()
        {
            float remaining = Mathf.Min(Clip.length - Source.time, 0);
            yield return WaitFor.SecondsRealtime(remaining);

            Source.PlayOneShot(EndingClip, EndClipVolume);
            yield return WaitFor.SecondsRealtime(EndingClip.length);
            _endClipCoroutine = null;
        }
    }
}