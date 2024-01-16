using CCL.Types;
using CCL.Types.Components;
using CCL.Types.Proxies.Wheels;
using DV.Simulation.Brake;
using DV.ThingTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(ColliderProcessor))]
    internal class BogieProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            var newFab = context.Car.prefab;
            var colliders = context.GetCompletedStep<ColliderProcessor>();

            Bogie frontBogie, rearBogie;
            TrainCar baseTrainCar = context.BaseLivery.prefab.GetComponent<TrainCar>();

            // Find existing bogie transforms
            Transform newFrontBogieTransform = newFab.transform.Find(CarPartNames.BOGIE_FRONT);
            if (!newFrontBogieTransform)
            {
                CCLPlugin.Error("Front bogie transform is missing from prefab!");
            }

            Transform newRearBogieTransform = newFab.transform.Find(CarPartNames.BOGIE_REAR);
            if (!newRearBogieTransform)
            {
                CCLPlugin.Error("Rear bogie transform is missing from prefab!");
            }

            // Front Bogie
            if (context.Car.UseCustomFrontBogie && newFrontBogieTransform)
            {
                // replacing the original bogie, only steal the script
                frontBogie = newFrontBogieTransform.gameObject.AddComponent<Bogie>();
            }
            else
            {
                frontBogie = StealBaseCarBogie(newFab.transform, newFrontBogieTransform, colliders.NewBogieColliderRoot,
                    colliders.BaseFrontBogie, baseTrainCar.Bogies.Last());
            }

            // TODO: apply front bogie config

            // Rear Bogie
            if (context.Car.UseCustomRearBogie && newRearBogieTransform)
            {
                rearBogie = newRearBogieTransform.gameObject.AddComponent<Bogie>();
            }
            else
            {
                rearBogie = StealBaseCarBogie(newFab.transform, newRearBogieTransform, colliders.NewBogieColliderRoot,
                    colliders.BaseRearBogie, baseTrainCar.Bogies.First());
            }

            // TODO: apply rear bogie config

            SetupBrakeGlows(newFab, frontBogie, rearBogie, context.BaseLivery);
            SetupWheelSlideSparks(newFab, frontBogie, rearBogie);
        }

        private static Bogie StealBaseCarBogie(Transform carRoot, Transform newBogieTransform, Transform bogieColliderRoot,
            CapsuleCollider baseBogieCollider, Bogie origBogie)
        {
            Vector3 bogiePosition = newBogieTransform.localPosition;
            UnityEngine.Object.Destroy(newBogieTransform.gameObject);

            //GameObject origBogie = baseCar.Bogies[0].gameObject;
            GameObject copiedObject = UnityEngine.Object.Instantiate(origBogie.gameObject, carRoot);
            copiedObject.name = CarPartNames.BOGIE_FRONT;
            copiedObject.transform.localPosition = bogiePosition;

            Bogie newBogie = copiedObject.GetComponent<Bogie>();

            // grab collider as well
            CapsuleCollider newCollider = bogieColliderRoot.gameObject.AddComponent<CapsuleCollider>();

            newCollider.center = new Vector3(0, baseBogieCollider.center.y, bogiePosition.z);
            newCollider.direction = baseBogieCollider.direction;
            newCollider.radius = baseBogieCollider.radius;
            newCollider.height = baseBogieCollider.height;

            return newBogie;
        }

        private static BrakesOverheatingColorGradient? _defaultBrakeGradient = null;
        private static BrakesOverheatingColorGradient DefaultBrakeGradient =>
            Extensions.GetCached(ref _defaultBrakeGradient,
                () => Resources.FindObjectsOfTypeAll<BrakesOverheatingColorGradient>().FirstOrDefault(g => g.name == "BrakeShoeOverheatColorGradient"));

        private static void SetupBrakeGlows(GameObject newFab, Bogie front, Bogie rear, TrainCarLivery baseLivery)
        {
            List<Renderer> brakeRenderers = new();

            // Front bogie pads.
            Transform? padsF = front.transform.FindSafe(
                $"{CarPartNames.BOGIE_CAR}/{CarPartNames.BOGIE_BRAKE_ROOT}/{CarPartNames.BOGIE_BRAKE_PADS}");

            if (padsF)
            {
                // Grab ALL the renderers.
                brakeRenderers.AddRange(padsF!.GetComponentsInChildren<Renderer>(true));
            }

            // Rear bogie pads.
            Transform? padsR = rear.transform.FindSafe(
                $"{CarPartNames.BOGIE_CAR}/{CarPartNames.BOGIE_BRAKE_ROOT}/{CarPartNames.BOGIE_BRAKE_PADS}");

            if (padsR)
            {
                brakeRenderers.AddRange(padsR!.GetComponentsInChildren<Renderer>(true));
            }

            // Extra renderers (similar to how the S060, S282 and DM3 are set up).
            if (newFab.TryGetComponent(out ExtraBrakeRenderers extraBrakeRenderers))
            {
                brakeRenderers.AddRange(extraBrakeRenderers.Renderers);
            }

            if (!brakeRenderers.Any()) return;

            var brakeGlow = newFab.AddComponent<BrakesOverheatingController>();
            brakeGlow.brakeRenderers = brakeRenderers.ToArray();

            // Gradient.
            brakeGlow.overheatColor = ScriptableObject.CreateInstance<BrakesOverheatingColorGradient>();

            if (newFab.TryGetComponent(out CustomBrakeGlow customGlow))
            {
                // Use a custom one if available.
                brakeGlow.overheatColor.colorGradient = customGlow.ColourGradient;
            }
            else
            {
                // Or just use the same one as the base car type.
                if (baseLivery.prefab.TryGetComponent(out BrakesOverheatingController baseGlow))
                {
                    brakeGlow.overheatColor.colorGradient = baseGlow.overheatColor.colorGradient;
                }
                else
                {
                    brakeGlow.overheatColor.colorGradient = DefaultBrakeGradient.colorGradient;
                    CCLPlugin.LogVerbose($"Apply default brake gradient to {newFab.name}");
                }
            }
        }

        private static void SetupWheelSlideSparks(GameObject newFab, Bogie front, Bogie rear)
        {
            WheelSlideSparksControllerProxy controller = newFab.GetComponentInChildren<WheelSlideSparksControllerProxy>();

            // If the prefab has no proxy component, add one anyways and use it to automatically
            // setup the sparks, then treat it as usual.
            if (controller == null)
            {
                var sparks = new GameObject(CarPartNames.WHEEL_SPARKS);
                sparks.transform.parent = newFab.transform;
                sparks.transform.localPosition = Vector3.zero;
                controller = sparks.gameObject.AddComponent<WheelSlideSparksControllerProxy>();
                controller.AutoSetupWithBogies(front.transform.Find(CarPartNames.BOGIE_CAR), rear.transform.Find(CarPartNames.BOGIE_CAR));
            }

            var temp = controller.gameObject.AddComponent<DV.Wheels.WheelSlideSparksController>();
            temp.sparkAnchors = controller.sparkAnchors.Where(x => ProcessSparkAnchor(x, front.transform, rear.transform)).ToArray();
            UnityEngine.Object.Destroy(controller);
        }

        private static bool ProcessSparkAnchor(Transform anchor, Transform frontBogie, Transform rearBogie)
        {
            Transform parent = anchor.parent;

            if (parent.parent == frontBogie || parent.parent == rearBogie)
            {
                return true;
            }

            // Try to reparent if the anchors are in the fake bogies.
            if (parent.parent.name.Equals(CarPartNames.BOGIE_CAR))
            {
                if (parent.parent.parent.name.Equals(CarPartNames.BOGIE_FRONT))
                {
                    parent.parent = frontBogie;
                    return true;
                }
                if (parent.parent.parent.name.Equals(CarPartNames.BOGIE_REAR))
                {
                    parent.parent = rearBogie;
                    return true;
                }

                return false;
            }

            // Not part of a bogie, take it.
            return true;
        }
    }
}
