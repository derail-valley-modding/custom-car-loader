using System;
using CCL_GameScripts.CabControls;

namespace DVCustomCarLoader.LocoComponents
{
    public interface ICabControlAcceptor
    {
        //(Action<float>, Func<float>) RegisterControl( CabInputRelay controlRelay );
        void RegisterControl( CabInputRelay controlRelay );

        bool AcceptsControlOfType( CabInputType inputType );
    }
}