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
        [SerializeField]
        protected HeadlightMUDependency multipleUnityDependent;
        public HeadlightProxy[] headlights;
        public Light[] lightSources;
    }
}
