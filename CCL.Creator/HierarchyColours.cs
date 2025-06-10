using CCL.Creator.Utility;
using CCL.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator
{
    [InitializeOnLoad]
    internal class HierarchyColours
    {
        private enum PrefabType
        {
            None,
            Root,
            Part
        };

        private static readonly Dictionary<string, Texture2D?> s_icons = new Dictionary<string, Texture2D?>();
        private static readonly Vector2 OffsetStart = new Vector2(-2, 0);
        private static readonly Vector2 OffsetActive = new Vector2(3, -1);
        private static readonly Vector2 OffsetInactive = new Vector2(3, 0);
        private static readonly Vector2 OffsetFull = new Vector2(58, 0);
        private static readonly Vector2 OffsetFullEnd = new Vector2(18, 0);
        private static readonly Color ColourPrefab = new Color32(125, 173, 243, 255);

        private static bool DarkMode => EditorGUIUtility.isProSkin;
        private static bool LightMode => !DarkMode;
        private static Color BackgroundColour => DarkMode ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
        private static Color SelectedBackgroundColour => DarkMode ? new Color32(44, 93, 135, 255) : new Color32(58, 114, 176, 255);
        private static Color HoveredBackgroundColour => DarkMode ? new Color32(68, 68, 68, 255) : new Color32(178, 178, 178, 255);

        static HierarchyColours()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= ItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += ItemGUI;
        }

        private static void ItemGUI(int instanceID, Rect selectionRect)
        {
            // Editor setting to use this or not.
            if (!CCLEditorSettings.Settings.HighlightSpecialGameObjectNames) return;

            // Valid GameObjects only.
            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj == null || !(obj is GameObject go)) return;

            GUIContent? content = null;
            Color? txC = null;

            switch (go.name)
            {
                // Colliders.
                case CarPartNames.Colliders.ROOT:
                    if (!IsUnderRoot(go))
                    {
                        SetBadEntry("This object must be a child of the root");
                        break;
                    }
                    if (go.transform.Find(CarPartNames.Colliders.COLLISION) &&
                        go.transform.Find(CarPartNames.Colliders.WALKABLE) &&
                        go.transform.Find(CarPartNames.Colliders.ITEMS))
                    {
                        content = EditorGUIUtility.IconContent("BoxCollider Icon");
                        content.tooltip = "The root of all colliders";
                        txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    }
                    else
                    {
                        SetWarning();
                    }
                    break;
                case CarPartNames.Colliders.COLLISION:
                case CarPartNames.Colliders.WALKABLE:
                case CarPartNames.Colliders.ITEMS:
                case CarPartNames.Colliders.CAMERA_DAMPENING:
                    content = EditorGUIUtility.IconContent("BoxCollider Icon");
                    txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    break;
                case CarPartNames.Colliders.BOGIES:
                    if (go.GetComponentsInChildren<CapsuleCollider>().Length == 2)
                    {
                        TrySetContentToTexture("Bogie");
                        txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    }
                    else
                    {
                        SetBadEntry("This object must have exactly 2 capsule colliders as its children");
                    }
                    break;
                case CarPartNames.Colliders.FALL_SAFETY:
                    if (go.TryGetComponent<Collider>(out var col) && col.isTrigger)
                    {
                        TrySetContentToTexture("FallSafety", "This object helps preventing falling out of vehicles");
                        txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    }
                    else
                    {
                        SetBadEntry(col ? "The collider in this object is not set as trigger" : "This object has no collider");
                    }
                    break;

                // Bogies.
                case CarPartNames.Bogies.FRONT:
                case CarPartNames.Bogies.REAR:
                case CarPartNames.Bogies.BOGIE_CAR:
                case CarPartNames.Bogies.BRAKE_ROOT:
                case CarPartNames.Bogies.BRAKE_PADS:
                case CarPartNames.Bogies.CONTACT_POINTS:
                case CarPartNames.Bogies.AXLE:
                    TrySetContentToTexture("Bogie");
                    txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    break;

                // Couplers.
                case CarPartNames.Couplers.RIG_FRONT:
                case CarPartNames.Couplers.RIG_REAR:
                case CarPartNames.Couplers.COUPLER_FRONT:
                case CarPartNames.Couplers.COUPLER_REAR:
                case CarPartNames.Couplers.CHAIN_ROOT:
                case CarPartNames.Couplers.HOSES_ROOT:
                case CarPartNames.Couplers.AIR_HOSE:
                case CarPartNames.Couplers.MU_CONNECTOR:
                case CarPartNames.Buffers.CHAIN_REGULAR:
                case CarPartNames.Buffers.PLATE_FRONT:
                case CarPartNames.Buffers.PLATE_REAR:
                    TrySetContentToTexture("Coupler");
                    txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    break;

                // Buffers.
                case CarPartNames.Buffers.PAD_FL:
                case CarPartNames.Buffers.PAD_RL:
                case CarPartNames.Buffers.PAD_FR:
                case CarPartNames.Buffers.PAD_RR:
                case "[BufferStems]":
                case "buffer anchor left":
                case "buffer anchor right":
                    TrySetContentToTexture("Buffers");
                    txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    break;

                // Car plates.
                case "[car plate anchor1]":
                case "[car plate anchor2]":
                    TrySetContentToTexture("CarPlate");
                    txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    break;

                // Dummy objects.
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_SMALL:
                case CarPartNames.Interactables.DUMMY_BRAKE_RELEASE:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_LARGE:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_S060:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_S282:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_DE2:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_DM3:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_DH4:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_MICROSHUNTER:
                case CarPartNames.Interactables.DUMMY_HANDBRAKE_DM1U:
                case CarPartNames.FuelPorts.DUMMY_FUEL_CAP_DE2:
                case CarPartNames.FuelPorts.DUMMY_CHARGE_PORT_BE2:
                    if (IsUnderRoot(go))
                    {
                        content = EditorGUIUtility.IconContent("ReflectionProbe Icon");
                        content.tooltip = "This object will be replaced at runtime by another";
                        txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    }
                    else
                    {
                        SetBadEntry("This object must be a child of the root");
                    }
                    break;

                // Dummy rain audio.
                case CarPartNames.Audio.RAIN_DUMMY_TRANSFORM:
                    content = EditorGUIUtility.IconContent("CloudConnect@2x");
                    txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    break;

                // Special transforms.
                case CarPartNames.CENTER_OF_MASS:
                case CarPartNames.Cab.TELEPORT_ROOT:
                case "[interior LOD]":
                    if (IsUnderRoot(go))
                    {
                        content = EditorGUIUtility.IconContent("Transform Icon");
                        txC = EditorHelpers.Colors.CONFIRM_ACTION;
                    }
                    else
                    {
                        SetBadEntry("This object must be a child of the root");
                    }
                    break;

                // Ignore this entirely for everything else.
                default:
                    return;
            }

            var prefab = GetPrefabType(go);
            var selected = IsSelected(go);
            var hovered = FullWidth(selectionRect).Contains(Event.current.mousePosition);

            selectionRect.position += OffsetStart;
            EditorGUI.DrawRect(selectionRect, GetBackgroundColour(selected, hovered));

            selectionRect.position += go.activeInHierarchy ? OffsetActive : OffsetInactive;
            content ??= GetDefaultIconContent(prefab, selected);
            content.text = go.name;
            content.tooltip = string.IsNullOrEmpty(content.tooltip) ? "This is a special CCL name" : content.tooltip;
            EditorGUI.LabelField(selectionRect, content, EditorHelpers.StyleWithTextColour(ProcessTextColour(go, txC, prefab, selected)));

            void TrySetContentToTexture(string name, string tooltip = "")
            {
                if (TryGetValidTexture(name, out var texture))
                {
                    content = new GUIContent(texture, tooltip);
                }
            }

            void SetBadEntry(string? reason = null)
            {
                content = EditorGUIUtility.IconContent("Error@2x");
                content.tooltip = string.IsNullOrEmpty(reason) ? "This object is incorrectly set up" : reason;
                txC = EditorHelpers.Colors.DELETE_ACTION;
            }

            void SetWarning()
            {
                content = EditorGUIUtility.IconContent("Warning@2x");
                txC = EditorHelpers.Colors.WARNING;
            }

            Rect FullWidth(Rect rect)
            {
                var rect2 = new Rect(rect);
                rect2.position -= OffsetFull;
                rect2.size += OffsetFull + OffsetFullEnd;
                return rect2;
            }
        }

        private static Color GetBackgroundColour(bool selected, bool hovered)
        {
            if (selected)
            {
                return SelectedBackgroundColour;
            }

            if (hovered)
            {
                return HoveredBackgroundColour;
            }

            return BackgroundColour;
        }

        private static bool IsSelected(GameObject go) => Selection.gameObjects.Contains(go);

        private static Color ProcessTextColour(GameObject go, Color? c, PrefabType prefab, bool selected)
        {
            if (selected) return GUI.contentColor;

            Color r = c.HasValue ? (DarkMode ? c.Value : Darken(c.Value)) : GUI.contentColor;

            if (prefab != PrefabType.None)
            {
                r = Color.Lerp(r, ColourPrefab, 0.65f);
            }

            return go.activeInHierarchy ? r : Transparent(r);
        }

        private static PrefabType GetPrefabType(GameObject go)
        {
            if (!PrefabUtility.IsPartOfAnyPrefab(go)) return PrefabType.None;

            return PrefabUtility.GetNearestPrefabInstanceRoot(go) == go ?
                PrefabType.Root :
                PrefabType.Part;
        }

        private static GUIContent GetDefaultIconContent(PrefabType type, bool selected) => type switch
        {
            PrefabType.Root => EditorGUIUtility.IconContent(selected ? "Prefab On Icon" : "Prefab Icon"),
            _ => EditorGUIUtility.IconContent(selected ? "GameObject On Icon" : "GameObject Icon"),
        };

        private static Color Darken(Color color) => new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, color.a);

        private static Color Transparent(Color color) => new Color(color.r, color.g, color.b, color.a * 0.65f);

        private static Texture2D? LoadTexture(string name)
        {
            string assetPath = $"Assets/CarCreator/Icons/Hierarchy/{name}.png";

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture)
            {
                s_icons.Add(name, texture);
                return texture;
            }
            return null;
        }

        private static bool TryGetValidTexture(string name, out Texture2D texture)
        {
            if (LightMode && TryGetValidTextureLightMode(name, out texture))
            {
                return true;
            }

            if (s_icons.TryGetValue(name, out texture!))
            {
                return texture;
            }
            else
            {
                s_icons[name] = LoadTexture(name);
            }

            return false;
        }

        private static bool TryGetValidTextureLightMode(string name, out Texture2D texture)
        {
            name = $"L_{name}";

            if (s_icons.TryGetValue(name, out texture!))
            {
                return texture;
            }
            else
            {
                s_icons[name] = LoadTexture(name);
            }

            return false;
        }

        private static bool IsUnderRoot(GameObject go) => go.transform.parent == go.transform.root;
    }
}
