using CCL.Types;
using CCL.Types.Components;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Controls;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class WizardlessWizards
    {
        // This one is definitely not a wizard but oh well...
        [MenuItem("CCL/About", false, MenuOrdering.MenuBar.About)]
        public static void About(MenuCommand command)
        {
            EditorUtility.DisplayDialog("About CCL",
                $"Custom Car Loader version {ExporterConstants.ExporterVersion}\n" +
                $"Minimum compatible CCL editor version: {ExporterConstants.MinimumCompatibleVersion}\n" +
                $"Minimum DV version: {ExporterConstants.MINIMUM_DV_BUILD}",
                "Good Stuff");
        }

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

        [MenuItem("GameObject/CCL/Add Bed", false, MenuOrdering.Interior.Bed)]
        public static void CreateBed(MenuCommand command)
        {
            var target = (GameObject)command.context;

            var bed = new GameObject("Bed");
            bed.transform.parent = target.transform;

            var comp = bed.AddComponent<BedSleepingProxy>();

            var button = new GameObject("Button").AddComponent<ButtonProxy>();
            button.transform.parent = bed.transform;
            button.gameObject.AddComponent<BoxCollider>();
            button.gameObject.AddComponent<HighlightTagProxy>();
            button.gameObject.AddComponent<BedButtonProperties>();
            button.colliderGameObjects = new[] { button.gameObject };
            button.createRigidbody = false;
            button.useJoints = false;
            button.disableTouchUse = true;
            button.overrideUseButton = VRControllerButton.Grip;

            var pillow = new GameObject("Pillow");
            pillow.transform.parent = bed.transform;
            pillow.transform.localEulerAngles = new Vector3(29.387f, 82.06f, -7.238f);

            comp.fadeTime = 1.3f;
            comp.waitBeforeUnfade = 1.5f;
            comp.pillowTarget = pillow.transform;

            EditorUtility.DisplayDialog("Bed Wizard", "Don't forget to add a mesh highlight", "OK");
        }

        [MenuItem("GameObject/CCL/Add Bed", true, MenuOrdering.Interior.Bed)]
        public static bool CreateBedValidate()
        {
            return Selection.activeGameObject;
        }
    }
}
