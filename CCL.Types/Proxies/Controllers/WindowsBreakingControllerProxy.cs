using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class WindowsBreakingControllerProxy : MonoBehaviour
    {
        [Header("Window")]
        public GameObject brokenWindowsPrefab;
        public GameObject windowsRenderGO;
        public GameObject brokenWindowsRenderGO;
        public GameObject[] windowColliders;
        public AudioClip windowsBreakingAudio;
    }
}
