using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public abstract class CopiedCabControl : MonoBehaviour
    {
        public bool ReplaceThisObject = false;

        public abstract (BaseTrainCarType, string) GetSourceObject();
    }

    public abstract class CopiedCabInput : CopiedCabControl
    {
        public CabInputType InputBinding;
    }

    public abstract class CopiedCabIndicator : CopiedCabControl
    {
        public CabIndicatorType OutputBinding;
    }
}