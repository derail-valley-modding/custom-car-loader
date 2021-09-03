using System;
using CCL_GameScripts.CabControls;
using DV.Util.EventWrapper;

namespace DVCustomCarLoader.LocoComponents
{
    public interface ILocoEventProvider
    {
        bool Bind( SimEventType eventType, ILocoEventAcceptor listener );
    }

    public interface ILocoEventAcceptor
    {
        Action<bool> BoolHandler { get; }
        Action<LocoSimulationEvents.Amount> AmountHandler { get; }
        Action<LocoSimulationEvents.CouplingIntegrityInfo> CouplingHandler { get; }
    }
}