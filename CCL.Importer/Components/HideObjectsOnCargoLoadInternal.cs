using DV.ThingTypes;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class HideObjectsOnCargoLoadInternal : MonoBehaviour
    {
        public GameObject[] Objects = new GameObject[0];

        private TrainCar _car = null!;

        private void Start()
        {
            _car = TrainCar.Resolve(gameObject);

            if (_car == null)
            {
                Debug.LogError("Could not find TrainCar for HideObjectsOnCargoLoad!");
                Destroy(this);
                return;
            }

            _car.CargoLoaded += Hide;
            _car.CargoUnloaded += Show;

            Hide(_car.LoadedCargo);
        }

        private void OnDestroy()
        {
            _car.CargoLoaded -= Hide;
            _car.CargoUnloaded -= Show;
        }

        public void Hide(CargoType type)
        {
            if (type == CargoType.None)
            {
                Show();
                return;
            }

            foreach (GameObject obj in Objects)
            {
                obj.SetActive(false);
            }
        }

        public void Show()
        {
            foreach (GameObject obj in Objects)
            {
                obj.SetActive(true);
            }
        }
    }
}
