using CCL.Types;
using LocoSim.Implementations;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class SimPortPlotterInternal : MonoBehaviour
    {
        private const string FormatString = "F3";
        private const int WindowId = 98;
        // Window size.
        private const float WindowWidth = 1020.0f;
        private const float WindowHeight = 700.0f;
        // Scroll area size.
        private const float ScrollWidth = 1000.0f;
        private const float ScrollHeight = 620.0f;
        // Box size.
        private const float BoxWidth = 800.0f;
        private const float BoxHeight = 150.0f;
        private const float BoxSpacing = 20.0f;
        private const float BoxTotalHeight = BoxHeight + BoxSpacing;
        // Label size.
        private const float LabelWidth= 80.0f;
        private const float LabelHeight = 20.0f;
        // Misc.
        private const float SideOffset = 100.0f;
        private const float ButtonSize = 17.5f;

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

            public PortData(string id, Port port)
            {
                Id = id;
                Port = port;
                Values = new Queue<float>(DataLength);
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

                if (Values.Count > DataLength)
                {
                    Values.Dequeue();
                }

                ZeroOffset = Mathf.InverseLerp(Min, Max, 0);
            }

            public void Draw(Rect bounds)
            {
                // If there's no difference to draw yet, draw a single line with the correct length.
                if (Min == Max)
                {
                    GUIHelper.DrawLine(ToPosition(0, bounds.height), ToPosition(Offset * Values.Count, bounds.height), 1.0f);
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
                        GUIHelper.DrawLine(prev.Value, current, 1.0f);
                    }

                    prev = current;
                }

                float GetMapped(float value)
                {
                    return Extensions.Mapf(Max, Min, 0, bounds.height, value);
                }

                Vector2 ToPosition(float x, float y)
                {
                    return new Vector2(x + bounds.xMin, y + bounds.yMin);
                }
            }

            private void SetStartingValues()
            {
                Min = Mathf.Min(0, Port.Value);
                Max = Mathf.Max(0, Port.Value);
            }
        }

        public int TickRate = 5;
        public List<string> PortIds = new();
        public List<string> PortReferenceIds = new();
        public bool UseColours = true;

        private List<PortData> _ports = new();
        private int _ticking = 0;
        private bool _record = false;
        private bool _display = true;
        private GUIStyle _style = null!;
        private Rect _windowRect = new Rect(10f, 0f, WindowWidth, WindowHeight);
        private Vector2 _scroll;

        private void Start()
        {
            var car = TrainCar.Resolve(gameObject);

            if (car == null)
            {
                Debug.LogError("Could not find TrainCar for SimPortPlotter");
                return;
            }

            var controller = car.SimController;

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
                    _ports.Add(new PortData(id, port));
                }
            }

            flow.TickEvent += OnTick;
        }

        private void OnTick()
        {
            if (!_record) return;
            if (_ticking-- > 0) return;

            foreach (var port in _ports)
            {
                port.Update();
            }

            _ticking = TickRate;
        }

        private void OnGUI()
        {
            if (_display)
            {
                _windowRect = GUILayout.Window(WindowId, _windowRect, Window, "Loco simulation graph");
                return;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Graph"))
            {
                _display = !_display;
            }

            GUILayout.EndVertical();
        }

        private void Window(int id)
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            // Record data box.
            Rect recordDataBox = new(10f, 0f, SideOffset, 20f);
            _record = GUI.Toggle(recordDataBox, _record, "Record data");

            if (GUI.Button(new Rect(20f + SideOffset, 0f, SideOffset, ButtonSize), "Clear data"))
            {
                ResetData();
            }

            if (GUI.Button(new Rect(1001.25f, 0f, ButtonSize, ButtonSize), "x"))
            {
                _display = !_display;
            }

            GUI.skin.label.fontSize = 16;
            Rect scrollArea = new(0f, 40f, ScrollWidth, ScrollHeight);
            _scroll = GUI.BeginScrollView(scrollArea, _scroll, new Rect(0f, 40f, BoxWidth, _ports.Count * BoxTotalHeight), false, true);
            GUILayout.BeginVertical();

            int count = _ports.Count;
            PortData port;
            for (int i = 0; i < count && !((i + 1) * BoxTotalHeight > ScrollHeight + _scroll.y); i++)
            {
                if ((i + 1) * BoxTotalHeight < _scroll.y) continue;

                port = _ports[i];

                if (port.Values.Count == 0) continue;

                float posY = 40f + i * BoxTotalHeight;
                float zeroOffset = BoxHeight * (1.0f - port.ZeroOffset);

                if (zeroOffset > 0 && zeroOffset < BoxHeight)
                {
                    GUI.Label(new Rect(0f, posY + zeroOffset, LabelWidth, LabelHeight), "0", _style);
                }

                GUI.Label(new Rect(0f, posY, LabelWidth, LabelHeight), port.Max.ToString(FormatString), _style);
                GUI.Label(new Rect(0f, posY + BoxHeight, LabelWidth, LabelHeight), port.Min.ToString(FormatString), _style);

                Rect box = new(SideOffset, posY + 10.0f, BoxWidth, BoxHeight);
                GUI.Box(box, GUIContent.none);
                GUI.color = GetColour(i);
                port.Draw(box);
                GUI.color = Color.white;
                GUI.BeginClip(box);
                GUI.Label(new Rect(10f, 0f, BoxWidth, LabelHeight), $"{port.Id}: {port.Last:F3}");
                GUI.EndClip();
            }

            GUILayout.EndVertical();
            GUI.EndScrollView();
        }

        private Color GetColour(int count)
        {
            if (!UseColours)
            {
                return Color.white;
            }

            return Color.HSVToRGB(count * 0.31f, 0.55f, 1.00f);
        }

        private void ResetData()
        {
            foreach (var port in _ports)
            {
                port.Reset();
            }
        }
    }
}
