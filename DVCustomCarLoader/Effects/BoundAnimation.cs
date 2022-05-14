using CCL_GameScripts.CabControls;
using DVCustomCarLoader.LocoComponents;
using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.Effects
{
    public class BoundAnimation : MonoBehaviour, ILocoEventAcceptor, ICabControlAcceptor
    {
        public Animator Animator;
        protected SimEventType[] EventBindings;
        public SimEventType[] EventTypes => EventBindings;
        public CabInputType[] InputBindings;

        public void Start()
        {
            if (!Animator || (EventTypes.SafeAny() && InputBindings.SafeAny()))
            {
                Main.Warning("BoundAnimation is missing animator and/or event binding");
                enabled = false;
                return;
            }
        }

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is float floatVal)
            {
                Animator.SetFloat(eventInfo.EventType.ToString(), floatVal);
            }
            else if (eventInfo.NewValue is bool boolVal)
            {
                Animator.SetBool(eventInfo.EventType.ToString(), boolVal);
            }
        }

        protected void HandleCabControlUpdated(object sender, float newValue)
        {
            CabInputType inputType = ((CabInputRelay)sender).Binding;
            Animator.SetFloat(inputType.ToString(), newValue);
        }

        public void RegisterControl(CabInputRelay controlRelay)
        {
            controlRelay.AddListener(HandleCabControlUpdated);
        }

        public bool AcceptsControlOfType(CabInputType inputType)
        {
            if (!InputBindings.SafeAny()) return false;
            return inputType.EqualsOneOf(InputBindings);
        }
    }
}