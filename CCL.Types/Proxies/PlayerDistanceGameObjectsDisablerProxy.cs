using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Optimizers/Player Distance GameObjects Disabler Proxy")]
    public class PlayerDistanceGameObjectsDisablerProxy : MonoBehaviour, ISelfValidation
    {
        public List<GameObject> optimizingGameObjects = new List<GameObject>();
        // Vanilla only uses squared distance, so there's a special mapping for this
        // field to square it. Using non squared for ease of use.
        public float disableDistance = 500f;
        public float checkPeriodPerGO = 2f;

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (optimizingGameObjects.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(optimizingGameObjects), out message, out highlight);
            }

            return this.Pass(out message, out highlight);
        }
    }
}
