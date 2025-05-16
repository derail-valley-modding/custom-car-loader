using CCL.Importer.Components.Simulation;
using DV.WeatherSystem;
using LocoSim.Implementations;

namespace CCL.Importer.Implementations
{
    internal class TimeReader : SimComponent
    {
        public readonly Port HoursReadOut;
        public readonly Port MinutesReadOut;
        public readonly Port NormalizedReadOut;

        private WeatherPresetManager? _weather;

        public TimeReader(TimeReaderDefinitionInternal def) : base(def.ID)
        {

            HoursReadOut = AddPort(def.HoursReadOut);
            MinutesReadOut = AddPort(def.MinutesReadOut);
            NormalizedReadOut = AddPort(def.NormalizedReadOut);

            var driver = WeatherDriver.Instance;
            _weather = driver != null ? driver.manager : null;
        }

        public override void Tick(float delta)
        {
            if (_weather == null) return;

            HoursReadOut.Value = _weather.DateTime.Hour;
            MinutesReadOut.Value = _weather.DateTime.Minute;
            NormalizedReadOut.Value = (float)(_weather.DateTime.TimeOfDay.TotalMinutes / 1440.0f);
        }
    }
}
