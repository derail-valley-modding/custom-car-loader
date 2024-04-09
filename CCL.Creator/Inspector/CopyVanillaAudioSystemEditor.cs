using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(CopyVanillaAudioSystem))]
    internal class CopyVanillaAudioSystemEditor : Editor
    {
        private CopyVanillaAudioSystem _comp = null!;
        private SerializedProperty _audioSystem = null!;
        private SerializedProperty _port1 = null!;
        private SerializedProperty _port2 = null!;
        private string? _portName;

        private void OnEnable()
        {
            _audioSystem = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.AudioSystem));
            _port1 = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.PortId1));
            _port2 = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.PortId2));
        }

        public override void OnInspectorGUI()
        {
            _comp = (CopyVanillaAudioSystem)target;

            EditorGUILayout.PropertyField(_audioSystem);

            if (!string.IsNullOrEmpty(_portName = GetPort1Name((VanillaAudioSystem)_audioSystem.intValue)))
            {
                EditorGUILayout.PropertyField(_port1, new GUIContent(_portName));
            }

            if (!string.IsNullOrEmpty(_portName = GetPort2Name((VanillaAudioSystem)_audioSystem.intValue)))
            {
                EditorGUILayout.PropertyField(_port2, new GUIContent(_portName));
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static string? GetPort1Name(VanillaAudioSystem system)
        {
            switch (system)
            {
                case VanillaAudioSystem.TMOverspeed:
                    return "TM Overspeed Port ID";

                case VanillaAudioSystem.DE2EnginePiston:
                case VanillaAudioSystem.DE6EngineThrottling:
                    return "Fuel Consumption Port ID";
                case VanillaAudioSystem.DE2ElectricMotor:
                case VanillaAudioSystem.DE6ElectricMotor:
                    return "Amps Per TM Port ID";

                case VanillaAudioSystem.SteamerChestAdmission:
                    return "Exhaust Pressure Port ID";
                case VanillaAudioSystem.SteamerValveGearDamaged:
                    return "Wheel RPM Port ID";
                case VanillaAudioSystem.SteamerCrownSheet:
                    return "Crown Sheet Temperature Port ID";

                case VanillaAudioSystem.BE2ElectricMotor:
                    return "Total Amps Port ID";

                default:
                    return "Port ID";
            }
        }

        private static string? GetPort2Name(VanillaAudioSystem system)
        {
            switch (system)
            {
                case VanillaAudioSystem.TMOverspeed:
                case VanillaAudioSystem.DE2ElectricMotor:
                case VanillaAudioSystem.DE6ElectricMotor:
                case VanillaAudioSystem.BE2ElectricMotor:
                    return "TM Normalized RPM Port ID";
                case VanillaAudioSystem.DE2EnginePiston:
                case VanillaAudioSystem.DE6EngineThrottling:
                    return "Engine RPM Port ID";

                case VanillaAudioSystem.SteamerChestAdmission:
                    return "Steam Injection Port ID";
                case VanillaAudioSystem.SteamerValveGearDamaged:
                    return "Lubrication Port ID";
                case VanillaAudioSystem.SteamerCrownSheet:
                    return "Boiler Broken Port ID";

                default:
                    return null;
            }
        }
    }
}
