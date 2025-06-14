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
        private const float WindowWidth = 1000.0f;
        private const float WindowHeight = 740.0f;
        private const float WindowBorder = 10.0f;
        // Scroll area size.
        private const float ScrollYPosition = 40.0f;
        private const float ScrollWidth = WindowWidth - 2 * WindowBorder;
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
        private const float LowerButtonsPosY = WindowHeight - WindowBorder - 2 * LabelHeight;
        private const float LowerButtonsPosY2 = WindowHeight - WindowBorder - LabelHeight;
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

            private void SetStartingValues()
            {
                Min = Mathf.Min(0, Port.Value);
                Max = Mathf.Max(0, Port.Value);
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

            public void Draw(Rect box, Color c, Rect view)
            {
                if (Values.Count < 2) return;

                //GUI.color = GetColourForPortValueType(Port.valueType);
                Vector2 pos1;
                Vector2 pos2;

                // If there's no difference to draw yet, draw a single line with the correct length.
                if (Min == Max)
                {
                    pos1 = ToGLPosition(0, box.height - 2);
                    pos2 = ToGLPosition(Offset * Values.Count, box.height - 2);

                    // Only draw if the line is within the view rectangle.
                    if (WithinViewBounds(pos1, view, out _) | WithinViewBounds(pos2, view, out _))
                    {
                        GLHelper.StartPixelLineStrip(c);
                        GL.Vertex(pos1);
                        GL.Vertex(pos2);
                        GLHelper.EndAndPop();
                    }

                    return;
                }

                // Draw lines and not line strips so they can be cut for clipping.
                GLHelper.StartPixelLines(c);
                Vector2 prev = Vector2.zero;
                float x = 0;
                bool init = false;

                foreach (var item in Values)
                {
                    var mapped = GetMapped(item);

                    // Setup the first previous value to be able to draw lines.
                    if (!init)
                    {
                        prev = ToGLPosition(x, mapped);
                        init = true;
                        continue;
                    }

                    var current = ToGLPosition(x, mapped);

                    // Only draw if at least one position is within the view rectangle.
                    // If both are out, don't draw, but if 1 is in we clip the line.
                    if (WithinViewBounds(prev, view, out pos1) | WithinViewBounds(current, view, out pos2))
                    {
                        GL.Vertex(pos1);
                        GL.Vertex(pos2);
                    }

                    // Advance offset and move previous.
                    x += Offset;
                    prev = current;
                }

                GLHelper.EndAndPop();

                float GetMapped(float value)
                {
                    // Slight offset from 0-height to keep within the box outlines.
                    return Extensions.Mapf(Max, Min, 2, box.height - 2, value);
                }

                Vector2 ToPosition(float x, float y)
                {
                    // Offsets to the box's origin.
                    return new Vector2(x + box.xMin, y + box.yMin);
                }

                Vector3 ToGLPosition(float x, float y)
                {
                    return GLHelper.GUIToGLPosition(GUIUtility.GUIToScreenPoint(ToPosition(x, y)));
                }

                bool WithinViewBounds(Vector2 position, Rect view, out Vector2 transformed)
                {
                    // Check if the position is within the view rectangle.
                    var contains = view.Contains(position);

                    // If it is not, clamp the position to the view.
                    transformed = contains ? position : new Vector2(
                        Mathf.Clamp(position.x, view.xMin + 1, view.xMax - 1),
                        Mathf.Clamp(position.y, view.yMin + 1, view.yMax - 1));

                    return contains;
                }
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
        }

        private static List<SimPortPlotterInternal> s_windows = new();
        private static GUIStyle? s_graphNumStyle = null;
        private static GUIStyle? s_portIdStyle = null;
        private static GUIStyle? s_windowButtons = null;
        private static bool s_round = false;

        private static GUIStyle GraphNumberStyle => Extensions.GetCached(ref s_graphNumStyle, () => new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        });
        private static GUIStyle PortIdStyle => Extensions.GetCached(ref s_portIdStyle, () => new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
        });
        private static GUIStyle WindowButtonsStyle => Extensions.GetCached(ref s_windowButtons, () => new GUIStyle(GUI.skin.button)
        {
            fontSize = 10,
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

            foreach (var loco in _car.trainset.GetAllLocos())
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
            // The view rect is used to clip the graphs.
            // Has to be created before the scrollview itself or else GUIToScreenRect will account for scrolling.
            var view = GLHelper.GUIToGLView(GUIUtility.GUIToScreenRect(
                new Rect(SideOffset / 2f, ScrollYPosition, BoxWidth + SideOffset, ScrollHeight)));
            _scroll = GUI.BeginScrollView(
                new Rect(0f, ScrollYPosition, ScrollWidth, ScrollHeight), _scroll,
                new Rect(0f, ScrollYPosition, BoxWidth, _ports.Count * BoxTotalHeight), false, true);
            GUILayout.BeginVertical();

            PortData port;
            int count = _ports.Count;

            for (int i = 0; i < count && (i * BoxTotalHeight <= ScrollHeight + _scroll.y); i++)
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
                    GUI.Label(new Rect(0f, posY + zeroOffset, LabelWidth, LabelHeight), "0", GraphNumberStyle);
                }

                // Draw max and min value labels.
                GUI.Label(new Rect(0f, posY, LabelWidth, LabelHeight), GetFormattedString(port.Max), GraphNumberStyle);
                GUI.Label(new Rect(0f, posY + BoxHeight, LabelWidth, LabelHeight), GetFormattedString(port.Min), GraphNumberStyle);

                posY += 10f;

                // Draw graph box.
                Rect box = new(SideOffset, posY, BoxWidth, BoxHeight);
                GUI.Box(box, GUIContent.none);
                port.Draw(box, GetColour(i), view);
                GUI.BeginClip(box);
                GUI.Label(new Rect(10f, 0f, BoxWidth, LabelHeight), $"{port.Id}: {GetFormattedString(port.Last, port.Unit)}", PortIdStyle);
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
        }

        private static string GetFormattedString(float value, string? unit = null)
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
                if (value >= 1000000000 || value <= -1000000000)
                {
                    return $"{value:0,,,.## G}{unit}";
                }

                if (value >= 1000000 || value <= -1000000)
                {
                    return $"{value:0,,.## M}{unit}";
                }

                if (value >= 10000 || value <= -10000)
                {
                    return $"{value:0,.## k}{unit}";
                }
            }

            // 3 decimal spaces looks ugly, so only keep them for very small values.
            return (value < 10 || value > -10) ? $"{value:0.###} {unit}" : $"{value:0.##} {unit}";
        }

        private void DrawButtons()
        {
            // Button to minimise (hide window).
            if (GUI.Button(new Rect(CloseButtonPosX - ButtonSize, 0f, ButtonSize, ButtonSize), "-", WindowButtonsStyle))
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
            using (new GUIColorScope(newBackground: Color.red))
            {
                // Button to reset port data.
                if (GUI.Button(new Rect(GetSideOffset(8), LowerButtonsPosY, SideOffset, ButtonSize), "Clear data"))
                {
                    ResetData();
                }

                // Button to close (disable GO).
                if (GUI.Button(new Rect(CloseButtonPosX, 0f, ButtonSize, ButtonSize), "x", WindowButtonsStyle))
                {
                    gameObject.SetActive(false);
                }
            }

            // 2nd row with additional info.
            var sim = _car.SimController;

            if (sim != null && sim.wheelslipController != null)
            {
                // Wheelslip is happening or not.
                GUI.Toggle(new(GetSideOffset(0), LowerButtonsPosY2, SideOffset, 20f), sim.wheelslipController.IsWheelslipping, "Wheelslipping");

                // Adhesion limit.
                GUI.Label(new(GetSideOffset(1), LowerButtonsPosY2, SideOffset * 2, 20f),
                    $"Adhesion Limit: {GetFormattedString(sim.wheelslipController.TotalForceLimit, GetUnitForValueType(PortValueType.FORCE))}");
            }

            if (_car.stress != null)
            {
                // Derail buildup normalized.
                bool canDerail = _car.GetVelocity().magnitude * 3.6f > _car.stress.gameParams.DerailMinVelocity;
                float chance = canDerail ? _car.stress.derailBuildUp / _car.stress.gameParams.derailBuildUpThreshold : 0;

                GUI.Label(new(GetSideOffset(3), LowerButtonsPosY2, SideOffset - 40f, 20f), "Derail:");

                // Red text for dangerous values.
                using (new GUIColorScope(newContent: DerailDangerColour(chance)))
                {
                    GUI.Label(new(GetSideOffset(3) + 40f, LowerButtonsPosY2, SideOffset - 40f, 20f), GetFormattedString(chance));
                }
            }

            // X offset based on button count.
            static float GetSideOffset(int count)
            {
                return WindowBorder + count * (10f + SideOffset);
            }

            static Color DerailDangerColour(float chance)
            {
                // Start changing colour at 75%, maximum red at 95%.
                return Color.Lerp(Color.white, Color.red, Extensions.Mapf(0.75f, 0.95f, 0.0f, 1.0f, chance));
            }
        }

        private Color GetColour(int count)
        {
            if (!UseColours)
            {
                return Color.white;
            }

            // Offsets colours a bit without looping perfectly for "random" colours as you go through ports.
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
            if (_dummyTrainsetForce != null)
            {
                _ports.RemoveAll(x => x.Port == _dummyTrainsetForce);
                _dummyTrainsetForce = null;
            }

            if (_dummyTrainsetPower != null)
            {
                _ports.RemoveAll(x => x.Port == _dummyTrainsetPower);
                _dummyTrainsetPower = null;
            }

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

        /// <summary>
        /// Gets or adds a <see cref="SimPortPlotterInternal"/> to a car with matching <paramref name="carId"/>.
        /// </summary>
        /// <param name="carId">The ID to match.</param>
        /// <returns>A <see cref="SimPortPlotterInternal"/> if successful, or <see langword="null"/> if not.</returns>
        public static SimPortPlotterInternal? GetOrAddToCarId(string carId)
        {
            if (!CarSpawner.Instance.TryGetTraincar(carId, out var car))
            {
                Debug.LogWarning($"Could not find car with ID {carId}");
                return null;
            }

            return GetOrAddToCar(car);
        }

        /// <summary>
        /// Gets or adds a <see cref="SimPortPlotterInternal"/> to the last locomotive used by the player.
        /// </summary>
        /// <returns>A <see cref="SimPortPlotterInternal"/> if successful, or <see langword="null"/> if not.</returns>
        public static SimPortPlotterInternal? GetOrAddToLastLoco()
        {
            var loco = PlayerManager.LastLoco;
            if (loco == null)
            {
                Debug.LogWarning($"Player has no last loco!");
                return null;
            }

            return GetOrAddToCar(loco);
        }

        private static SimPortPlotterInternal GetOrAddToCar(TrainCar car)
        {
            var comp = car.GetComponentInChildren<SimPortPlotterInternal>(true);

            if (comp != null)
            {
                comp.gameObject.SetActive(true);
                return comp;
            }

            comp = new GameObject("DATA").AddComponent<SimPortPlotterInternal>();
            comp.transform.parent = car.transform;
            comp.transform.Reset();

            return comp;
        }
    }
}
