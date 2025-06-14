using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    public class CoachLightsController : MonoBehaviour
    {
        [Header("Interior Lights")]
        public Light[] InteriorLights = new Light[0];
        public Renderer[] InteriorLamps = new Renderer[0];
        public Material LampsOn = null!;
        public Material LampsOff = null!;

        [Header("Taillights")]
        public GameObject[] TaillightGlaresF = new GameObject[0];
        public GameObject[] TaillightGlaresR = new GameObject[0];
        public Renderer[] TaillightLampsF = new Renderer[0];
        public Renderer[] TaillightLampsR = new Renderer[0];
        public Material TaillightOn = null!;
        public Material TaillightOff = null!;
    }
}
