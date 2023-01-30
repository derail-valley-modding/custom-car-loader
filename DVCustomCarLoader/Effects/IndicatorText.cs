using CCL_GameScripts.Attributes;
using CCL_GameScripts.CabControls;
using DVCustomCarLoader.LocoComponents;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DVCustomCarLoader.Effects
{
    public class IndicatorText : MonoBehaviour, ILocoEventAcceptor, ICabControlAcceptor
    {
        public TextMeshPro Display;
        public float RefreshDelay;
        public string FormatString;

        public OutputBinding[] Bindings;
        public SimEventType[] EventTypes => _eventTypes;
        [SerializeField]
        protected SimEventType[] _eventTypes;

        public CabInputType[] ControlTypes;

        private object[] _values;
        private Coroutine _updateRoutine;
        private bool _initialized = false;

        public void Start()
        {
            if (!Display)
            {
                Main.Warning("Missing text mesh from text indicator, disabling self");
                Destroy(this);
                return;
            }

            if (_eventTypes == null) _eventTypes = new SimEventType[0];
            if (ControlTypes == null) ControlTypes = new CabInputType[0];

            Bindings = _eventTypes
                .Select(b => new OutputBinding { SimEventType = b })
                .Concat(ControlTypes.Select(b => new OutputBinding { CabInputType = b }))
                .ToArray();

            _values = new object[Bindings.Length];

            _initialized = true;

            if (_updateRoutine == null)
            {
                _updateRoutine = StartCoroutine(UpdateRoutine());
            }
        }

        public void OnEnable()
        {
            if (_initialized && (_updateRoutine == null))
            {
                _updateRoutine = StartCoroutine(UpdateRoutine());
            }
        }

        public void OnDisable()
        {
            StopAllCoroutines();
            _updateRoutine = null;
        }

        private IEnumerator UpdateRoutine()
        {
            while (true)
            {
                yield return WaitFor.Seconds(RefreshDelay);

                Display.text = string.Format(FormatString, _values);
            }
        }

        private void SetValue(SimEventType binding, float value)
        {
            for (int i = 0; i < Bindings.Length; i++)
            {
                if (binding == Bindings[i].SimEventType)
                {
                    _values[i] = value;
                }
            }
        }

        private void SetValue(CabInputType binding, float value)
        {
            for (int i = 0; i < Bindings.Length; i++)
            {
                if (binding == Bindings[i].CabInputType)
                {
                    _values[i] = value;
                }
            }
        }

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is float newVal)
            {
                SetValue(eventInfo.EventType, newVal);
            }
        }

        private void HandleControlInput(object sender, float newValue)
        {
            var relay = (CabInputRelay)sender;
            SetValue(relay.Binding, newValue);
        }

        public bool AcceptsControlOfType(CabInputType inputType)
        {
            return ControlTypes.Contains(inputType);
        }

        public void RegisterControl(CabInputRelay controlRelay)
        {
            controlRelay.AddListener(HandleControlInput);
        }
    }
}