using CCL.Creator.Utility;
using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(CopyVanillaAudioSystem))]
    internal class CopyVanillaAudioSystemEditor : Editor
    {
        private SerializedProperty _audioSystem = null!;
        private SerializedProperty _port1 = null!;
        private SerializedProperty _port2 = null!;
        private SerializedProperty _positions = null!;
        private SerializedProperty _clips = null!;

        private void OnEnable()
        {
            _audioSystem = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.AudioSystem));
            _port1 = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.PortId1));
            _port2 = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.PortId2));
            _positions = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.SourcePositions));
            _clips = serializedObject.FindProperty(nameof(CopyVanillaAudioSystem.Clips));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_audioSystem);
            var system = (VanillaAudioSystem)_audioSystem.intValue;

            var ports = GetPortNames(system);

            if (ports.Length > 0)
            {
                EditorHelpers.DrawHeader("Ports",
                    "The port connections that control the Layered Audio");

                EditorGUILayout.PropertyField(_port1, new GUIContent(ports[0]));
            }

            if (ports.Length > 1)
            {
                EditorGUILayout.PropertyField(_port2, new GUIContent(ports[1]));
            }

            var positions = GetSourcePositionNames(system);
            int length = positions.Length;
            _positions.arraySize = length;

            if (length > 0)
            {
                EditorHelpers.DrawHeader("Layer Audio Source Positions",
                    "Optional transforms to move the audio source of each layer\n" +
                    "Leave empty to use the position of this object");

                for (int i = 0; i < length; i++)
                {
                    var label = new GUIContent(positions[i]);
                    EditorGUILayout.PropertyField(_positions.GetArrayElementAtIndex(i), label);
                }
            }

            var clips = GetClipNames(system);
            length = clips.Length;
            _clips.arraySize = length;

            if (length > 0)
            {
                EditorHelpers.DrawHeader("Layer Audio Source Clips",
                    "Optional audio clips to replace the ones already in the system\n" +
                    "Leave empty to keep the original audio");

                for (int i = 0; i < length; i++)
                {
                    var label = new GUIContent(clips[i]);
                    EditorGUILayout.PropertyField(_clips.GetArrayElementAtIndex(i), label);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static string[] GetPortNames(VanillaAudioSystem system)
        {
            switch (system)
            {
                case VanillaAudioSystem.TMOverspeed:
                    return new[] { "TM Overspeed Port ID", "TM RPM Port ID" };

                case VanillaAudioSystem.DE2EnginePiston:
                case VanillaAudioSystem.DE6EngineThrottling:
                case VanillaAudioSystem.DH4EnginePiston:
                case VanillaAudioSystem.DM3EnginePiston:
                    return new[] { "Fuel Consumption Port ID", "Engine RPM Port ID" };
                case VanillaAudioSystem.DE2ElectricMotor:
                case VanillaAudioSystem.DE6ElectricMotor:
                    return new[] { "Amps Per TM Port ID", "TM RPM Port ID" };

                case VanillaAudioSystem.DH4FluidCoupler:
                    return new[] { "Pump Torque Port ID", "Turbine RPM Port ID" };
                case VanillaAudioSystem.DH4HydroDynamicBrake:
                    return new[] { "Hydro Dynamic Brake Port ID", "Turbine RPM Port ID" };
                case VanillaAudioSystem.DH4TransmissionEngaged:
                case VanillaAudioSystem.DM3TransmissionEngaged:
                case VanillaAudioSystem.DM3JakeBrake:
                    return new[] { "Transmission Engaged Port ID", "Wheel Speed Port ID" };

                case VanillaAudioSystem.SteamerChestAdmission:
                    return new[] { "Steam Injection Port ID", "Exhaust Pressure Port ID" };
                case VanillaAudioSystem.SteamerValveGearDamaged:
                    return new[] { "Lubrication Port ID", "Wheel RPM Port ID" };
                case VanillaAudioSystem.SteamerCrownSheet:
                    return new[] { "Crown Sheet Temperature Port ID", "Boiler Broken Port ID" };
                case VanillaAudioSystem.SteamerCylinderCock:
                    return new string[0];

                case VanillaAudioSystem.BE2ElectricMotor:
                    return new[] { "TM RPM Port ID", "Total Amps Port ID" };

                default:
                    return new[] { "Port ID" };
            }
        }
        
        private string[] GetSourcePositionNames(VanillaAudioSystem system)
        {
            switch (system)
            {
                case VanillaAudioSystem.DE2Engine:
                case VanillaAudioSystem.DH4Engine:
                case VanillaAudioSystem.DM3Engine:
                    return new[] { "Gear", "Idle" };
                case VanillaAudioSystem.DE2EnginePiston:
                    return new[] { "Piston", "Throttle" };

                case VanillaAudioSystem.DE6EngineIdle:
                    return new[] { "Compressor Whine (Tail)", "Engine Exhaust (Idle)", "Shaft Whine" };
                case VanillaAudioSystem.DE6EngineThrottling:
                    return new[] { "Combustion Bass", "Compressor Whine", "Engine Exhaust" };

                case VanillaAudioSystem.SteamerFire:
                    return new[] { "Strong Fire", "Weak Fire" };
                case VanillaAudioSystem.SteamerValveGear:
                    return new[] { "4s", "8s", "16s" };

                case VanillaAudioSystem.S060Whistle:
                    return new[] { "Chime 1", "Chime 2", "Chord (Steam)" };
                case VanillaAudioSystem.S282Whistle:
                    return new[] { "Chime 1", "Chime 2", "Chime 3", "Chime 4", "Chime 5", "Chord (Steam)" };

                default:
                    return new string[0];
            }
        }

        private string[] GetClipNames(VanillaAudioSystem system)
        {
            switch (system)
            {
                case VanillaAudioSystem.S060Whistle:
                    return new[] { "Chime 1", "Chime 2", "Chord (Steam)" };
                case VanillaAudioSystem.S282Whistle:
                    return new[] { "Chime 1", "Chime 2", "Chime 3", "Chime 4", "Chime 5", "Chord (Steam)" };
                default:
                    return new string[0];
            }
        }
    }
}
