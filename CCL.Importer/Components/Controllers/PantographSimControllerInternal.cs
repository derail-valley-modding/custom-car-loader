using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CCL.Importer.Implementations;
using CCL.Types.Proxies.Ports;

using DV.Simulation.Controllers;

using LocoSim.Implementations;

using UnityEngine;

using static UnityEngine.UI.CanvasScaler;

namespace CCL.Importer.Components.Controllers
{
    internal class PantographSimControllerInternal : ASimInitializedController
    {
        #region OCS interface

        const string OCSClassName = "electric_sim.catenary.overhead_equipment, electric_sim";
        const string OCSPropertyName = "system";
        const string OCSWireHeightAndVoltageMethodName = "relative_wire_height_and_voltage";
        const string OCSActivationEventName = "catenary_activated";
        const string OCSDeactivationEventName = "catenary_deactivated";

        private static Type? _OCSType = null;
        private static MethodInfo? _getWireHeightAndVoltageInfo = null;
        private static PropertyInfo? _OCSObjectInfo = null;
        private static object? _OCSInstance = null;

        #endregion

        private static readonly Dictionary<TrainCar, List<PantographSimControllerInternal>> _allCatenaryControllers = new();

        public Transform? pantographBase;
        public Transform? contactStripFirstEnd, contactStripSecondEnd;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string initialHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string headHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string wireHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, true)]
        public string wireVoltagePortId = string.Empty;
        [PortId(DVPortValueType.AMPS)]
        public string inputCurrentPortId = string.Empty;

        private Func<Transform, Transform, Transform, Transform, float, (float?, float)>? GetWireHeightAndVoltage = null;
        private TrainCar? _unit;
        private Port? _initialHeadHeight, _headHeight, _wireHeight, _wireVoltage, _inputCurrent;
        private Vector3 _lastTipPosition = new(0.0f, float.MinValue, 0.0f);
        private float _lastHeadMidpointHeight;

        public override bool ExternalTick => true;

        private static void TryGetOCSType()
        {
            if (_OCSType != null)
                return;
            _OCSType = Type.GetType(OCSClassName, false);
            if (_OCSType == null)
            {
                CCLPlugin.Log("Catenary not installed; overhead power will be unavailable");
                return;
            }
            _getWireHeightAndVoltageInfo = _OCSType.GetMethod(OCSWireHeightAndVoltageMethodName, 
                new Type[] { typeof(Transform), typeof(Transform), typeof(Transform), typeof(Transform), typeof(float) });
            _OCSObjectInfo = _OCSType.GetProperty(OCSPropertyName, BindingFlags.Public | BindingFlags.Static);
            EventInfo? OCSActivationInfo = _OCSType.GetEvent(OCSActivationEventName, BindingFlags.Public | BindingFlags.Static);
            EventInfo? OCSDeactivationInfo = _OCSType.GetEvent(OCSDeactivationEventName, BindingFlags.Public | BindingFlags.Static);
            if (_getWireHeightAndVoltageInfo == null || _OCSObjectInfo == null || OCSActivationInfo == null || OCSDeactivationInfo == null)
            {
                CCLPlugin.Error("Unable to retreive OCS class information; overhead power will be unavailable");
                _OCSType = null;
                return;
            }
            OCSActivationInfo.AddEventHandler(null, (Action) SetUpConnectionForAllControllers);
            OCSDeactivationInfo.AddEventHandler(null, (Action) SeverConnectionForAllControllers);
            SetUpConnectionForAllControllers();
        }

        private static void SetUpConnectionForAllControllers()
        {
            if (_OCSType != null && _OCSObjectInfo != null)
            {
                try
                {
                    _OCSInstance = _OCSObjectInfo.GetValue(null);
                }
                catch (InvalidOperationException _)
                {
                    CCLPlugin.Log("Catenary inactive, overhead power not available");
                    _OCSInstance = null;
                    return;
                }
                CCLPlugin.LogVerbose("Catenary activated, restoring overhead power access");
                foreach (List<PantographSimControllerInternal> carCatenaryControllers in _allCatenaryControllers.Values)
                {
                    foreach (PantographSimControllerInternal controller in carCatenaryControllers)
                        controller.SetUpCatenaryConnection();
                }
            }
        }
        
        private static void SeverConnectionForAllControllers()
        {
            CCLPlugin.LogVerbose("Catenary deactivated, turning off overhead power");
            _OCSInstance = null;
            foreach (List<PantographSimControllerInternal> carCatenaryControllers in _allCatenaryControllers.Values)
            {
                foreach (PantographSimControllerInternal controller in carCatenaryControllers)
                    controller.DisablePower();
            }
        }

        private void DisablePower()
        {
            GetWireHeightAndVoltage = null;
            _wireHeight!.Value = -1.0f;
            _wireVoltage!.Value = 0.0f;
        }
        
        private void SetUpCatenaryConnection()
        {
            if (_OCSInstance == null || _getWireHeightAndVoltageInfo == null)
            { 
                DisablePower();
                return; 
            }
            GetWireHeightAndVoltage = _getWireHeightAndVoltageInfo.CreateDelegate(typeof(Func<Transform, Transform, Transform, Transform, float, (float?, float)>), _OCSInstance)
                as Func<Transform, Transform, Transform, Transform, float, (float?, float)>;
            if (GetWireHeightAndVoltage != null)
                CCLPlugin.LogVerbose($"Connection to OCS successfully established for car {_unit!.name}");
            else
            { 
                Debug.LogError($"Unable to connect car {_unit!.name} to OCS, pantograph will not receive power", this); 
                DisablePower();
            }
        }

        public override void Init(TrainCar car, SimulationFlow simFlow)
        {
            if (pantographBase == null || contactStripFirstEnd == null || contactStripSecondEnd == null)
            {
                Debug.LogError($"Pantograph control transforms not set, pantograph sim controller disabled", this);
                Destroy(this);
                return;
            }
            if (!simFlow.TryGetPort(initialHeightPortId, out _initialHeadHeight) ||
                !simFlow.TryGetPort(headHeightPortId, out _headHeight) ||
                !simFlow.TryGetPort(wireHeightPortId, out _wireHeight) ||
                !simFlow.TryGetPort(wireVoltagePortId, out _wireVoltage) ||
                !simFlow.TryGetPort(inputCurrentPortId, out _inputCurrent))
            { 
                Debug.LogError($"Referenced ports not set, pantograph sim controller disabled", this);
                Destroy(this);
                return;
            }
            _unit = TrainCar.Resolve(gameObject);
            if (_unit == null)
            { 
                Debug.LogError($"Car unresolved, pantograph sim controller disabled", this);
                Destroy(this);
                return;
            }

            TryGetOCSType();
            SetUpCatenaryConnection();
            (_initialHeadHeight.Value, _) = GetHeadMidpointHeight();
        }

        private (float height, bool positionChanged) GetHeadMidpointHeight()
        {
            Vector3 currentTipPosition = contactStripFirstEnd!.position;
            Vector3 positionDifference = currentTipPosition - _lastTipPosition;
            bool positionChanged;
            if (Math.Abs(positionDifference.y) < 0.003f && Math.Abs(positionDifference.x) + Math.Abs(positionDifference.z) < 0.1f)
                positionChanged = false;
            else
            {
                positionChanged = true;
                _lastTipPosition = currentTipPosition;
                _lastHeadMidpointHeight = _unit!.transform.InverseTransformPoint((currentTipPosition + contactStripSecondEnd!.position) / 2.0f).y;
            }
            return (_lastHeadMidpointHeight, positionChanged);
        }

        public override void Tick(float deltaTime)
        {
            if (GetWireHeightAndVoltage == null)
            { 
                return; 
            }

            float inputCurrent = _inputCurrent!.Value;
            if (float.IsNaN(inputCurrent) || float.IsInfinity(inputCurrent))
            {
                inputCurrent = 0.0f;
            }
            bool headPositionChanged;
            (_headHeight!.Value, headPositionChanged) = GetHeadMidpointHeight();
            if (Mathf.Abs(inputCurrent) > 0.1f || headPositionChanged)
            {
                float? wireHeight;
                (wireHeight, _wireVoltage!.Value) = GetWireHeightAndVoltage(_unit!.transform, pantographBase!, contactStripFirstEnd!, contactStripSecondEnd!, inputCurrent);
                _wireHeight!.Value = wireHeight ?? -1.0f;
            }
        }

        private void OnDestroy()
        {
            if (_unit is not null && _allCatenaryControllers.ContainsKey(_unit))
            {
                foreach (PantographSimControllerInternal controller in _allCatenaryControllers[_unit])
                    controller.DisablePower();
                _allCatenaryControllers.Remove(_unit);
            }
        }
    }
}
