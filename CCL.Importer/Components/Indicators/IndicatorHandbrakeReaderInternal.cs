using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Components.Indicators
{
    internal class IndicatorHandbrakeReaderInternal : MonoBehaviour
    {
        public string FuseId = string.Empty;

        private TrainCar _car = null!;
        private Indicator _indicator = null!;
        private Fuse _fuse = null!;

        private void Awake()
        {
            _car = TrainCar.Resolve(gameObject);
            _indicator = GetComponent<Indicator>();

            if (_indicator == null)
            {
                Debug.LogError("Can't find Indicator on " + base.gameObject.name + ". Ignoring init");
                Object.Destroy(this);
                return;
            }
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(FuseId))
            {
                var simController = _car.SimController;
                var simulationFlow = (simController != null) ? simController.simFlow : null;

                if (simulationFlow == null)
                {
                    Debug.LogError("Couldn't find SimFlow, ignoring FuseId won't be functional!");
                    return;
                }
                if (!simulationFlow.TryGetFuse(FuseId, out _fuse))
                {
                    Debug.LogError("[" + base.gameObject.GetPath() + "]: IndicatorHandbrakeReader isn't initialized properly, fuse won't be set");
                }
            }
        }

        private void Update()
        {
            if (_fuse != null && !_fuse.State)
            {
                _indicator.Value = _indicator.minValue;
                return;
            }

            _indicator.Value = _car.brakeSystem.handbrakePosition;
        }
    }
}
