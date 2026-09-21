using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Material Grabber (Particle)")]
    public class MaterialGrabberParticle : MonoBehaviour, ICustomGrabberValidation
    {
        public ParticleSystem ParticleSystem = null!;
        public string Replacement = string.Empty;

        private void Reset()
        {
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public bool IsValid(out string error)
        {
            if (ParticleSystem == null)
            {
                error = $"MaterialGrabberParticle in {gameObject.GetPath()} cannot have a null system.";
                return false;
            }

            if (!MaterialGrabber.MaterialNames.Contains(Replacement))
            {
                error = $"MaterialGrabberParticle in {gameObject.GetPath()} does not have a valid replacement ({Replacement}).";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
