using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    public abstract class HeadlightsSubControllerBaseProxy : MonoBehaviour, ISelfValidation
    {
        public enum HeadlightMUDependency
        {
            None,
            Front,
            Rear
        }

        public bool isFront;
        public HeadlightMUDependency multipleUnityDependent;
        public HeadlightProxy[] headlights = new HeadlightProxy[0];
        public Light[] lightSources = new Light[0];

        public SelfValidationResult Validate(out string message)
        {
            if (headlights.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(headlights), out message);
            }

            if (lightSources.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(lightSources), out message);
            }

            return this.Pass(out message);
        }
    }
}
