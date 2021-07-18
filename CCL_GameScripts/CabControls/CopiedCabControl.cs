using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public abstract class CopiedCabControl : MonoBehaviour
    {
        public CabInputType InputBinding;

        public abstract (BaseTrainCarType, string) GetSourceObject();
    }

    public enum CabInputType
    {
        IndependentBrake,
        TrainBrake,
        Throttle,
        Reverser,
        Horn
    }
}