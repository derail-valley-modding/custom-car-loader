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
        public bool UseAbsoluteMappedValue = false;
    }

    public abstract class CopiedCabIndicator : CopiedCabDevice, IBoundIndicator
    {
        public SimEventType OutputBinding;
        public CabInputType ControlBinding;

        public OutputBinding Binding => new OutputBinding() { SimEventType = OutputBinding, CabInputType = ControlBinding };

        [SerializeField]
        [HideInInspector]
        private SimEventType _lastEventType;
        [SerializeField]
        [HideInInspector]
        private CabInputType _lastControlBinding;

        public void OnValidate()
        {
            if ((OutputBinding != SimEventType.None) && (OutputBinding != _lastEventType))
            {
                ControlBinding = CabInputType.None;
            }
            else if ((ControlBinding != CabInputType.None) && (ControlBinding != _lastControlBinding))
            {
                OutputBinding = SimEventType.None;
            }

            _lastEventType = OutputBinding;
            _lastControlBinding = ControlBinding;
        }
    }
}