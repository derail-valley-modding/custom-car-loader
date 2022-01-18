using System;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using Object = UnityEngine.Object;
using CCL_GameScripts;
using DV.Logic.Job;
using DVCustomCarLoader.LocoComponents;

namespace DVCustomCarLoader
{
    public class CustomCar
    {
        /// <summary>
        ///     Identifier of this car.
        /// </summary>
        public string identifier = "Custom Car";

        /// <summary>
        ///     Generated type enum for this car.
        /// </summary>
        public TrainCarType CarType = TrainCarType.NotSet;

        /// <summary>
        ///     The underlying type of this car.
        /// </summary>
        public TrainCarType BaseCarType = TrainCarType.FlatbedEmpty;
        public bool ReplaceBaseType { get; protected set; } = false;

        /// <summary>
        ///     The base prefab that will be duplicated from.
        /// </summary>
        public GameObject CarPrefab;

        public GameObject InteriorPrefab;

        //Bogies
        public bool HasCustomFrontBogie = false;
        public bool HasCustomRearBogie = false;
        public CustomBogieParams FrontBogieConfig = null;
        public CustomBogieParams RearBogieConfig = null;
        
        //Couplers
        public Vector3 FrontCouplerPosition;
        public Vector3 RearCouplerPosition;

        public LocoParamsType LocoType { get; protected set; } = LocoParamsType.None;
        public LocoAudioBasis LocoAudioType { get; protected set; } = LocoAudioBasis.None;
        public LocoRequiredLicense RequiredLicense { get; protected set; } = LocoRequiredLicense.None;
        public CargoContainerType CargoClass { get; protected set; } = CargoContainerType.None;

        public Sprite BookletSprite { get; set; } = null;
        public float FullDamagePrice { get; protected set; } = 10000f;

        public bool FinalizePrefab()
        {
            Main.ModEntry.Logger.Log($"Augmenting prefab for {identifier}");

            GameObject newFab = Object.Instantiate(CarPrefab, null);
            newFab.SetActive(false);
            Object.DontDestroyOnLoad(newFab);

            TrainCarSetup carSetup = newFab.GetComponent<TrainCarSetup>();
            if( !carSetup )
            {
                Main.Error($"Prefab {CarPrefab.name} for {identifier} has no TrainCarSetup!");
                return false;
            }

            GameObject basePrefab = CarTypes.GetCarPrefab(BaseCarType);
            TrainCar baseCar = basePrefab.GetComponent<TrainCar>();

            GameObject copiedObject;

            //==============================================================================================================
            #region Buffers/Chains

            Vector3 frontRigPosition, rearRigPosition;
            if (carSetup.UseCustomBuffers)
            {
                (frontRigPosition, rearRigPosition) = SetupCustomBuffers(newFab, basePrefab, carSetup.UseCustomHosePositions);
            }
            else
            {
                (frontRigPosition, rearRigPosition) = SetupDefaultBuffers(newFab, basePrefab);
            }

            #endregion

            //==============================================================================================================
            #region Extra Coupler Transforms

            GameObject frontCoupler = basePrefab.transform.Find(CarPartNames.COUPLER_FRONT).gameObject;
            copiedObject = Object.Instantiate(frontCoupler, newFab.transform);
            copiedObject.name = CarPartNames.COUPLER_FRONT;
            FrontCouplerPosition = frontRigPosition + CarPartOffset.COUPLER_FRONT;
            copiedObject.transform.localPosition = FrontCouplerPosition;

            GameObject rearCoupler = basePrefab.transform.Find(CarPartNames.COUPLER_REAR).gameObject;
            copiedObject = Object.Instantiate(rearCoupler, newFab.transform);
            copiedObject.name = CarPartNames.COUPLER_REAR;
            RearCouplerPosition = rearRigPosition + CarPartOffset.COUPLER_REAR;
            copiedObject.transform.localPosition = RearCouplerPosition;

            #endregion

            //==============================================================================================================
            #region Colliders

            // [colliders]
            Transform colliderRoot = newFab.transform.Find(CarPartNames.COLLIDERS_ROOT);
            if( !colliderRoot )
            {
                // collider should be initialized in prefab, but make sure
                Main.ModEntry.Logger.Warning($"Adding collision root to {identifier}, should have been part of prefab!");

                GameObject colliders = new GameObject(CarPartNames.COLLIDERS_ROOT);
                colliderRoot = colliders.transform;
                colliderRoot.parent = newFab.transform;
            }

            // [collision]
            Transform collision = colliderRoot.Find(CarPartNames.COLLISION_ROOT);
            if( !collision )
            {
                var collisionObj = new GameObject(CarPartNames.COLLISION_ROOT);
                collision = collisionObj.transform;
                collision.parent = colliderRoot.transform;
            }

            // find [walkable]
            // copy walkable to items if items doesn't exist
            Transform walkable = colliderRoot.Find(CarPartNames.WALKABLE_COLLIDERS);
            if( walkable )
            {
                Transform items = colliderRoot.Find(CarPartNames.ITEM_COLLIDERS);
                if( !items )
                {
                    Main.ModEntry.Logger.Log("Reusing walkable colliders as item colliders");
                    GameObject newItemsObj = Object.Instantiate(walkable.gameObject, colliderRoot);
                    newItemsObj.name = CarPartNames.ITEM_COLLIDERS;
                }

                // set layer
                walkable.gameObject.SetLayersRecursive("Train_Walkable");

                var boundingColliders = collision.GetComponentsInChildren<BoxCollider>();
                if( boundingColliders.Length == 0 )
                {
                    // autogenerate bounding box from walkable extents (only works with box collider bits though)
                    var walkableColliders = walkable.GetComponentsInChildren<BoxCollider>();
                    if( walkableColliders.Length > 0 )
                    {
                        Main.ModEntry.Logger.Log("Building bounding collision box from walkable colliders");

                        Bounds boundBox = BoundsUtil.BoxColliderAABB(walkableColliders[0], newFab.transform);
                        for( int i = 1; i < walkableColliders.Length; i++ )
                        {
                            boundBox.Encapsulate(BoundsUtil.BoxColliderAABB(walkableColliders[i], newFab.transform));
                        }

                        BoxCollider newCollisionBox = collision.gameObject.AddComponent<BoxCollider>();
                        newCollisionBox.center = boundBox.center - collision.localPosition;
                        newCollisionBox.size = boundBox.size;
                    }
                }
            }

            Transform bogieColliderTform = colliderRoot.transform.Find(CarPartNames.BOGIE_COLLIDERS);
            if( !bogieColliderTform )
            {
                Main.ModEntry.Logger.Log("Adding bogie collider root");

                GameObject bogiesRoot = new GameObject(CarPartNames.BOGIE_COLLIDERS);
                bogieColliderTform = bogiesRoot.transform;
                bogieColliderTform.parent = colliderRoot.transform;
            }

            Transform baseBogieColliderRoot = baseCar.transform.Find(CarPartNames.COLLIDERS_ROOT).Find(CarPartNames.BOGIE_COLLIDERS);
            var baseBogieColliders = baseBogieColliderRoot.GetComponentsInChildren<CapsuleCollider>();

            #endregion

            //==============================================================================================================
            #region Bogies

            Bogie frontBogie, rearBogie;

            // Find existing bogie transforms
            Transform newFrontBogieTransform = newFab.transform.Find(CarPartNames.BOGIE_FRONT);
            if( !newFrontBogieTransform )
            {
                Main.ModEntry.Logger.Error("Front bogie transform is missing from prefab!");
            }

            Transform newRearBogieTransform = newFab.transform.Find(CarPartNames.BOGIE_REAR);
            if( !newRearBogieTransform )
            {
                Main.ModEntry.Logger.Error("Rear bogie transform is missing from prefab!");
            }

            // Front bogie
            if( this.HasCustomFrontBogie && newFrontBogieTransform )
            {
                // replacing the original bogie, only steal the script
                frontBogie = newFrontBogieTransform.gameObject.AddComponent<Bogie>();
            }
            else
            {
                // need to steal the original bogie
                Vector3 bogiePosition = newFrontBogieTransform.localPosition;
                Object.Destroy(newFrontBogieTransform.gameObject);

                GameObject origBogie = baseCar.Bogies[0].gameObject;
                copiedObject = Object.Instantiate(origBogie, newFab.transform);
                copiedObject.name = CarPartNames.BOGIE_FRONT;
                copiedObject.transform.localPosition = bogiePosition;

                frontBogie = copiedObject.GetComponent<Bogie>();

                // grab collider as well
                CapsuleCollider toCopy = (baseBogieColliders[0].center.z > 0) ? baseBogieColliders[0] : baseBogieColliders[1];
                CapsuleCollider newCollider = bogieColliderTform.gameObject.AddComponent<CapsuleCollider>();

                newCollider.center = new Vector3(0, toCopy.center.y, bogiePosition.z);
                newCollider.direction = toCopy.direction;
                newCollider.radius = toCopy.radius;
                newCollider.height = toCopy.height;
            }

            if( FrontBogieConfig != null )
            {
                FrontBogieConfig.ApplyToBogie(frontBogie);
            }

            // Rear bogie
            if( this.HasCustomRearBogie && newRearBogieTransform )
            {
                // use bogie from new prefab
                rearBogie = newRearBogieTransform.gameObject.AddComponent<Bogie>();
            }
            else
            {
                // steal original bogie
                Vector3 bogiePosition = newRearBogieTransform.localPosition;
                Object.Destroy(newRearBogieTransform.gameObject);

                GameObject origBogie = baseCar.Bogies.Last().gameObject;
                copiedObject = Object.Instantiate(origBogie, newFab.transform);
                copiedObject.name = CarPartNames.BOGIE_REAR;
                copiedObject.transform.localPosition = bogiePosition;

                rearBogie = copiedObject.GetComponent<Bogie>();

                // grab collider as well
                CapsuleCollider toCopy = (baseBogieColliders[0].center.z < 0) ? baseBogieColliders[0] : baseBogieColliders[1];
                CapsuleCollider newCollider = bogieColliderTform.gameObject.AddComponent<CapsuleCollider>();

                newCollider.center = new Vector3(0, toCopy.center.y, bogiePosition.z);
                newCollider.direction = toCopy.direction;
                newCollider.radius = toCopy.radius;
                newCollider.height = toCopy.height;
            }

            if( RearBogieConfig != null )
            {
                RearBogieConfig.ApplyToBogie(rearBogie);
            }

            #endregion

            //==============================================================================================================
            #region Info Plates

            // transforms should be found when the traincar script initializes them,
            // but chuck out the placeholder plates
            foreach( string plateName in CarPartNames.INFO_PLATES )
            {
                Transform plateRoot = newFab.transform.Find(plateName);
                if( plateRoot )
                {
                    foreach( Transform child in plateRoot )
                    {
                        Object.Destroy(child.gameObject);
                    }
                }
            }

            #endregion

            // Setup new car script
            var newCar = InitSpecManager.CreateRealComponent<TrainCarSetup, TrainCar>(carSetup);
            if( !newCar )
            {
                Main.Warning("Couldn't create TrainCar component");
                Object.Destroy(newFab);
                return false;
            }

            // setup traincar properties
            CargoClass = (CargoContainerType)carSetup.CargoClass;
            BookletSprite = carSetup.BookletSprite;
            FullDamagePrice = carSetup.FullDamagePrice;
            ReplaceBaseType = carSetup.ReplaceBaseType;

            Main.Log($"Cargo class: {CargoClass}, Damage price: {FullDamagePrice}");

            if( !carSetup.OverridePhysics )
            {
                newCar.bogieDamping = baseCar.bogieDamping;
                newCar.bogieMassRatio = baseCar.bogieMassRatio;
                newCar.bogieSpring = baseCar.bogieSpring;
                newCar.totalMass = baseCar.totalMass;
                newCar.wheelRadius = baseCar.wheelRadius;
            }

            newCar.carType = CarType;

            //==============================================================================================================
            #region Loco Params

            var simParams = newFab.GetComponent<SimParamsBase>();
            if( simParams )
            {
                LocoComponentManager.AddLocoSimulation(newFab, simParams);
                LocoType = simParams.SimType;
                LocoAudioType = simParams.AudioType;
                RequiredLicense = simParams.RequiredLicense;
            }
            
            if( carSetup.InteriorPrefab )
            {
                GameObject interiorFab = Object.Instantiate(carSetup.InteriorPrefab, null);
                interiorFab.SetActive(false);
                Object.DontDestroyOnLoad(interiorFab);

                LocoComponentManager.CreateComponentsFromProxies(interiorFab);
                LocoComponentManager.CreateCopiedControls(interiorFab);
                interiorFab.SetLayersRecursive("Interactable");

                LocoComponentManager.MakeDoorsCollidable(interiorFab);

                interiorFab.AddComponent<DoorAndWindowTracker>();

                newCar.interiorPrefab = interiorFab;
                InteriorPrefab = interiorFab;
            }

            var prefabProxies = newFab.GetComponentsInChildren<ComponentInitSpec>(true);
            foreach (ComponentInitSpec spec in prefabProxies)
            {
                if (spec is TrainCarSetup) continue;
                InitSpecManager.CreateRealComponent(spec);
            }

            newFab.AddComponent<LocoEventManager>();

            #endregion

            //==============================================================================================================
            #region Cab

            var cabTform = newFab.transform.Find(CarPartNames.CAB_TELEPORT_ROOT);
            if( cabTform )
            {
                var teleportDest = cabTform.gameObject.AddComponent<CabTeleportDestination>();
                teleportDest.hoverRenderer = cabTform.GetComponentInChildren<Renderer>();

                if (InteriorPrefab)
                {
                    var cabCollider = cabTform.gameObject.GetComponentInChildren<BoxCollider>();
                    if (cabCollider)
                    {
                        var snapshotSwitcher = newFab.AddComponent<CabinSnapshotSwitcher>();
                        snapshotSwitcher.box = cabCollider;
                    }
                }
            }

            #endregion

            CarPrefab = newFab;
            CarPrefab.name = identifier;

            Main.ModEntry.Logger.Log($"Finalized prefab for {identifier}");
            return true;
        }

        private float _interCouplerDistance;

        public float InterCouplerDistance
        {
            get
            {
                if (_interCouplerDistance == 0.0)
                    _interCouplerDistance = Vector3.Distance(FrontCouplerPosition, RearCouplerPosition);
                return _interCouplerDistance;
            }
        }

        private Bounds _bounds;
        
        public Bounds Bounds
        {
            get
            {
                if (this._bounds.size.z == 0.0)
                {
                    //._bounds = GetCollisionBounds(CarPrefab);
                    _bounds = GetTotalBounds(CarPrefab);
                    this._bounds.Encapsulate(this.FrontCouplerPosition);
                    this._bounds.Encapsulate(this.RearCouplerPosition);
                }
                return this._bounds;
            }
        }
        
        public static Bounds GetTotalBounds(GameObject car)
        {
            //Get all colliders and renderers.
            BoxCollider[] colliders = car.GetComponentsInChildren<BoxCollider>();
            Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
            
            //Return default bounds if no colliders or renderers.
            if (colliders.Length == 0 && renderers.Length==0)
                return new Bounds();
            
            //Create bounds depending on first collider or renderer.
            Bounds bounds = colliders.Length > 0
                ? GetBounds(colliders[0])
                : GetMeshBounds(renderers[0]);

            foreach (var col in colliders)
            {
                bounds.Encapsulate(GetBounds(col));
            }
            foreach (var ren in renderers)
            {
                bounds.Encapsulate(ren.bounds);
            }
            
            return bounds;

            Bounds GetBounds(BoxCollider collider)
            {
                return new Bounds(Vector3.Scale(collider.center, collider.transform.lossyScale), Vector3.Scale(collider.size, collider.transform.lossyScale));
            }

            Bounds GetMeshBounds(Renderer r)
            {
                Bounds newBounds = new Bounds();
                newBounds.Encapsulate(r.bounds);
                return newBounds;
            }
        }

        #region Buffer & Chains Setup

        private (Vector3, Vector3) SetupDefaultBuffers(GameObject newPrefab, GameObject basePrefab)
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
                Main.Error("Missing front coupler rig from prefab!");
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
                Main.Error("Missing rear coupler rig from prefab!");
            }

            // copy main buffer part cohort
            GameObject bufferRoot = basePrefab.transform.Find(CarPartNames.BUFFERS_ROOT).gameObject;
            bufferRoot = Object.Instantiate(bufferRoot, newPrefab.transform);
            bufferRoot.name = CarPartNames.BUFFERS_ROOT;

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
                    Main.ModEntry.Logger.Log($"Unknown buffer child {childName}");
                }
            }

            return (frontRigPosition, rearRigPosition);
        }

        private (Vector3, Vector3) SetupCustomBuffers(GameObject newPrefab, GameObject basePrefab, bool customHoses)
        {
            Transform frontCouplerRig = newPrefab.transform.Find(CarPartNames.COUPLER_RIG_FRONT);
            Vector3 frontRigPosition = frontCouplerRig.position;

            Transform rearCouplerRig = newPrefab.transform.Find(CarPartNames.COUPLER_RIG_REAR);
            Vector3 rearRigPosition = rearCouplerRig.position;

            // copy main buffer part cohort
            GameObject bufferRoot = basePrefab.transform.Find(CarPartNames.BUFFERS_ROOT).gameObject;
            bufferRoot = Object.Instantiate(bufferRoot, newPrefab.transform);
            bufferRoot.name = CarPartNames.BUFFERS_ROOT;

            Transform newFrontBufferRig = null;
            Transform newRearBufferRig = null;

            // adjust transforms of buffer components
            for (int i = 0; i < bufferRoot.transform.childCount; i++)
            {
                Transform child = bufferRoot.transform.GetChild(i);
                string childName = child.name.Trim();

                if (CarPartNames.BUFFER_CHAIN_RIGS.Contains(childName))
                {
                    // front or rear chain rig
                    // determine whether front or rear chain rig: +z is front
                    if (child.localPosition.z > 0)
                    {
                        child.localPosition = frontRigPosition;
                        newFrontBufferRig = child;
                        Main.Log($"Set newFrontBufferRig {newFrontBufferRig.name}");
                    }
                    else
                    {
                        child.localPosition = rearRigPosition;
                        newRearBufferRig = child;
                        Main.Log($"Set newRearBufferRig {newRearBufferRig.name}");
                    }
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
                else if (CarPartNames.BUFFER_FRONT_PADS.Contains(childName) || CarPartNames.BUFFER_REAR_PADS.Contains(childName))
                {
                    // destroy template buffer pads since we're overriding
                    GameObject.Destroy(child.gameObject);
                }
                else
                {
                    Main.Log($"Unknown buffer child {childName}");
                }
            }

            // reparent buffer pads to new root & adjust anchor positions
            foreach (Transform rig in new []{ frontCouplerRig, rearCouplerRig })
            {
                string lPadName, rPadName;
                Transform newBufferRig;

                if (rig == frontCouplerRig)
                {
                    lPadName = CarPartNames.BUFFER_PAD_FL;
                    rPadName = CarPartNames.BUFFER_PAD_FR;
                    newBufferRig = newFrontBufferRig;
                }
                else
                {
                    lPadName = CarPartNames.BUFFER_PAD_RL;
                    rPadName = CarPartNames.BUFFER_PAD_RR;
                    newBufferRig = newRearBufferRig;
                }

                // Reparent buffer pads
                BufferController bufferController = newBufferRig.gameObject.GetComponentInChildren<BufferController>(true);
                if (bufferController)
                {
                    var lPad = rig.Find(lPadName);
                    Vector3 position = newPrefab.transform.InverseTransformPoint(lPad.position);
                    lPad.parent = bufferRoot.transform;
                    lPad.localPosition = position;
                    bufferController.bufferModelLeft = lPad;

                    var rPad = rig.Find(rPadName);
                    position = newPrefab.transform.InverseTransformPoint(rPad.position);
                    rPad.parent = bufferRoot.transform;
                    rPad.localPosition = position;
                    bufferController.bufferModelRight = rPad;
                }
                else
                {
                    Main.Warning($"No buffer controller, newBufferRig={newBufferRig} {rig.name}");
                    continue;
                }

                // Adjust new anchors to match positions in prefab
                Transform bufferChainRig = rig.Find(CarPartNames.BUFFER_CHAIN_REGULAR);

                foreach (string anchorName in CarPartNames.BUFFER_ANCHORS)
                {
                    var anchor = bufferChainRig.Find(anchorName);
                    var newAnchor = newBufferRig.Find(anchorName);

                    newAnchor.localPosition = anchor.localPosition;
                }

                // Adjust air hose & MU connector positions
                if (customHoses)
                {
                    var hoseRoot = bufferChainRig.Find(CarPartNames.HOSES_ROOT);
                    var newHoseRoot = newBufferRig.Find(CarPartNames.HOSES_ROOT);

                    if (hoseRoot.Find(CarPartNames.AIR_HOSE) is Transform airHose)
                    {
                        var newAir = newHoseRoot.Find(CarPartNames.AIR_HOSE);
                        newAir.localPosition = airHose.localPosition;
                    }

                    if (hoseRoot.Find(CarPartNames.MU_CONNECTOR) is Transform muHose)
                    {
                        var newMU = newHoseRoot.Find(CarPartNames.MU_CONNECTOR);
                        newMU.localPosition = muHose.localPosition;
                    }
                }
            }

            GameObject.Destroy(frontCouplerRig.gameObject);
            GameObject.Destroy(rearCouplerRig.gameObject);

            return (frontRigPosition, rearRigPosition);
        }

        #endregion
    }

    public class CustomBogieParams
    {
        public int AxleCount;
        public float AxleSeparation;
        public float BrakingForcePerBar;
        public float RollingResistanceCoefficient;

        public static CustomBogieParams FromJSON( JSONObject json )
        {
            return new CustomBogieParams()
            {
                AxleCount = (int)json.GetField("axleCount").i,
                AxleSeparation = json.GetField("axleSeparation").n,
                BrakingForcePerBar = json.GetField("brakingForcePerBar").n,
                RollingResistanceCoefficient = json.GetField("rollingResistance").n
            };
        }

        public void ApplyToBogie( Bogie target )
        {
            if( AxleCount > 0 ) target.axleCount = AxleCount;
            if( AxleSeparation > 0 ) target.axleSeparation = AxleSeparation;
            if( BrakingForcePerBar > 0 ) target.brakingForcePerBar = BrakingForcePerBar;
            if( RollingResistanceCoefficient > 0 ) target.rollingResistanceCoefficient = RollingResistanceCoefficient;
        }
    }
}