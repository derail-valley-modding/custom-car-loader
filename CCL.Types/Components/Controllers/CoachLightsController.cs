using System.Linq;
using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Coach Lights Controller")]
    public class CoachLightsController : MonoBehaviour, ISelfValidation
    {
        public bool SelfPowered = false;

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

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (InteriorLights.Any(x => x is null))
            {
                return this.FailForNullEntries(nameof(InteriorLights), out message, out highlight);
            }

            if (InteriorLamps.Any(x => x is null))
            {
                return this.FailForNullEntries(nameof(InteriorLamps), out message, out highlight);
            }

            if (TaillightGlaresF.Any(x => x is null))
            {
                return this.FailForNullEntries(nameof(TaillightGlaresF), out message, out highlight);
            }

            if (TaillightGlaresR.Any(x => x is null))
            {
                return this.FailForNullEntries(nameof(TaillightGlaresR), out message, out highlight);
            }

            if (TaillightLampsF.Any(x => x is null))
            {
                return this.FailForNullEntries(nameof(TaillightLampsF), out message, out highlight);
            }

            if (TaillightLampsR.Any(x => x is null))
            {
                return this.FailForNullEntries(nameof(TaillightLampsR), out message, out highlight);
            }

            if (LampsOn == null)
            {
                message = $"{nameof(LampsOn)} material is null";
                highlight = nameof(LampsOn);
                return SelfValidationResult.Warning;
            }

            if (LampsOff == null)
            {
                message = $"{nameof(LampsOff)} material is null";
                highlight = nameof(LampsOff);
                return SelfValidationResult.Warning;
            }

            if (TaillightOn == null)
            {
                message = $"{nameof(TaillightOn)} material is null";
                highlight = nameof(TaillightOn);
                return SelfValidationResult.Warning;
            }

            if (TaillightOff == null)
            {
                message = $"{nameof(TaillightOff)} material is null";
                highlight = nameof(TaillightOff);
                return SelfValidationResult.Warning;
            }

            return this.Pass(out  message, out highlight);
        }
    }
}
