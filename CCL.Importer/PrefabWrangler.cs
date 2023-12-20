using CCL.Importer.Proxies;
using CCL.Importer.Types;
using CCL.Types;
using DV;
using DV.CabControls;
using DV.CabControls.Spec;
using DV.Optimizers;
using DV.Simulation.Brake;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Ports;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using DV.Utils;
using LocoSim.Definitions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Importer
{
    public static class PrefabWrangler
    {
        private static TrainCarLivery GetBaseType(TrainCarType baseType)
        {
            return Globals.G.Types.TrainCarType_to_v2[baseType];
        }

        public static bool FinalizeCarTypePrefabs(CCL_CarType carType)
        {
            foreach (var livery in carType.Variants)
            {
                if (!FinalizeLiveryPrefab(livery))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FinalizeLiveryPrefab(CCL_CarVariant livery)
        {
            CCLPlugin.Log($"Augmenting prefab for {livery.id}");

            // Create a modifiable copy of the prefab
            GameObject newFab = Object.Instantiate(livery.prefab, null);
            newFab.SetActive(false);
            Object.DontDestroyOnLoad(newFab);

            // Fetch the base type prefab for this car
            TrainCarLivery baseLivery = GetBaseType(livery.BaseCarType);

            var bufferPositions = WrangleBuffers(newFab, baseLivery, livery);
            var couplerPositions = WrangleCouplers(newFab, baseLivery, bufferPositions);
            var colliders = WrangleColliders(newFab, baseLivery);

            WrangleBogies(newFab, livery, baseLivery, colliders);
            CleanInfoPlates(newFab.transform);

            WrangleExternalInteractables(livery);

            UpdateLiveryShaders(livery);

            // Create new TrainCar script
            var newTrainCar = newFab.AddComponent<TrainCar>();
            newTrainCar.carLivery = livery;

            // TODO: loco params, cab

            newFab.name = livery.id;
            livery.prefab = newFab;

            WrangleProxies(livery);
            BuildSimulationElements(livery);

            CCLPlugin.Log($"Finalized prefab for {livery.id}");
            return true;
        }

        //==============================================================================================================
        #region Proxies
        private static void WrangleProxies(TrainCarLivery livery)
        {
            if (livery.interiorPrefab)
            {
                ProxyWrangler.Instance.MapProxiesOnPrefab(livery.interiorPrefab);
            }
            if (livery.externalInteractablesPrefab)
            {
                ProxyWrangler.Instance.MapProxiesOnPrefab(livery.externalInteractablesPrefab);
            }
            ProxyWrangler.Instance.MapProxiesOnPrefab(livery.prefab);
        }
        #endregion

        //==============================================================================================================
        #region SimulationComponents - SimController and friends

        private static void BuildSimulationElements(TrainCarLivery livery)
        {
            // Map additional controllers for all prefab parts
            AddAdditionalControllers(livery.prefab);
            if (livery.interiorPrefab)
            {
                AddAdditionalControllers(livery.interiorPrefab);
            }
            if (livery.externalInteractablesPrefab)
            {
                AddAdditionalControllers(livery.externalInteractablesPrefab);
            }

            // If we have something that gets referenced through the simConnections decoupling mechanism - these are generally things
            // that make ports exist.
            var hasSimConnections = livery.prefab.GetComponentsInChildren<SimComponentDefinition>().Length > 0 ||
                livery.prefab.GetComponentsInChildren<Connection>().Length > 0 ||
                livery.prefab.GetComponentsInChildren<PortReferenceConnection>().Length > 0;
            if (hasSimConnections)
            {
                AttachSimConnectionsToPrefab(livery.prefab);
            }

            // If we have something that can use a sim controller and don't already have a sim controller
            var needsSimController = livery.prefab.GetComponentInChildren<SimConnectionDefinition>() || 
                livery.prefab.GetComponentsInChildren<ASimInitializedController>().Length > 0 && 
                !livery.prefab.GetComponentInChildren<SimController>();
            if (needsSimController)
            {
                var simController = livery.prefab.AddComponent<SimController>();
                simController.connectionsDefinition = livery.prefab.GetComponent<SimConnectionDefinition>() ?? AttachSimConnectionsToPrefab(livery.prefab);
                simController.otherSimControllers = livery.prefab.GetComponentsInChildren<ASimInitializedController>();
            }
        }

        
        private static void AddAdditionalControllers(GameObject prefab)
        {
            if (prefab.GetComponentsInChildren<InteractablePortFeeder>().Length > 0)
            {
                var controller = prefab.AddComponent<InteractablePortFeedersController> ();
                controller.entries = prefab.GetComponentsInChildren <InteractablePortFeeder>();
            }

            if (prefab.GetComponentsInChildren<IndicatorPortReader>().Length > 0)
            {
                var controller = prefab.AddComponent<IndicatorPortReadersController> ();
                controller.entries = prefab.GetComponentsInChildren<IndicatorPortReader>();
            }
            // Add more wrapper controllers here - or possibly use MEF to initialize wrapper controllers?
        }

        private static SimConnectionDefinition AttachSimConnectionsToPrefab(GameObject prefab)
        {
            // SimConnectionDefinition is a structure that holds all of the magical port generating items
            var simConnections = prefab.AddComponent<SimConnectionDefinition>();

            simConnections.executionOrder = prefab.GetComponentsInChildren<SimComponentDefinition>();
            simConnections.connections = prefab.GetComponentsInChildren<Connection>();
            simConnections.portReferenceConnections = prefab.GetComponentsInChildren<PortReferenceConnection>();

            return simConnections;
        }

        #endregion

        //==============================================================================================================
        #region Buffer horrors beyond comprehension

        private struct PositionPair
        {
            public Vector3 Front;
            public Vector3 Rear;

            public PositionPair(Vector3 front, Vector3 rear)
            {
                Front = front;
                Rear = rear;
            }
        }

        private static PositionPair WrangleBuffers(GameObject newFab, TrainCarLivery baseCar, CCL_CarVariant newCar)
        {
            // copy main buffer part cohort
            GameObject bufferRoot = baseCar.prefab.transform.Find(CarPartNames.BUFFERS_ROOT).gameObject;
            bufferRoot = Object.Instantiate(bufferRoot, newFab.transform);
            bufferRoot.name = CarPartNames.BUFFERS_ROOT;

            // special case for refrigerator - chain rigs are parented to car root instead of [buffers]
            if (baseCar.v1 == TrainCarType.RefrigeratorWhite)
            {
                for (int i = 0; i < baseCar.prefab.transform.childCount; i++)
                {
                    var child = baseCar.prefab.transform.GetChild(i).gameObject;
                    if (child.name == CarPartNames.BUFFER_CHAIN_REGULAR)
                    {
                        GameObject copiedChain = Object.Instantiate(child, bufferRoot.transform);
                        copiedChain.name = CarPartNames.BUFFER_CHAIN_REGULAR;

                        var bufferController = copiedChain.GetComponent<TrainBufferController>();
                        if (copiedChain.transform.localPosition.z > 0)
                        {
                            // front buffer
                            bufferController.bufferModelLeft = bufferRoot.transform.Find(CarPartNames.BUFFER_PAD_FL);
                            bufferController.bufferModelRight = bufferRoot.transform.Find(CarPartNames.BUFFER_PAD_FR);
                        }
                        else
                        {
                            // rear buffer
                            bufferController.bufferModelLeft = bufferRoot.transform.Find(CarPartNames.BUFFER_PAD_RL);
                            bufferController.bufferModelRight = bufferRoot.transform.Find(CarPartNames.BUFFER_PAD_RR);
                        }
                    }
                }
            }

            if (newCar.UseCustomBuffers)
            {
                return SetupCustomBuffers(newFab, baseCar.prefab, newCar);
            }
            else
            {
                return SetupDefaultBuffers(newFab, baseCar.prefab);
            }
        }

        private static PositionPair SetupDefaultBuffers(GameObject newPrefab, GameObject basePrefab)
        {
            // yeet the dummy buffer rigs so they aren't duplicated
            Transform frontCouplerRig = newPrefab.transform.Find(CarPartNames.COUPLER_RIG_FRONT);
            Vector3 frontRigPosition;
            if (frontCouplerRig)
            {
                frontRigPosition = frontCouplerRig.position;
                Object.Destroy(frontCouplerRig.gameObject);
            }
            else
            {
                frontRigPosition = new Vector3(0, 1.05f, 8.77f);
                CCLPlugin.Error("Missing front coupler rig from prefab!");
            }

            Transform rearCouplerRig = newPrefab.transform.Find(CarPartNames.COUPLER_RIG_REAR);
            Vector3 rearRigPosition;
            if (rearCouplerRig)
            {
                rearRigPosition = rearCouplerRig.position;
                Object.Destroy(rearCouplerRig.gameObject);
            }
            else
            {
                rearRigPosition = new Vector3(0, 1.05f, -8.77f);
                CCLPlugin.Error("Missing rear coupler rig from prefab!");
            }

            // get copied buffer part cohort
            GameObject bufferRoot = newPrefab.transform.Find(CarPartNames.BUFFERS_ROOT).gameObject;

            // adjust transforms of buffer components
            for (int i = 0; i < bufferRoot.transform.childCount; i++)
            {
                Transform child = bufferRoot.transform.GetChild(i);
                string childName = child.name.Trim();

                if (CarPartNames.BUFFER_CHAIN_RIGS.Contains(childName))
                {
                    // front or rear chain rig
                    // determine whether front or rear chain rig: +z is front
                    child.localPosition = (child.localPosition.z > 0) ? frontRigPosition : rearRigPosition;
                }
                else if (CarPartNames.BUFFER_PLATE_FRONT.Equals(childName))
                {
                    // front hook plate
                    child.localPosition = frontRigPosition + CarPartOffset.HOOK_PLATE_F;
                }
                else if (CarPartNames.BUFFER_PLATE_REAR.Equals(childName))
                {
                    // rear hook plate
                    child.localPosition = rearRigPosition + CarPartOffset.HOOK_PLATE_R;
                }
                else if (CarPartNames.BUFFER_STEMS.Equals(childName))
                {
                    // stems
                    Object.Destroy(child.gameObject);
                }
                else if (CarPartNames.BUFFER_FRONT_PADS.Contains(childName))
                {
                    // front buffer pads
                    Vector3 xShiftBase = new Vector3(child.localPosition.x, frontRigPosition.y, frontRigPosition.z);
                    child.localPosition = xShiftBase + CarPartOffset.BUFFER_PAD_F;
                }
                else if (CarPartNames.BUFFER_REAR_PADS.Contains(childName))
                {
                    // rear buffer pads
                    Vector3 xShiftBase = new Vector3(child.localPosition.x, rearRigPosition.y, rearRigPosition.z);
                    child.localPosition = xShiftBase + CarPartOffset.BUFFER_PAD_R;
                }
                else
                {
                    CCLPlugin.LogVerbose($"Unknown buffer child {childName}");
                }
            }

            return new PositionPair(frontRigPosition, rearRigPosition);
        }

        private static PositionPair SetupCustomBuffers(GameObject newPrefab, GameObject basePrefab, CCL_CarVariant carSetup)
        {
            Transform frontCouplerRig = newPrefab.transform.Find(CarPartNames.COUPLER_RIG_FRONT);
            Vector3 frontRigPosition = frontCouplerRig.position;

            Transform rearCouplerRig = newPrefab.transform.Find(CarPartNames.COUPLER_RIG_REAR);
            Vector3 rearRigPosition = rearCouplerRig.position;

            // get copied buffer part cohort
            GameObject bufferRoot = newPrefab.transform.Find(CarPartNames.BUFFERS_ROOT).gameObject;

            Transform newFrontBufferRig = null!;
            Transform newRearBufferRig = null!;

            // adjust transforms of buffer components
            for (int i = 0; i < bufferRoot.transform.childCount; i++)
            {
                Transform child = bufferRoot.transform.GetChild(i);
                string childName = child.name.Trim(' ', '1');

                if (CarPartNames.BUFFER_CHAIN_RIGS.Contains(childName))
                {
                    // front or rear chain rig
                    // determine whether front or rear chain rig: +z is front
                    if (child.localPosition.z > 0)
                    {
                        child.localPosition = frontRigPosition;
                        newFrontBufferRig = child;
                        CCLPlugin.LogVerbose($"Set newFrontBufferRig {newFrontBufferRig.name}");
                    }
                    else
                    {
                        child.localPosition = rearRigPosition;
                        newRearBufferRig = child;
                        CCLPlugin.LogVerbose($"Set newRearBufferRig {newRearBufferRig.name}");
                    }
                }
                else if (CarPartNames.BUFFER_PLATE_FRONT.Equals(childName))
                {
                    // front hook plate
                    child.localPosition = frontRigPosition + CarPartOffset.HOOK_PLATE_F;
                    CCLPlugin.LogVerbose("Adjust Hook Plate F");
                }
                else if (CarPartNames.BUFFER_PLATE_REAR.Equals(childName))
                {
                    // rear hook plate
                    child.localPosition = rearRigPosition + CarPartOffset.HOOK_PLATE_R;
                    CCLPlugin.LogVerbose("Adjust Hook Plate R");
                }
                else if (CarPartNames.BUFFER_STEMS.Equals(childName))
                {
                    // destroy stems
                    Object.Destroy(child.gameObject);
                    CCLPlugin.LogVerbose("Destroy buffer stems");
                }
                else if (CarPartNames.BUFFER_FRONT_PADS.Contains(childName) || CarPartNames.BUFFER_REAR_PADS.Contains(childName))
                {
                    // destroy template buffer pads since we're overriding
                    Object.Destroy(child.gameObject);
                    CCLPlugin.LogVerbose($"Destroy buffer pad {childName}");
                }
                else
                {
                    CCLPlugin.LogVerbose($"Unknown buffer child {childName}");
                }
            }

            // duplicate front rig to replace missing rear
            if (!carSetup.HideBackCoupler && !newRearBufferRig)
            {
                newRearBufferRig = Object.Instantiate(newFrontBufferRig, newFrontBufferRig.parent);
                newRearBufferRig.eulerAngles = new Vector3(0, 180, 0);
                newRearBufferRig.localPosition = rearRigPosition;
            }

            // get rid of unwanted rear rig
            if (carSetup.HideBackCoupler && newRearBufferRig)
            {
                Object.Destroy(newRearBufferRig.gameObject);
            }

            // duplicate rear to replace missing front
            if (!carSetup.HideFrontCoupler && !newFrontBufferRig)
            {
                newFrontBufferRig = Object.Instantiate(newRearBufferRig, newRearBufferRig.parent);
                newFrontBufferRig.eulerAngles = Vector3.zero;
                newFrontBufferRig.localPosition = frontRigPosition;
            }

            if (carSetup.HideFrontCoupler && newFrontBufferRig)
            {
                Object.Destroy(newFrontBufferRig.gameObject);
            }

            // reparent buffer pads to new root & adjust anchor positions
            foreach (Transform rig in new[] { frontCouplerRig, rearCouplerRig })
            {
                if (!rig) continue;

                string lPadName, rPadName;
                Transform newBufferRig;

                if (rig == frontCouplerRig)
                {
                    if (carSetup.HideFrontCoupler) continue;
                    lPadName = CarPartNames.BUFFER_PAD_FL;
                    rPadName = CarPartNames.BUFFER_PAD_FR;
                    newBufferRig = newFrontBufferRig;
                }
                else
                {
                    if (carSetup.HideBackCoupler) continue;
                    lPadName = CarPartNames.BUFFER_PAD_RL;
                    rPadName = CarPartNames.BUFFER_PAD_RR;
                    newBufferRig = newRearBufferRig;
                }

                CCLPlugin.LogVerbose($"Adjust pads for {newBufferRig?.name}, rig = {rig != null}: {rig?.name}");

                // Reparent buffer pads
                TrainBufferController bufferController = newBufferRig!.gameObject.GetComponentInChildren<TrainBufferController>(true);
                if (bufferController)
                {
                    Vector3 position;

                    var lPad = rig.FindSafe(lPadName);
                    if (lPad)
                    {
                        position = newPrefab.transform.InverseTransformPoint(lPad!.position);
                        lPad.parent = bufferRoot.transform;
                        lPad.localPosition = position;
                        bufferController.bufferModelLeft = lPad;
                    }

                    var rPad = rig.FindSafe(rPadName);
                    if (rPad)
                    {
                        position = newPrefab.transform.InverseTransformPoint(rPad!.position);
                        rPad.parent = bufferRoot.transform;
                        rPad.localPosition = position;
                        bufferController.bufferModelRight = rPad;
                    }
                }
                else
                {
                    CCLPlugin.Warning($"No buffer controller, newBufferRig={newBufferRig} {rig!.name}");
                    continue;
                }

                // Adjust new anchors to match positions in prefab
                Transform? bufferChainRig = rig.FindSafe(CarPartNames.BUFFER_CHAIN_REGULAR);

                CCLPlugin.LogVerbose($"Adjust anchors for {newBufferRig.name} - {bufferChainRig?.name}");

                foreach (string anchorName in CarPartNames.BUFFER_ANCHORS)
                {
                    var anchor = bufferChainRig.FindSafe(anchorName);
                    var newAnchor = newBufferRig.Find(anchorName);

                    if (anchor)
                    {
                        newAnchor.localPosition = anchor!.localPosition;
                    }
                }

                // Adjust air hose & MU connector positions
                if (carSetup.UseCustomHosePositions)
                {
                    CCLPlugin.LogVerbose($"Adjust hoses for {newBufferRig?.name}");

                    var hoseRoot = bufferChainRig.FindSafe(CarPartNames.HOSES_ROOT);
                    var newHoseRoot = newBufferRig.FindSafe(CarPartNames.HOSES_ROOT);

                    Transform? airHose = hoseRoot.FindSafe(CarPartNames.AIR_HOSE);
                    CCLPlugin.LogVerbose($"Air hose = {!!airHose}");
                    if (airHose)
                    {
                        var newAir = newHoseRoot.FindSafe(CarPartNames.AIR_HOSE);
                        if (newAir)
                        {
                            newAir!.localPosition = airHose!.localPosition;
                        }
                    }

                    Transform? muHose = hoseRoot.FindSafe(CarPartNames.MU_CONNECTOR);
                    CCLPlugin.LogVerbose($"MU hose = {!!muHose}");
                    if (muHose)
                    {
                        var newMU = newHoseRoot.FindSafe(CarPartNames.MU_CONNECTOR);
                        if (newMU)
                        {
                            newMU!.localPosition = muHose!.localPosition;
                        }
                    }
                }
            }

            Object.Destroy(frontCouplerRig.gameObject);
            Object.Destroy(rearCouplerRig.gameObject);

            return new PositionPair(frontRigPosition, rearRigPosition);
        }

        private static PositionPair WrangleCouplers(GameObject newFab, TrainCarLivery baseCar, PositionPair bufferPositions)
        {
            GameObject copiedObject;

            GameObject frontCoupler = baseCar.prefab.transform.Find(CarPartNames.COUPLER_FRONT).gameObject;
            copiedObject = Object.Instantiate(frontCoupler, newFab.transform);
            copiedObject.name = CarPartNames.COUPLER_FRONT;
            var frontCouplerPosition = bufferPositions.Front + CarPartOffset.COUPLER_FRONT;
            copiedObject.transform.localPosition = frontCouplerPosition;

            GameObject rearCoupler = baseCar.prefab.transform.Find(CarPartNames.COUPLER_REAR).gameObject;
            copiedObject = Object.Instantiate(rearCoupler, newFab.transform);
            copiedObject.name = CarPartNames.COUPLER_REAR;
            var rearCouplerPosition = bufferPositions.Rear + CarPartOffset.COUPLER_REAR;
            copiedObject.transform.localPosition = rearCouplerPosition;

            return new PositionPair(frontCouplerPosition, rearCouplerPosition);
        }

        #endregion

        //==============================================================================================================
        #region Colliders

        private struct ColliderData
        {
            public Transform NewBogieColliderRoot;
            public CapsuleCollider[] BaseBogieColliders;

            public readonly CapsuleCollider BaseFrontBogie => BaseBogieColliders.First();
            public readonly CapsuleCollider BaseRearBogie => BaseBogieColliders.Last();

            public ColliderData(Transform bogieColliders, CapsuleCollider[] baseBogieColliders)
            {
                NewBogieColliderRoot = bogieColliders;
                BaseBogieColliders = baseBogieColliders.OrderByDescending(c => c.center.z).ToArray();
            }
        }

        private static ColliderData WrangleColliders(GameObject newFab, TrainCarLivery baseCar)
        {
            // [colliders]
            Transform colliderRoot = newFab.transform.Find(CarPartNames.COLLIDERS_ROOT);
            if (!colliderRoot)
            {
                // collider should be initialized in prefab, but make sure
                CCLPlugin.Warning("Adding collision root to car, should have been part of prefab!");

                GameObject colliders = new GameObject(CarPartNames.COLLIDERS_ROOT);
                colliderRoot = colliders.transform;
                colliderRoot.parent = newFab.transform;
            }

            // [collision]
            Transform collision = colliderRoot.Find(CarPartNames.COLLISION_ROOT);
            if (!collision)
            {
                var collisionObj = new GameObject(CarPartNames.COLLISION_ROOT);
                collision = collisionObj.transform;
                collision.parent = colliderRoot.transform;
            }
            // Ensure PitStop detects this as a serviceable car
            collision.tag = "MainTriggerCollider";

            // find [walkable]
            // copy walkable to items if items doesn't exist
            Transform walkable = colliderRoot.Find(CarPartNames.WALKABLE_COLLIDERS);
            if (walkable)
            {
                Transform items = colliderRoot.Find(CarPartNames.ITEM_COLLIDERS);
                if (!items)
                {
                    CCLPlugin.LogVerbose("Reusing walkable colliders as item colliders");
                    GameObject newItemsObj = Object.Instantiate(walkable.gameObject, colliderRoot);
                    newItemsObj.name = CarPartNames.ITEM_COLLIDERS;
                    newItemsObj.SetLayersRecursive(DVLayer.Interactable);
                }

                // set layer
                walkable.gameObject.SetLayersRecursive(DVLayer.Train_Walkable);

                // automagic bounding box from walkable
                var boundingColliders = collision.GetComponentsInChildren<BoxCollider>();
                if (boundingColliders.Length == 0)
                {
                    // autogenerate bounding box from walkable extents (only works with box collider bits though)
                    var walkableColliders = walkable.GetComponentsInChildren<BoxCollider>();
                    if (walkableColliders.Length > 0)
                    {
                        CCLPlugin.LogVerbose("Building bounding collision box from walkable colliders");

                        Bounds boundBox = BoundsUtil.BoxColliderAABB(walkableColliders[0], newFab.transform);
                        for (int i = 1; i < walkableColliders.Length; i++)
                        {
                            boundBox.Encapsulate(BoundsUtil.BoxColliderAABB(walkableColliders[i], newFab.transform));
                        }

                        BoxCollider newCollisionBox = collision.gameObject.AddComponent<BoxCollider>();
                        newCollisionBox.center = boundBox.center - collision.localPosition;
                        newCollisionBox.size = boundBox.size;
                    }
                }
            }

            // [bogies]
            Transform bogieColliderTform = colliderRoot.transform.Find(CarPartNames.BOGIE_COLLIDERS);
            if (!bogieColliderTform)
            {
                CCLPlugin.LogVerbose("Adding bogie collider root");

                GameObject bogiesRoot = new GameObject(CarPartNames.BOGIE_COLLIDERS);
                bogieColliderTform = bogiesRoot.transform;
                bogieColliderTform.parent = colliderRoot.transform;
            }

            Transform baseBogieColliderRoot = baseCar.prefab.transform.Find(CarPartNames.COLLIDERS_ROOT).Find(CarPartNames.BOGIE_COLLIDERS);
            var baseBogieColliders = baseBogieColliderRoot.GetComponentsInChildren<CapsuleCollider>();

            return new ColliderData(bogieColliderTform, baseBogieColliders);
        }

        #endregion

        //==============================================================================================================
        #region Bogies

        private static void WrangleBogies(GameObject newFab, CCL_CarVariant newCar, TrainCarLivery baseCar, ColliderData colliders)
        {
            Bogie frontBogie, rearBogie;

            TrainCar baseTrainCar = baseCar.prefab.GetComponent<TrainCar>();

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
            if (newCar.UseCustomFrontBogie && newFrontBogieTransform)
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
            if (newCar.UseCustomRearBogie && newRearBogieTransform)
            {
                rearBogie = newRearBogieTransform.gameObject.AddComponent<Bogie>();
            }
            else
            {
                rearBogie = StealBaseCarBogie(newFab.transform, newRearBogieTransform, colliders.NewBogieColliderRoot,
                    colliders.BaseRearBogie, baseTrainCar.Bogies.First());
            }

            // TODO: apply rear bogie config

            // Setup brake glows.
            SetupBrakeGlows(newFab, frontBogie, rearBogie, baseCar);
        }

        private static Bogie StealBaseCarBogie(Transform carRoot, Transform newBogieTransform, Transform bogieColliderRoot,
            CapsuleCollider baseBogieCollider, Bogie origBogie)
        {
            Vector3 bogiePosition = newBogieTransform.localPosition;
            Object.Destroy(newBogieTransform.gameObject);

            //GameObject origBogie = baseCar.Bogies[0].gameObject;
            GameObject copiedObject = Object.Instantiate(origBogie.gameObject, carRoot);
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

        private static void SetupBrakeGlows(GameObject newFab, Bogie front, Bogie rear, TrainCarLivery baseLivery)
        {
            var brakeGlow = newFab.AddComponent<BrakesOverheatingController>();
            List<Renderer> brakeRenderers = new();

            // Front bogie pads.
            Transform padsF = front.transform.Find(
                $"{CarPartNames.BOGIE_CAR}/{CarPartNames.BOGIE_BRAKE_ROOT}/{CarPartNames.BOGIE_BRAKE_PADS}");

            if (padsF != null)
            {
                // Grab ALL the renderers.
                brakeRenderers.AddRange(padsF.GetComponentsInChildren<Renderer>(true));
            }

            // Rear bogie pads.
            Transform padsR = rear.transform.Find(
                $"{CarPartNames.BOGIE_CAR}/{CarPartNames.BOGIE_BRAKE_ROOT}/{CarPartNames.BOGIE_BRAKE_PADS}");

            if (padsR != null)
            {
                brakeRenderers.AddRange(padsR.GetComponentsInChildren<Renderer>(true));
            }

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
                brakeGlow.overheatColor.colorGradient = baseLivery.prefab.GetComponent<BrakesOverheatingController>().overheatColor.colorGradient;
            }
        }

        #endregion

        //==============================================================================================================
        #region Info Plates

        private static void CleanInfoPlates(Transform carRoot)
        {
            // transforms should be found when the traincar script initializes them,
            // but chuck out the placeholder plates
            foreach (string plateName in CarPartNames.INFO_PLATES)
            {
                Transform plateRoot = carRoot.Find(plateName);
                if (plateRoot)
                {
                    foreach (Transform child in plateRoot)
                    {
                        Object.Destroy(child.gameObject);
                    }
                }
            }
        }

        #endregion

        //==============================================================================================================
        #region External Interactables

        private static GameObject _flatbedHandbrake = null!;
        private static GameObject _flatbedBrakeRelease = null!;

        public static void FetchInteractables()
        {
            var flatbedInteractables = TrainCarType.FlatbedEmpty.ToV2().externalInteractablesPrefab;
            _flatbedHandbrake = flatbedInteractables.transform.Find(CarPartNames.HANDBRAKE_SMALL).gameObject;
            _flatbedBrakeRelease = flatbedInteractables.transform.Find(CarPartNames.BRAKE_CYL_RELEASE).gameObject;
        }

        private static void WrangleExternalInteractables(CCL_CarVariant livery)
        {
            if (CarTypes.IsRegularCar(livery))
            {
                var newFab = SetupFreightInteractables(livery.externalInteractablesPrefab);

                var brakeFeeders = newFab.AddComponent<HandbrakeFeedersController>();
                brakeFeeders.RefreshChildren();

                var keyboardCtrl = newFab.AddComponent<InteractablesKeyboardControl>();
                keyboardCtrl.RefreshChildren();

                var optimizer = newFab.AddComponent<PlayerOnCarScriptsOptimizer>();
                optimizer.scriptsToDisable = new MonoBehaviour[] { keyboardCtrl };

                livery.externalInteractablesPrefab = newFab;
            }
        }

        private static GameObject SetupFreightInteractables(GameObject interactables)
        {
            GameObject newFab = Object.Instantiate(interactables, null);
            newFab.SetActive(false);
            Object.DontDestroyOnLoad(newFab);

            var existingChildren = Enumerable.Range(0, newFab.transform.childCount)
                .Select(i => newFab.transform.GetChild(i))
                .ToList();

            foreach (var current in existingChildren)
            {
                switch (current.name)
                {
                    case CarPartNames.DUMMY_HANDBRAKE_SMALL:
                        var newBrake = Object.Instantiate(_flatbedHandbrake, newFab.transform);
                        newBrake.transform.localPosition = current.localPosition;
                        newBrake.transform.localRotation = current.localRotation;
                        Object.Destroy(current.gameObject);
                        break;

                    case CarPartNames.DUMMY_BRAKE_RELEASE:
                        var newRelease = Object.Instantiate(_flatbedBrakeRelease, newFab.transform);
                        newRelease.transform.localPosition = current.localPosition;
                        newRelease.transform.localRotation = current.localRotation;
                        Object.Destroy(current.gameObject);
                        break;

                    default:
                        break;
                }
            }

            newFab.SetLayersRecursive(DVLayer.Interactable);
            FixControlColliders(newFab);

            return newFab;
        }

        private static void FixControlColliders(GameObject root)
        {
            var controls = root.GetComponentsInChildren<Collider>(true);
            foreach (var control in controls)
            {
                control.isTrigger = true;
            }
        }

        #endregion

        //==============================================================================================================
        #region Shaders

        private static Shader? _engineShader = null;

        private static Shader EngineShader
        {
            get
            {
                if (!_engineShader)
                {
                    var prefab = GetBaseType(TrainCarType.LocoShunter).prefab;
                    var exterior = prefab.transform.Find("LocoDE2_Body/ext 621_exterior");
                    var material = exterior.GetComponent<MeshRenderer>().material;
                    _engineShader = material.shader;
                }
                return _engineShader!;
            }
        }

        private static void UpdateLiveryShaders(CCL_CarVariant livery)
        {
            ApplyDefaultShader(livery.prefab);

            if (livery.interiorPrefab) ApplyDefaultShader(livery.interiorPrefab);
            if (livery.explodedInteriorPrefab) ApplyDefaultShader(livery.explodedInteriorPrefab);

            if (livery.externalInteractablesPrefab) ApplyDefaultShader(livery.externalInteractablesPrefab);
            if (livery.explodedExternalInteractablesPrefab) ApplyDefaultShader(livery.explodedExternalInteractablesPrefab);
        }

        private static void ApplyDefaultShader(GameObject prefab)
        {
            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.materials)
                {
                    // replace opaque material shader
                    if ((material.shader.name == "Standard") && (material.GetFloat("_Mode") == 0))
                    {
                        material.shader = EngineShader;
                    }
                }
            }
        }

        #endregion
    }
}
