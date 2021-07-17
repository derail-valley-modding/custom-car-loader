using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class LeverSetup : ControlSetupBase
    {
        public override CabControlType ControlType => CabControlType.Lever;

		[Header("Lever")]
		public bool UseNotches = true; // useSteppedJoint
		public int Notches = 20;
		public bool InvertDirection = false;

		[Header("Hinge Joint")]
		public Vector3 JointAxis = Vector3.up;
		//public bool UseLimits = true;
		public float JointLimitMin = 0;
		public float JointLimitMax = 90;
		//public bool UseSpring = true;
		public float JointSpring = 100f;
		public float JointDamper = 10;

		[Header("Rigidbody")]
		public float RigidbodyMass = 30;
		public float RigidbodyDrag = 8;
		//public float RigidbodyAngularDrag = 0;

		[Header("NonVR")]
		public float HoverScrollMagnitude = 4;
		public float ScrollWheelSpring = 50;
		public GameObject StaticInteractionArea = null;

		[Header("VR")]
		public float MaxAppliedForce = float.PositiveInfinity;
		public float PullingForceMultiplier = 1;
		[InspectorName("Interaction Point (optional)")]
		public Transform InteractionPoint = null;
		public bool VibrateAtLimit = false;

		// TODO: Audio

#if UNITY_EDITOR
		[MethodButton(
			nameof(IndependentBrakeDefaults),
			nameof(TrainBrakeDefaults),
			nameof(DieselThrottleDefaults),
			nameof(DieselReverserDefaults))]
		[SerializeField]
		private bool editorFoldout;

		public void IndependentBrakeDefaults()
		{
			UseNotches = true;
			Notches = 20;
			InvertDirection = false;

			JointAxis = Vector3.up;
			JointLimitMin = -61;
			JointLimitMax = 0;
			JointSpring = 100f;
			JointDamper = 10;

			RigidbodyMass = 30;
			RigidbodyDrag = 8;

			HoverScrollMagnitude = 4;
			ScrollWheelSpring = 50;

			MaxAppliedForce = float.PositiveInfinity;
			PullingForceMultiplier = 1;
			VibrateAtLimit = false;
		}

		public void TrainBrakeDefaults()
        {
			UseNotches = true;
			Notches = 20;
			InvertDirection = false;

			JointAxis = Vector3.up;
			JointLimitMin = 0;
			JointLimitMax = 72;
			JointSpring = 100f;
			JointDamper = 10;

			RigidbodyMass = 30;
			RigidbodyDrag = 8;

			HoverScrollMagnitude = 4;
			ScrollWheelSpring = 50;

			MaxAppliedForce = float.PositiveInfinity;
			PullingForceMultiplier = 1;
			VibrateAtLimit = false;
		}

		public void DieselThrottleDefaults()
        {
			UseNotches = true;
			Notches = 8;
			InvertDirection = false;

			JointAxis = Vector3.up;
			JointLimitMin = -52;
			JointLimitMax = 0;
			JointSpring = 100f;
			JointDamper = 0;

			RigidbodyMass = 30;
			RigidbodyDrag = 16;

			HoverScrollMagnitude = 1;
			ScrollWheelSpring = 100;

			MaxAppliedForce = float.PositiveInfinity;
			PullingForceMultiplier = 1;
			VibrateAtLimit = false;
		}

		public void DieselReverserDefaults()
        {
			UseNotches = true;
			Notches = 3;
			InvertDirection = false;

			JointAxis = Vector3.up;
			JointLimitMin = -25;
			JointLimitMax = 65;
			JointSpring = 25;
			JointDamper = 2;

			RigidbodyMass = 30;
			RigidbodyDrag = 0;

			HoverScrollMagnitude = 1;
			ScrollWheelSpring = 8;

			MaxAppliedForce = float.PositiveInfinity;
			PullingForceMultiplier = 1;
			VibrateAtLimit = false;
		}
#endif
	}
}
