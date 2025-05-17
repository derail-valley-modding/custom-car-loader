using CCL.Types;
using CCL.Types.Proxies;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class WizardlessWizards
    {
        [MenuItem("GameObject/CCL/Add Cab", false, MenuOrdering.Body.Cab)]
        public static void CreateCab(MenuCommand command)
        {
            var target = (GameObject)command.context;

            var cab = new GameObject(CarPartNames.Cab.TELEPORT_ROOT);
            cab.transform.parent = target.transform;

            var comp = cab.AddComponent<CabTeleportDestinationProxy>();

            var indicator = new GameObject("Collider");
            indicator.transform.parent = cab.transform;

            var box = indicator.AddComponent<BoxCollider>();
            box.isTrigger = true;

            indicator.AddComponent<GrabberRaycastPassThroughProxy>();

            var highlight = new GameObject("CabHighlightGlow");
            highlight.transform.parent = cab.transform;
            
            var glow = highlight.AddComponent<TeleportHoverGlowProxy>();

            var roomscale = new GameObject("RoomscalePosition");
            roomscale.transform.parent = cab.transform;

            comp.hoverGlow = glow;
            comp.roomscaleTeleportPosition = roomscale.transform;

            Undo.RegisterCreatedObjectUndo(cab, "Created Cab");
        }

        [MenuItem("GameObject/CCL/Add Cab", true, MenuOrdering.Body.Cab)]
        public static bool CreateCabValidate()
        {
            return Selection.activeGameObject;
        }

        [MenuItem("GameObject/CCL/Add License Blocker", false, MenuOrdering.Body.LicenseBlocker)]
        public static void CreateLicenseBlocker(MenuCommand command)
        {
            var target = (GameObject)command.context;

            var blocker = new GameObject(CarPartNames.LICENSE_BLOCKER);
            blocker.transform.parent = target.transform;

            var comp = blocker.AddComponent<LocoZoneBlockerProxy>();
            comp.cab = target.transform.root.GetComponentInChildren<CabTeleportDestinationProxy>();
            comp.blockerObjectsParent = blocker;

            var tp = new GameObject("TP_blocker");
            tp.transform.parent = blocker.transform;

            var box = tp.AddComponent<BoxCollider>();
            var reaction = tp.AddComponent<InvalidTeleportLocationReactionProxy>();
            reaction.blocker = comp;

            Undo.RegisterCreatedObjectUndo(blocker, "Created License Blocker");
        }

        [MenuItem("GameObject/CCL/Add License Blocker", true, MenuOrdering.Body.LicenseBlocker)]
        public static bool CreateLicenseBlockerValidate()
        {
            return Selection.activeGameObject;
        }
    }
}
