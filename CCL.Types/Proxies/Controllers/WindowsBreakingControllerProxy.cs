using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Windows Breaking Controller Proxy")]
    public class WindowsBreakingControllerProxy : MonoBehaviour
    {
        [Header("Window")]
        public GameObject brokenWindowsPrefab = null!;
        public GameObject windowsRenderGO = null!;
        public GameObject brokenWindowsRenderGO = null!;
        public GameObject[] windowColliders = new GameObject[0];
        public AudioClip windowsBreakingAudio = null!;
    }
}
