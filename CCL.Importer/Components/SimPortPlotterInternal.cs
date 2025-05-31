using CCL.Types;
using DV.Simulation.Cars;
using LocoSim.Definitions;
using LocoSim.Implementations;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class SimPortPlotterInternal : MonoBehaviour
    {
        private const int WindowId = 9001;
        // Window size.
        private const float WindowWidth = 1010.0f;
        private const float WindowHeight = 720.0f;
        // Scroll area size.
        private const float ScrollWidth = 1000.0f;
        private const float ScrollHeight = 620.0f;
        // Box size.
        private const float BoxWidth = 800.0f;
        private const float BoxHeight = 130.0f;
        private const float BoxSpacing = 20.0f;
        private const float BoxTotalHeight = BoxHeight + BoxSpacing;
        // Label size.
        private const float LabelWidth= 80.0f;
        private const float LabelHeight = 20.0f;
        // Misc.
        private const float SideOffset = 100.0f;
        private const float ButtonSize = 17.5f;
        private const float ButtonSelectWidth = 100f;
        private const float ButtonSelectHeight = 20f;
        private const float LowerButtonsPosY = WindowHeight - 30.0f;
        private const float CloseButtonPosX = WindowWidth - 18.75f;

        private class PortData
        {
            public const int DataLength = 1000;
            public const float Offset = BoxWidth / DataLength;

            public string Id;
            public Port Port;
            public float Min = 0;
            public float Max = 1;
            public float Last;
            public float ZeroOffset;
            public Queue<float> Values;
            public string Unit;

            public PortData(string id, Port port, string? unit = null)
            {
                Id = id;
                Port = port;
                Values = new Queue<float>(DataLength);

                Unit = unit ?? (id.Contains("NORMALIZED") ? string.Empty : GetUnitForValueType(port.valueType));
            }

            public void Reset()
            {
                Min = 0;
                Max = 1;
                Last = 0;
                ZeroOffset = 0;
                Values.Clear();
            }

            public void Update()
            {
                if (Values.Count == 0)
                {
                    SetStartingValues();
                }

                if (Values.Count >= DataLength)
                {
                    Values.Dequeue();
                }

                var value = Port.Value;

                if (value < Min)
                {
                    Min = value;
                }

                if (value > Max)
                {
                    Max = value;
                }

                Values.Enqueue(value);
                Last = value;

                ZeroOffset = Mathf.InverseLerp(Min, Max, 0);
            }

            public void Draw(Rect bounds)
            {
                //GUI.color = GetColourForPortValueType(Port.valueType);

                // If there's no difference to draw yet, draw a single line with the correct length.
                if (Min == Max)
                {
                    GUIHelper.DrawLine(ToPosition(0, bounds.height), ToPosition(Offset * Values.Count, bounds.height), 1.0f);
                    return;
                }

                Vector2? prev = null;
                float x = 0;

                foreach (var item in Values)
                {
                    var mapped = GetMapped(item);

                    if (!prev.HasValue)
                    {
                        prev = ToPosition(x, mapped);
                        continue;
                    }

                    x += Offset;
                    var current = ToPosition(x, mapped);
                    GUIHelper.DrawPixel(current);

                    // To bridge large vertical gaps.
                    var dif = current.y - prev.Value.y;
                    if (dif * dif > 4.0f)
                    {
                        GUIHelper.DrawLine(prev.Value, current, 1);
                    }

                    prev = current;
                }

                float GetMapped(float value)
                {
                    // Slight offset from 0-height to keep within the box outlines.
                    return Extensions.Mapf(Max, Min, 1, bounds.height - 2, value);
                }

                Vector2 ToPosition(float x, float y)
                {
                    // Offsets to the box's origin.
                    return new Vector2(x + bounds.xMin, y + bounds.yMin);
                }
            }

            private void SetStartingValues()
            {
                Min = Mathf.Min(0, Port.Value);
                Max = Mathf.Max(0, Port.Value);
            }

            public static Color GetColourForPortValueType(PortValueType valueType) => valueType switch
            {
                PortValueType.GENERIC => new Color(0.90f, 0.90f, 0.90f),
                PortValueType.CONTROL => new Color(0.80f, 0.80f, 0.80f),
                PortValueType.STATE => new Color(0.55f, 1.00f, 0.85f),
                PortValueType.DAMAGE => new Color(1.00f, 0.35f, 0.90f),
                PortValueType.POWER => new Color(0.80f, 0.20f, 0.90f),
                PortValueType.TORQUE => new Color(0.45f, 0.90f, 1.00f),
                PortValueType.FORCE => new Color(0.45f, 1.00f, 0.90f),
                PortValueType.TEMPERATURE => new Color(1.00f, 0.40f, 0.40f),
                PortValueType.RPM => new Color(0.70f, 0.70f, 0.70f),
                PortValueType.AMPS => new Color(1.00f, 1.00f, 0.20f),
                PortValueType.VOLTS => new Color(0.20f, 1.00f, 0.90f),
                PortValueType.HEAT_RATE => new Color(1.00f, 0.80f, 0.20f),
                PortValueType.PRESSURE => new Color(0.50f, 1.00f, 0.60f),
                PortValueType.MASS_RATE => new Color(0.60f, 1.00f, 0.70f),
                PortValueType.OHMS => new Color(0.30f, 0.80f, 1.00f),
                PortValueType.FUEL => new Color(0.85f, 0.55f, 0.20f),
                PortValueType.OIL => new Color(0.20f, 0.55f, 0.85f),
                PortValueType.SAND => new Color(0.95f, 0.90f, 0.30f),
                PortValueType.WATER => new Color(0.45f, 0.75f, 1.00f),
                PortValueType.COAL => new Color(0.50f, 0.50f, 0.50f),
                PortValueType.ELECTRIC_CHARGE => new Color(0.50f, 1.00f, 1.00f),
                _ => Color.white,
            };

            public static string GetUnitForValueType(PortValueType valueType) => valueType switch
            {
                PortValueType.POWER => "W",
                PortValueType.TORQUE => "Nm",
                PortValueType.FORCE => "N",
                PortValueType.TEMPERATURE => "°C",
                PortValueType.AMPS => "A",
                PortValueType.VOLTS => "V",
                PortValueType.PRESSURE => "bar",
                PortValueType.OHMS => "Ω",
                PortValueType.FUEL => "l",
                PortValueType.OIL => "l",
                PortValueType.SAND => "kg",
                PortValueType.WATER => "l",
                PortValueType.COAL => "kg",
                PortValueType.ELECTRIC_CHARGE => "C",
                _ => string.Empty,
            };
        }

        private static List<SimPortPlotterInternal> s_windows = new();
        private static GUIStyle? s_style = null;
        private static bool s_round = false;

        private static GUIStyle Style => Extensions.GetCached(ref s_style, () => new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        });

        public int TickRate = 5;
        public List<string> PortIds = new();
        public List<string> PortReferenceIds = new();
        public bool UseColours = true;
        public bool AddDummyForcePort = true;
        public bool AddDummyPowerPort = true;

        private TrainCar _car = null!;
        private List<PortData> _ports = new();
        private Rect _windowRect = new(10f, 0f, WindowWidth, WindowHeight);
        private Vector2 _scroll = Vector2.zero;
        private int _ticking = 0;
        private bool _record = false;
        private bool _active = true;
        private DrivingForce? _drivingForce;
        private Port? _dummyForce;
        private Port? _dummyPower;
        private Port? _dummyTrainsetForce;
        private Port? _dummyTrainsetPower;
        private string _portToAdd = string.Empty;

        private void Start()
        {
            _car = TrainCar.Resolve(gameObject);

            if (_car == null)
            {
                Debug.LogError("Could not find TrainCar for SimPortPlotter");
                return;
            }

            var controller = _car.SimController;

            if (controller == null)
            {
                Debug.LogError("TrainCar has no SimController for SimPortPlotter");
                return;
            }

            var flow = controller.SimulationFlow;
            var map = new Dictionary<string, Port>();

            foreach (var comp in flow.OrderedSimComps)
            {
                foreach (var port in comp.GetAllPorts())
                {
                    map[port.id] = port;
                }

                foreach (var portReference in comp.GetAllPortReferences())
                {
                    map[portReference.id] = portReference.GetPort();
                }
            }

            foreach (var id in PortIds)
            {
                if (map.TryGetValue(id, out var port))
                {
                    _ports.Add(new PortData(id, port));
                }
            }

            foreach (var id in PortReferenceIds)
            {
                if (map.TryGetValue(id, out var port))
                {
                    _ports.Add(new PortData($"{id} (R)", port));
                }
            }

            if (AddDummyForcePort && controller.drivingForce != null)
            {
                _drivingForce = controller.drivingForce;
                _dummyForce = new Port("car", new PortDefinition(
                    PortType.READONLY_OUT,
                    PortValueType.FORCE,
                    "FORCE"));

                _ports.Add(new PortData("GENERATED_FORCE", _dummyForce));
            }

            if (AddDummyPowerPort && controller.drivingForce != null)
            {
                _drivingForce = controller.drivingForce;
                _dummyPower = new Port("car", new PortDefinition(
                    PortType.READONLY_OUT,
                    PortValueType.POWER,
                    "POWER"));

                _ports.Add(new PortData("GENERATED_POWER", _dummyPower));
            }

            flow.TickEvent += OnTick;

            s_windows.Add(this);
        }

        private void OnDestroy()
        {
            s_windows.Remove(this);
        }

        private void OnTick()
        {
            if (!_record) return;
            if (_ticking-- > 0) return;

            UpdateDummies();

            foreach (var port in _ports)
            {
                port.Update();
            }

            _ticking = TickRate;
        }

        private void UpdateDummies()
        {
            if (_drivingForce == null) return;

            if (_dummyForce != null)
            {
                _dummyForce.Value = _drivingForce.generatedForce;
            }

            if (_dummyPower != null)
            {
                _dummyPower.Value = _drivingForce.generatedForce * _drivingForce.train.GetAbsSpeed();
            }

            if (_dummyTrainsetForce == null || _dummyTrainsetPower == null) return;

            float force = 0;
            float power = 0;

            foreach (var loco in _car.trainset.AllLocos())
            {
                var driving = loco.SimController.drivingForce;

                if (driving == null) continue;

                force += driving.generatedForce;
                power += driving.generatedForce * loco.GetAbsSpeed();
            }

            _dummyTrainsetForce.Value = force;
            _dummyTrainsetPower.Value = power;
        }

        private void OnGUI()
        {
            int index = s_windows.IndexOf(this);

            if (_active)
            {
                _windowRect = GUILayout.Window(WindowId + index, _windowRect, Window, $"Loco Simulation Graph - {_car.ID}");
                return;
            }

            GUILayout.BeginVertical();

            if (GUI.Button(new Rect(0, index * ButtonSelectHeight, ButtonSelectWidth, ButtonSelectHeight), $"Graph ({_car.ID})"))
            {
                _active = !_active;
            }

            GUILayout.EndVertical();
        }

        private void Window(int id)
        {
            DrawButtons();

            // Prepare scroll view.
            GUI.skin.label.fontSize = 16;
            _scroll = GUI.BeginScrollView(
                new Rect(0f, 40f, ScrollWidth, ScrollHeight), _scroll,
                new Rect(0f, 40f, BoxWidth, _ports.Count * BoxTotalHeight), false, true);
            GUILayout.BeginVertical();

            PortData port;
            int count = _ports.Count;

            for (int i = 0; i < count && ((i + 1) * BoxTotalHeight <= ScrollHeight + _scroll.y); i++)
            {
                // Skip drawing outside bounds.
                if ((i + 1) * BoxTotalHeight < _scroll.y) continue;

                port = _ports[i];

                // Skip drawing if there are no values.
                if (port.Values.Count == 0) continue;

                float posY = 40f + i * BoxTotalHeight;
                float zeroOffset = BoxHeight * (1.0f - port.ZeroOffset);

                // Draw where 0 is if it is not at the borders.
                if (zeroOffset > 0 && zeroOffset < BoxHeight)
                {
                    GUI.Label(new Rect(0f, posY + zeroOffset, LabelWidth, LabelHeight), "0", Style);
                }

                // Draw max and min value labels.
                GUI.Label(new Rect(0f, posY, LabelWidth, LabelHeight), GetFormattedString(port.Max), Style);
                GUI.Label(new Rect(0f, posY + BoxHeight, LabelWidth, LabelHeight), GetFormattedString(port.Min), Style);

                posY += 10f;

                // Draw graph box.
                Rect box = new(SideOffset, posY, BoxWidth, BoxHeight);
                GUI.Box(box, GUIContent.none);
                GUI.color = GetColour(i);
                port.Draw(box);
                GUI.color = Color.white;
                GUI.BeginClip(box);
                GUI.Label(new Rect(10f, 0f, BoxWidth, LabelHeight), $"{port.Id}: {GetFormattedString(port.Last, port.Unit)}");
                GUI.EndClip();

                // Buttons to swap port order.
                var buttonHeight = BoxHeight / 4f;

                if (i > 0)
                {
                    if (GUI.Button(new Rect(LabelWidth + BoxWidth + 30f, posY, 30f, buttonHeight), "↑"))
                    {
                        _ports.Swap(i, i - 1);
                    }
                }
                if (i < count - 1)
                {
                    if (GUI.Button(new Rect(LabelWidth + BoxWidth + 30f, posY + BoxHeight - buttonHeight, 30f, buttonHeight), "↓"))
                    {
                        _ports.Swap(i, i + 1);
                    }
                }
            }

            GUILayout.EndVertical();
            GUI.EndScrollView();

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));

            static string GetFormattedString(float value, string? unit = null)
            {
                // 1 234 567 890 ->    1.23 G
                //   123 456 789 ->  123.45 M
                //    12 345 678 ->   12.34 M
                //     1 234 567 ->    1.23 M
                //       123 456 ->  123.45 k
                //        12 345 ->   12.34 k
                //         1 234 -> 1234.000

                unit ??= string.Empty;

                if (s_round && unit != "kg")
                {
                    if (value >= 1000000000)
                    {
                        return $"{value:0,,,.## G}{unit}";
                    }

                    if (value >= 1000000)
                    {
                        return $"{value:0,,.## M}{unit}";
                    }

                    if (value >= 10000)
                    {
                        return $"{value:0,.## k}{unit}";
                    }
                }

                return $"{value:0.###} {unit}";
            }
        }

        private void DrawButtons()
        {
            // Button to minimise (hide window).
            if (GUI.Button(new Rect(CloseButtonPosX - ButtonSize, 0f, ButtonSize, ButtonSize), "-"))
            {
                _active = !_active;
            }

            // Record data toggle.
            _record = GUI.Toggle(new(GetSideOffset(0), LowerButtonsPosY, SideOffset, 20f), _record, "Record data");

            // Rounding toggle.
            s_round = GUI.Toggle(new(GetSideOffset(1), LowerButtonsPosY, SideOffset, 20f), s_round, "Round values");

            // Text field and button to add ports easily.
            if (GUI.Button(new Rect(GetSideOffset(2), LowerButtonsPosY, SideOffset, ButtonSize), "Try Add Port"))
            {
                TryAddPort();
            }

            _portToAdd = GUI.TextField(new Rect(GetSideOffset(3), LowerButtonsPosY, SideOffset * 3, ButtonSize), _portToAdd);

            // Button to add trainset ports.
            if (GUI.Button(new Rect(GetSideOffset(6), LowerButtonsPosY, SideOffset, ButtonSize), "Trainset Ports"))
            {
                AddTrainsetPorts();
            }

            // Change background colour for the last 2 buttons.
            var colour = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            // Button to reset port data.
            if (GUI.Button(new Rect(GetSideOffset(8), LowerButtonsPosY, SideOffset, ButtonSize), "Clear data"))
            {
                ResetData();
            }

            // Button to close (disable GO).
            if (GUI.Button(new Rect(CloseButtonPosX, 0f, ButtonSize, ButtonSize), "x"))
            {
                gameObject.SetActive(false);
            }
            GUI.backgroundColor = colour;

            // X offset based on button count.
            static float GetSideOffset(int count)
            {
                return 10f + count * (10f + SideOffset);
            }
        }

        private Color GetColour(int count)
        {
            if (!UseColours)
            {
                return Color.white;
            }

            return Color.HSVToRGB((count * 0.31f) % 1.00f, 0.55f, 1.00f);
        }

        private void ResetData()
        {
            foreach (var port in _ports)
            {
                port.Reset();
            }
        }

        private void AddTrainsetPorts()
        {
            _dummyTrainsetForce = new Port("trainset", new PortDefinition(
                PortType.READONLY_OUT,
                PortValueType.FORCE,
                "FORCE"));

            _ports.Add(new PortData("TRAINSET_FORCE", _dummyTrainsetForce));

            _dummyTrainsetPower = new Port("trainset", new PortDefinition(
                PortType.READONLY_OUT,
                PortValueType.POWER,
                "POWER"));

            _ports.Add(new PortData("TRAINSET_POWER", _dummyTrainsetPower));
        }

        private void TryAddPort()
        {
            var flow = _car.SimController.SimulationFlow;
            var map = new Dictionary<string, Port>();

            foreach (var comp in flow.OrderedSimComps)
            {
                foreach (var port in comp.GetAllPorts())
                {
                    map[port.id] = port;
                }

                foreach (var portReference in comp.GetAllPortReferences())
                {
                    map[portReference.id] = portReference.GetPort();
                }
            }

            if (map.TryGetValue(_portToAdd, out var match))
            {
                _ports.Add(new PortData(_portToAdd, match));
            }
        }

        public static SimPortPlotterInternal? AddToCarId(string carId)
        {
            if (!CarSpawner.Instance.AllCars.TryFind(x => x.ID == carId, out var car))
            {
                Debug.LogWarning($"Could not find car with ID {carId}");
                return null;
            }

            return AddToCar(car);
        }

        public static SimPortPlotterInternal? AddToLastLoco()
        {
            var loco = PlayerManager.LastLoco;
            if (loco == null)
            {
                Debug.LogWarning($"Player has no last loco!");
                return null;
            }

            return AddToCar(loco);
        }

        private static SimPortPlotterInternal AddToCar(TrainCar car)
        {
            var comp = new GameObject("DATA").AddComponent<SimPortPlotterInternal>();
            comp.transform.parent = car.transform;
            comp.transform.Reset();

            return comp;
        }
    }
}
