using CCL_GameScripts.CabControls;
using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class BridgedEventManager : LocoEventManager
    {
        private LocoEventManager _linkedManager;
        public LocoEventManager LinkedManager
        {
            get => _linkedManager;
            set
            {
                if (_linkedManager)
                {
                    _linkedManager.OnDispatch -= HandleDispatchedEvent;
                }

                _linkedManager = value;
                if (value)
                {
                    _linkedManager.OnDispatch += HandleDispatchedEvent;
                }
            }
        }

        public override void Dispatch(ILocoEventProvider provider, SimEventType eventType, object newVal)
        {
            if (LinkedManager)
            {
                LinkedManager.Dispatch(provider, eventType, newVal);
            }
            else
            {
                HandleDispatchedEvent(provider, eventType, newVal);
            }
        }

        protected void HandleDispatchedEvent(ILocoEventProvider provider, SimEventType eventType, object newVal)
        {
            var acceptors = ExteriorEvents.GetAcceptors(eventType);
            foreach (var acceptor in acceptors)
            {
                acceptor.HandleEvent(new LocoEventInfo(provider, eventType, newVal));
            }

            acceptors = InteriorEvents.GetAcceptors(eventType);
            foreach (var acceptor in acceptors)
            {
                acceptor.HandleEvent(new LocoEventInfo(provider, eventType, newVal));
            }
        }
    }
}