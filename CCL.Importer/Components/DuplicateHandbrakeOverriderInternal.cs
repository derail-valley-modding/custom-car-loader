using CCL.Importer.Implementations;
using CCL.Types;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class DuplicateHandbrakeOverriderInternal : MonoBehaviour, BaseControlsOverrider.IHandbrakeOverrider
    {
        public CouplerDirection Direction;

        public bool AlwaysCopy = false;
        [CarKindField]
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];

        public HandbrakeControl GetHandbrake(TrainCar car)
        {
            return new DuplicateHandbrake(car, this);
        }
    }
}
