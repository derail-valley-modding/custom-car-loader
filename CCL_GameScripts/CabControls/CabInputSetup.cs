using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class CabInputSetup : MonoBehaviour
	{
		public GameObject Brake;
		public GameObject IndependentBrake;
		public GameObject Reverser;
		public GameObject Throttle;

		public void SetInputObject( CabInputType inputType, GameObject controlRoot )
        {
			switch( inputType )
            {
				case CabInputType.TrainBrake:
					Brake = controlRoot;
					break;

				case CabInputType.IndependentBrake:
					IndependentBrake = controlRoot;
					break;

				case CabInputType.Reverser:
					Reverser = controlRoot;
					break;

				case CabInputType.Throttle:
					Throttle = controlRoot;
					break;

				default:
					Debug.LogWarning("Tried to set input of unsupported type");
					break;
            }
        }
	}
}