using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Caboose Controller Proxy")]
    public class CabooseControllerProxy : MonoBehaviour, ISelfValidation
    {
        public GameObject cabTeleportDestinationCollidersGO = null!;

        public SelfValidationResult Validate(out string message)
        {
            if (cabTeleportDestinationCollidersGO == null)
            {
                return this.FailForNull(nameof(cabTeleportDestinationCollidersGO), out  message);
            }

            return this.Pass(out message);
        }
    }
}
