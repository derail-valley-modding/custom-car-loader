using CCL.Types.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Multiple Port Decoder \u2215 Encoder Definition Proxy")]
    public class MultiplePortDecoderEncoderDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized,
        IDM1UDefaults, IS060Defaults, IS282Defaults, IBE2Defaults
    {
        [Serializable]
        public class FloatArray
        {
            public float this[int i]
            {
                get => array[i];
                set => array[i] = value;
            }

            public float[] array;

            public FloatArray()
            {
                array = Array.Empty<float>();
            }

            public FloatArray(int length)
            {
                array = new float[length];
            }

            public FloatArray(float[] f)
            {
                array = f;
            }
        }

        public int combinations;
        public float defaultOutputValue;
        public bool useDefaultValueOnMatchNotFound = true;
        public bool matchClosestOutputValue;
        public FloatArray[] values = Array.Empty<FloatArray>();
        public PortDefinition[] inputPorts = Array.Empty<PortDefinition>();
        public PortDefinition outputPort = new PortDefinition();
        public bool saveState;

        [SerializeField, RenderMethodButtons]
        [MethodButton(nameof(ResizeArrays), "Resize Arrays", "Resizes arrays to have the required length to match the ports")]
        [MethodButton(nameof(SetupForSteamerSingleHeadlightControl), "Setup For Single Steamer Headlight Control")]
        private bool _renderButton;

        [SerializeField, HideInInspector]
        private string? _inPorts;
        [SerializeField, HideInInspector]
        private string? _outPort;
        [SerializeField, HideInInspector]
        private int[]? _lenghts;
        [SerializeField, HideInInspector]
        private float[]? _values;

        public override IEnumerable<PortDefinition> ExposedPorts
        {
            get
            {
                foreach (var port in inputPorts) yield return port;
                yield return outputPort;
            }
        }

        private void ResizeArrays()
        {
            int length = inputPorts.Length + 1;

            foreach (var item in values)
            {
                Array.Resize(ref item.array, length);
            }
        }

        private void SetupForSteamerSingleHeadlightControl()
        {
            values = FromMulti(new[,]
            {
                { 0 / 5f, 5 / 5f, 0 / 6f },
                { 0 / 5f, 4 / 5f, 1 / 6f },
                { 1 / 5f, 3 / 5f, 2 / 6f },
                { 2 / 5f, 2 / 5f, 3 / 6f },
                { 3 / 5f, 1 / 5f, 4 / 6f },
                { 4 / 5f, 0 / 5f, 5 / 6f },
                { 5 / 5f, 0 / 5f, 6 / 6f },

                { 2 / 5f, 5 / 5f, 0 / 6f },
                { 2 / 5f, 4 / 5f, 1 / 6f },
                { 2 / 5f, 3 / 5f, 2 / 6f },
                { 2 / 5f, 1 / 5f, 3 / 6f },
                { 2 / 5f, 0 / 5f, 3 / 6f },

                { 5 / 5f, 2 / 5f, 6 / 6f },
                { 4 / 5f, 2 / 5f, 5 / 6f },
                { 3 / 5f, 2 / 5f, 4 / 6f },
                { 1 / 5f, 2 / 5f, 3 / 6f },
                { 0 / 5f, 2 / 5f, 3 / 6f },

                { 5 / 5f, 5 / 5f, 3 / 6f },
                { 0 / 5f, 0 / 5f, 3 / 6f }
            });
        }

        private void SetupPortsForHeadlightControl()
        {
            inputPorts = new[]
            {
                new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "FRONT_HEADLIGHTS_EXT_IN"),
                new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "REAR_HEADLIGHTS_EXT_IN")
            };
            outputPort = new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "HEADLIGHTS_EXT_IN");
        }

        private static FloatArray[] FromMulti(float[,] array)
        {
            int length = array.GetLength(1);
            int count = array.GetLength(0);

            FloatArray[] result = new FloatArray[count];

            for (int c = 0; c < count; c++)
            {
                var entry = new FloatArray(length);

                for (int i = 0; i < length; i++)
                {
                    entry[i] = array[c, i];
                }

                result[c] = entry;
            }

            return result;
        }

        private static FloatArray[] FromJagged(float[][] array)
        {
            int count = array.Length;

            FloatArray[] result = new FloatArray[count];

            for (int c = 0; c < count; c++)
            {
                result[c] = new FloatArray(array[c]);
            }

            return result;
        }

        public void OnValidate()
        {
            _inPorts = JSONObject.ToJson(inputPorts);
            _outPort = JSONObject.ToJson(outputPort);

            List<float> floats = new List<float>();
            List<int> lenghts = new List<int>();

            foreach (var item in values)
            {
                floats.AddRange(item.array);
                lenghts.Add(item.array.Length);
            }

            _values = floats.ToArray();
            _lenghts = lenghts.ToArray();
        }

        public void AfterImport()
        {
            inputPorts = JSONObject.FromJson(_inPorts, () => new PortDefinition[0]);
            outputPort = JSONObject.FromJson(_outPort, () => new PortDefinition());

            if (_values == null || _lenghts == null) return;

            int index = 0;
            values = new FloatArray[_lenghts.Length];

            for (int i = 0; i < _lenghts.Length; i++)
            {
                int length = _lenghts[i];
                var array = new FloatArray(length);

                Array.Copy(_values, index, array.array, 0, length);

                index += length;
                values[i] = array;
            }
        }

        #region Defaults

        public void ApplyDM1UDefaults()
        {
            values = FromMulti(new[,]
            {
                { 0, 0, 0 },
                { 1, 0, 0.5f },
                { 1, 1, 1 },
            });

            SetupPortsForHeadlightControl();
            inputPorts[0].ID = "DASH_LIGHT_CONTROL";
            inputPorts[1].ID = "CAB_LIGHT_CONTROL";
            outputPort.ID = "CAB_LIGHT_CONTROL_EXT_IN";
        }

        public void ApplyS060Defaults()
        {
            SetupForSteamerSingleHeadlightControl();
            SetupPortsForHeadlightControl();
        }

        public void ApplyS282Defaults()
        {
            SetupForSteamerSingleHeadlightControl();
            SetupPortsForHeadlightControl();
        }

        public void ApplyBE2Defaults()
        {
            values = FromMulti(new[,]
            {
                { 0, 0, 0 / 3f },
                { 1, 0, 1 / 3f },
                { 0, 1, 2 / 3f },
                { 1, 1, 3 / 3f },
            });

            SetupPortsForHeadlightControl();
        }

        #endregion
    }
}
