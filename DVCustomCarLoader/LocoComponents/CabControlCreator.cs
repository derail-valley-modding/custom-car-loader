using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts.CabControls;
using DV.CabControls.Spec;

namespace DVCustomCarLoader.LocoComponents
{
    public static class CabControlCreator
    {
        public static void Create( ControlSetupBase control )
        {
            switch( control.ControlType )
            {
                case CabControlType.Lever:
                    CreateLever((LeverSetup)control);
                    break;

                default:
                    Main.Warning($"Unknown cab control type on {control.gameObject.name}");
                    break;
            }
        }

        private static void ApplyBaseConfig( ControlSetupBase setup, ControlSpec spec )
        {
            spec.colliderGameObjects = setup.InteractionColliders.ToArray();
        }

        private static void CreateLever( LeverSetup setup )
        {
            var spec = setup.gameObject.AddComponent<Lever>();
            ApplyBaseConfig(setup, spec);

            spec.useSteppedJoint = setup.UseNotches;
            spec.notches = setup.Notches;
            spec.invertDirection = setup.InvertDirection;

            spec.jointAxis = setup.JointAxis;
            spec.jointLimitMin = setup.JointLimitMin;
            spec.jointLimitMax = setup.JointLimitMax;
            spec.jointSpring = setup.JointSpring;
            spec.jointDamper = setup.JointDamper;

            spec.rigidbodyMass = setup.RigidbodyMass;
            spec.rigidbodyDrag = setup.RigidbodyDrag;

            spec.scrollWheelHoverScroll = setup.HoverScrollMagnitude;
            spec.scrollWheelSpring = setup.ScrollWheelSpring;

            spec.maxForceAppliedMagnitude = setup.MaxAppliedForce;
            spec.pullingForceMultiplier = setup.PullingForceMultiplier;
            spec.interactionPoint = setup.InteractionPoint;
            spec.limitVibration = setup.VibrateAtLimit;

            if( setup.StaticInteractionArea )
            {
                spec.nonVrStaticInteractionArea = setup.StaticInteractionArea.AddComponent<StaticInteractionArea>();
            }

            UnityEngine.Object.Destroy(setup);
        }
    }
}
