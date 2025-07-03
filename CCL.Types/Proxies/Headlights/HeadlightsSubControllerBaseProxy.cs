using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    public abstract class HeadlightsSubControllerBaseProxy : MonoBehaviour
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
    }
}
