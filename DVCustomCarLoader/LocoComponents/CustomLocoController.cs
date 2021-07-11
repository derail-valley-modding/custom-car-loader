using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoController : LocoControllerBase
    {
        
    }

    public abstract class CustomLocoController<TSim> : CustomLocoController
        where TSim : CustomLocoSimulation
    {
        protected TSim sim;
        protected DebtTrackerCustomLoco locoDebt;
    }
}