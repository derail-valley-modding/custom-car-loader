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
    internal class CatenaryInteractionControllerInternal : ASimInitializedController
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

        private static readonly Dictionary<TrainCar, List<CatenaryInteractionControllerInternal>> _allCatenaryControllers = new();

        public Transform? pantographBase;
        public Transform? contactStripFirstEnd, contactStripSecondEnd;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string wireHeightPortId = string.Empty;

        private Func<Transform, Transform, Transform, Transform, float, (float?, float)>? GetWireHeightAndVoltage = null;
        private Port? _wireHeight = null;

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
                foreach (List<CatenaryInteractionControllerInternal> carCatenaryControllers in _allCatenaryControllers.Values)
                {
                    foreach (CatenaryInteractionControllerInternal controller in carCatenaryControllers)
                        controller.SetUpCatenaryConnection();
                }
            }
        }
        
        private static void SeverConnectionForAllControllers()
        {
            CCLPlugin.LogVerbose("Catenary deactivated, turning off overhead power");
            _OCSInstance = null;
            foreach (List<CatenaryInteractionControllerInternal> carCatenaryControllers in _allCatenaryControllers.Values)
            {
                foreach (CatenaryInteractionControllerInternal controller in carCatenaryControllers)
                { 
                    controller.GetWireHeightAndVoltage = null;
                    //controller._pantographToggle.Value = 0.0f;
                }
            }
        }

        private void SetUpCatenaryConnection()
        {
            GetWireHeightAndVoltage = null;
            if (_OCSInstance == null || _getWireHeightAndVoltageInfo == null)
                return;
            GetWireHeightAndVoltage = _getWireHeightAndVoltageInfo.CreateDelegate(typeof(Func<Transform, Transform, Transform, Transform, float, (float?, float)>), _OCSInstance)
                as Func<Transform, Transform, Transform, Transform, float, (float?, float)>;
            if (GetWireHeightAndVoltage == null)
                CCLPlugin.Error($"Unable to connect car {TrainCar.Resolve(gameObject).name} to OCS, pantograph will not receive power");
            else
                CCLPlugin.LogVerbose($"Connection to OCS successfully established for car {TrainCar.Resolve(gameObject).name}");
        }

        public override void Init(TrainCar car, SimulationFlow simFlow)
        {
            Debug.Log($"OCSI {car.ID}");
            if (!simFlow.TryGetPort(wireHeightPortId, out _wireHeight))
                Debug.Log("OCSI NP");
            else
                Debug.Log($"OCSI P {_wireHeight.Value}");
            TryGetOCSType();
            SetUpCatenaryConnection();
            Debug.Log($"OCSI {GetWireHeightAndVoltage?.ToString() ?? "<null>"}");
        }

        public override void Tick(float deltaTime)
        {
            Debug.Log("OCSI TK");
        }
    }
}
