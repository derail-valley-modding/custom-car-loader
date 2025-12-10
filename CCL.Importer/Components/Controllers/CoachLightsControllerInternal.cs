using DV.Customization;
using DV.Simulation.Cars;
using DV.WeatherSystem;
using UnityEngine;

namespace CCL.Importer.Components.Controllers
{
    internal class CoachLightsControllerInternal : MonoBehaviour
    {
        private static SpriteLightsEvent? s_events = null;

        private static SpriteLightsEvent Events => Extensions.GetCached(ref s_events,
            WorldTimeBasedEvents.Instance.GetComponent<SpriteLightsEvent>);

        public static bool StreetlightState => Events.LightTypeOn[(int)SpriteLightType.StreetSpriteLight];

        public Light[] InteriorLights = new Light[0];
        public Renderer[] InteriorLamps = new Renderer[0];
        public Material LampsOn = null!;
        public Material LampsOff = null!;
        public GameObject[] TaillightGlaresF = new GameObject[0];
        public GameObject[] TaillightGlaresR = new GameObject[0];
        public MeshRenderer[] TaillightLampsF = new MeshRenderer[0];
        public MeshRenderer[] TaillightLampsR = new MeshRenderer[0];
        public Material TaillightOn = null!;
        public Material TaillightOff = null!;

        private TrainCar _car = null!;
        private bool _currentState = false;

        private static bool IsDay
        {
            get
            {
                var time = WeatherDriver.Instance.manager.timeOfDay;
                return time > AutomaticHeadlightsController.DAYTIME_START && time < AutomaticHeadlightsController.DAYTIME_END;
            }
        }

        private bool IsFoggy
        {
            get
            {
                return WeatherDriver.Instance.GetFogDensity(transform.position) > 0.5f;
            }
        }

        private void Start()
        {
            _car = TrainCar.Resolve(gameObject);

            if (_car == null)
            {
                Debug.LogError("Could not find TrainCar for CoachLightsController, destroying self");
                Destroy(this);
                return;
            }

            _car.TrainsetChanged += TrainsetUpdate;
            WeatherDriver.Instance.manager.MinuteChanged += TimeUpdate;

            UpdateLights(ShouldBeOn(), true);
        }

        private void TrainsetUpdate(Trainset trainset)
        {
            UpdateLights(ShouldBeOn(), true);
        }

        private void TimeUpdate()
        {
            UpdateLights(ShouldBeOn());
        }

        private void UpdateLights(bool on, bool forced = false)
        {
            if (on == _currentState && !forced) return;

            foreach (var item in InteriorLights) item.enabled = on;
            foreach (var item in InteriorLamps) item.material = on ? LampsOn : LampsOff;

            bool redOnF = on && (_car.frontCoupler == null || _car.frontCoupler.coupledTo == null);

            foreach (var item in TaillightGlaresF) item.SetActive(redOnF);
            foreach (var item in TaillightLampsF) item.material = redOnF ? TaillightOn : TaillightOff;

            bool redOnR = on && (_car.rearCoupler == null || _car.rearCoupler.coupledTo == null);

            foreach (var item in TaillightGlaresR) item.SetActive(redOnR);
            foreach (var item in TaillightLampsR) item.material = redOnR ? TaillightOn : TaillightOff;

            _currentState = on;
        }

        private bool GetTrainsetPoweredState()
        {
            if (_car.trainset is null) return false;

            foreach (var loco in _car.trainset.GetAllLocos())
            {
                if (loco.TryGetComponent(out TrainCarCustomization tcc) &&
                    tcc.electronicsFuse != null &&
                    tcc.electronicsFuse.State)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldBeOn() => /*!IsDay*/ StreetlightState && GetTrainsetPoweredState();
    }
}
