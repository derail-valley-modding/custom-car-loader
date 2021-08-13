using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV.CabControls.NonVR;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomDieselReverserLock : MonoBehaviour
    {
		private const float DRIVING_REVERSER_DRAG = 82f;
		private const float DRIVING_REVERSER_ANGULAR_DRAG = 1000f;

		private CustomLocoControllerDiesel controller;
		private Rigidbody rb;
		private GrabHandlerHingeJoint nonVrGrab;
		private LeverNonVR nonVrLever;

		private float normalDrag;
		private float normalAngularDrag;
		private bool reverserBlocked;
		private bool initCompleted;

		private IEnumerator Start()
		{
			controller = TrainCar.Resolve(gameObject).GetComponent<CustomLocoControllerDiesel>();

			while( (rb = GetComponent<Rigidbody>()) == null )
			{
				yield return WaitFor.SecondsRealtime(0.5f);
			}

			nonVrGrab = GetComponent<GrabHandlerHingeJoint>();
			nonVrLever = GetComponent<LeverNonVR>();

			normalAngularDrag = rb.angularDrag;
			normalDrag = rb.drag;
			initCompleted = true;

			yield break;
		}

		private void SetBlocked( bool blocked )
		{
			if( blocked == reverserBlocked )
			{
				return;
			}

			if( blocked )
			{
				if( nonVrGrab )
				{
					nonVrGrab.SetMovingDisabled(true);
				}

				rb.drag = DRIVING_REVERSER_DRAG;
				rb.angularDrag = DRIVING_REVERSER_ANGULAR_DRAG;
			}
			else
			{
				// not blocked
				if( nonVrGrab )
				{
					nonVrGrab.SetMovingDisabled(false);
				}

				rb.drag = normalDrag;
				rb.angularDrag = normalAngularDrag;
			}

			if( nonVrLever )
			{
				nonVrLever.IsScrollingBlocked = blocked;
			}

			reverserBlocked = blocked;
		}

		private void Update()
		{
			if( !initCompleted )
			{
				return;
			}

			SetBlocked(controller.targetThrottle > 0.05f && controller.EngineRunning);
		}
	}
}
