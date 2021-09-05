using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public abstract class CopiedCabDevice : MonoBehaviour
    {
        public bool ReplaceThisObject = false;

        public abstract (BaseTrainCarType, string) GetSourceObject();
    }

    public abstract class CopiedCabInput : CopiedCabDevice
    {
        public CabInputType InputBinding;

        public float MappedMinimum = 0;
        public float MappedMaximum = 1;
    }

    public abstract class CopiedCabIndicator : CopiedCabDevice
    {
        public CabIndicatorType OutputBinding;
    }
}