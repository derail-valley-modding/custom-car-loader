using System;
using CCL_GameScripts.CabControls;
using DV.Util.EventWrapper;

namespace DVCustomCarLoader.LocoComponents
{
    public interface ILocoEventProvider
    {
        SimEventWrapper GetEvent( SimEventType eventType );
    }

    public struct SimEventWrapper
    {
        public static readonly SimEventWrapper Empty = new SimEventWrapper();

        private readonly event_<bool>? boolEvent;
        private readonly event_<LocoSimulationEvents.Amount>? amountEvent;
        private readonly event_<LocoSimulationEvents.CouplingIntegrityInfo>? coupleEvent;

        public bool HasEvent => boolEvent.HasValue || amountEvent.HasValue || coupleEvent.HasValue;

        public void Bind(
            Action<bool> boolHandler, 
            Action<LocoSimulationEvents.Amount> amountHandler, 
            Action<LocoSimulationEvents.CouplingIntegrityInfo> coupleHandler )
        {
            if( boolEvent.HasValue ) boolEvent.Value.Register(boolHandler);
            if( amountEvent.HasValue ) amountEvent.Value.Register(amountHandler);
            if( coupleEvent.HasValue ) coupleEvent.Value.Register(coupleHandler);
        }

        public SimEventWrapper( event_<bool> e )
        {
            boolEvent = e;
            amountEvent = null;
            coupleEvent = null;
        }

        public SimEventWrapper( event_<LocoSimulationEvents.Amount> e )
        {
            boolEvent = null;
            amountEvent = e;
            coupleEvent = null;
        }

        public SimEventWrapper( event_<LocoSimulationEvents.CouplingIntegrityInfo> e )
        {
            boolEvent = null;
            amountEvent = null;
            coupleEvent = e;
        }

        public static implicit operator SimEventWrapper( event_<bool> e ) => new SimEventWrapper(e);
        public static implicit operator SimEventWrapper( event_<LocoSimulationEvents.Amount> e ) => new SimEventWrapper(e);
        public static implicit operator SimEventWrapper( event_<LocoSimulationEvents.CouplingIntegrityInfo> e ) => new SimEventWrapper(e);

        public static implicit operator bool( SimEventWrapper wrapper ) => wrapper.HasEvent;
    }
}