using CCL.Types;
using CCL.Types.Proxies;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class WizardlessWizards
    {
        [MenuItem("GameObject/CCL/Add Cab", false, 10)]
        public static void CreateCab(MenuCommand command)
        {
            var target = (GameObject)command.context;

            var cab = new GameObject(CarPartNames.Cab.TELEPORT_ROOT);
            cab.transform.parent = target.transform;

            var comp = cab.AddComponent<CabTeleportDestinationProxy>();

            var indicator = new GameObject("teleport_indicator");
            indicator.transform.parent = cab.transform;

            var box = indicator.AddComponent<BoxCollider>();
            box.isTrigger = true;

            indicator.AddComponent<GrabberRaycastPassThroughProxy>();

            var highlight = new GameObject("CabHighlightGlow");
            highlight.transform.parent = cab.transform;
            
            var glow = highlight.AddComponent<TeleportHoverGlowProxy>();

            var roomscale = new GameObject("roomscale position");
            roomscale.transform.parent = cab.transform;

            comp.hoverGlow = glow;
            comp.roomscaleTeleportPosition = roomscale.transform;

            Undo.RegisterCreatedObjectUndo(cab, "Created Cab");
        }

        [MenuItem("GameObject/CCL/Add Cab", true, 10)]
        public static bool CreateCabValidate()
        {
            return Selection.activeGameObject;
        }
    }
}
