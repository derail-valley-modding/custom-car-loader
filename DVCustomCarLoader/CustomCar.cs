using System;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using Object = UnityEngine.Object;
using CCL_GameScripts;

namespace DVCustomCarLoader
{
    public class CustomCar
    {
        /// <summary>
        ///     Identifier of this car.
        /// </summary>
        public string identifier = "Custom Car";

        /// <summary>
        ///     The underlying type of this car.
        /// </summary>
        public TrainCarType BaseCarType = TrainCarType.FlatbedEmpty;

        /// <summary>
        ///     The locomotive type of this car
        /// </summary>
        public LocoSimType SimType = LocoSimType.None;

        /// <summary>
        ///     The base prefab that will be duplicated from.
        /// </summary>
        public GameObject CarPrefab;

        //Bogies
        public bool HasCustomFrontBogie = false;
        public bool HasCustomRearBogie = false;
        public CustomBogieParams FrontBogieConfig = null;
        public CustomBogieParams RearBogieConfig = null;
        
        //Couplers
        public Vector3 FrontCouplerPosition;
        public Vector3 RearCouplerPosition;

        public void FinalizePrefab()
        {
            Main.ModEntry.Logger.Log($"Augmenting prefab for {identifier}");

            GameObject newFab = Object.Instantiate(CarPrefab, null);
            newFab.SetActive(false);
            Object.DontDestroyOnLoad(newFab);

            GameObject basePrefab = CarTypes.GetCarPrefab(BaseCarType);
            TrainCar baseCar = basePrefab.GetComponent<TrainCar>();

            GameObject copiedObject;

            //==============================================================================================================
            #region Buffers/Chains

            // yeet the dummy buffer rigs so they aren't duplicated
            Transform frontCouplerRig = newFab.transform.Find(CarPartNames.COUPLER_RIG_FRONT);
            Vector3 frontRigPosition;
            if( frontCouplerRig )
            {
                frontRigPosition = frontCouplerRig.position;
                Object.Destroy(frontCouplerRig.gameObject);
            }
            else
            {
                frontRigPosition = new Vector3(0, 1.05f, 8.77f);
                Main.ModEntry.Logger.Error("Missing front coupler rig from prefab!");
            }

            Transform rearCouplerRig = newFab.transform.Find(CarPartNames.COUPLER_RIG_REAR);
            Vector3 rearRigPosition;
            if( rearCouplerRig )
            {
                rearRigPosition = rearCouplerRig.position;
                Object.Destroy(rearCouplerRig.gameObject);
            }
            else
            {
                rearRigPosition = new Vector3(0, 1.05f, -8.77f);
                Main.ModEntry.Logger.Error("Missing rear coupler rig from prefab!");
            }

            // copy main buffer part cohort
            GameObject bufferRoot = basePrefab.transform.Find(CarPartNames.BUFFERS_ROOT).gameObject;
            bufferRoot = Object.Instantiate(bufferRoot, newFab.transform);
            bufferRoot.name = CarPartNames.BUFFERS_ROOT;

            // adjust transforms of buffer components
            for( int i = 0; i < bufferRoot.transform.childCount; i++ )
            {
                Transform child = bufferRoot.transform.GetChild(i);
                string childName = child.name.Trim();

                if( CarPartNames.BUFFER_CHAIN_RIG.Equals(childName) )
                {
                    // front or rear chain rig
                    // determine whether front or rear chain rig: +z is front
                    child.localPosition = (child.localPosition.z > 0) ? frontRigPosition : rearRigPosition;
                }
                else if( CarPartNames.BUFFER_PLATE_FRONT.Equals(childName) )
                {
                    // front hook plate
                    child.localPosition = frontRigPosition + CarPartOffset.HOOK_PLATE_F;
                }
                else if( CarPartNames.BUFFER_PLATE_REAR.Equals(childName) )
                {
                    // rear hook plate
                    child.localPosition = rearRigPosition + CarPartOffset.HOOK_PLATE_R;
                }
                else if( CarPartNames.BUFFER_FRONT_PADS.Contains(childName) )
                {
                    // front buffer pads
                    Vector3 xShiftBase = new Vector3(child.localPosition.x, frontRigPosition.y, frontRigPosition.z);
                    child.localPosition = xShiftBase + CarPartOffset.BUFFER_PAD_F;
                }
                else if( CarPartNames.BUFFER_REAR_PADS.Contains(childName) )
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
            TrainCar newCar = newFab.AddComponent<TrainCar>();

            // setup traincar properties
            newCar.bogieDamping = baseCar.bogieDamping;
            newCar.bogieMassRatio = baseCar.bogieMassRatio;
            newCar.bogieSpring = baseCar.bogieSpring;
            newCar.totalMass = baseCar.totalMass;
            newCar.wheelRadius = baseCar.wheelRadius;
            newCar.carType = BaseCarType;

            CarPrefab = newFab;

            string pn = CarPrefab == null ? "null" : "notnull";
            Main.ModEntry.Logger.Log($"New prefab for {identifier} is {pn}");

            Main.ModEntry.Logger.Log($"Finalized prefab for {identifier}");
        }

        private static Delegate[] carSpawnedDelegates = null;

        private static void RaiseCarSpawned( TrainCar car )
        {
            if( carSpawnedDelegates == null )
            {
                if( !(AccessTools.Field(typeof(CarSpawner), nameof(CarSpawner.CarSpawned)).GetValue(null) is MulticastDelegate mcd) )
                {
                    Main.ModEntry.Logger.Error("Couldn't get CarSpawner.CarSpawned delegate");
                    return;
                }

                carSpawnedDelegates = mcd.GetInvocationList();
            }

            var args = new object[] { car };
            foreach( Delegate d in carSpawnedDelegates )
            {
                d.Method.Invoke(d.Target, args);
            }
        }

        public TrainCar SpawnCar( RailTrack track, Vector3 position, Vector3 forward, bool playerSpawnedCar = false )
        {
            GameObject carObj = Object.Instantiate(CarPrefab);
            if( !carObj.activeSelf )
            {
                carObj.SetActive(true);
            }
            TrainCar spawnedCar = carObj.GetComponentInChildren<TrainCar>();

            spawnedCar.playerSpawnedCar = playerSpawnedCar;
            spawnedCar.InitializeNewLogicCar();
            spawnedCar.SetTrack(track, position, forward);
            
            spawnedCar.OnDestroyCar += Main.CustomCarManagerInstance.DeregisterCar;
            Main.CustomCarManagerInstance.RegisterSpawnedCar(spawnedCar, identifier);

            RaiseCarSpawned(spawnedCar);
            return spawnedCar;
        }

        public TrainCar SpawnLoadedCar(
            string carId, string carGuid, bool playerSpawnedCar, Vector3 position, Quaternion rotation,
            bool bogie1Derailed, RailTrack bogie1Track, double bogie1PositionAlongTrack,
            bool bogie2Derailed, RailTrack bogie2Track, double bogie2PositionAlongTrack,
            bool couplerFCoupled, bool couplerRCoupled )
        {
            GameObject carObj = Object.Instantiate(CarPrefab, position, rotation);
            if( !carObj.activeSelf )
            {
                carObj.SetActive(true);
            }

            TrainCar spawnedCar = carObj.GetComponentInChildren<TrainCar>();
            spawnedCar.playerSpawnedCar = playerSpawnedCar;
            spawnedCar.InitializeExistingLogicCar(carId, carGuid);

            if( !bogie1Derailed )
            {
                spawnedCar.Bogies[0].SetTrack(bogie1Track, bogie1PositionAlongTrack);
            }
            else
            {
                spawnedCar.Bogies[0].SetDerailedOnLoadFlag(true);
            }

            if( !bogie2Derailed )
            {
                spawnedCar.Bogies[1].SetTrack(bogie2Track, bogie2PositionAlongTrack);
            }
            else
            {
                spawnedCar.Bogies[1].SetDerailedOnLoadFlag(true);
            }

            spawnedCar.frontCoupler.forceCoupleStateOnLoad = true;
            spawnedCar.frontCoupler.loadedCoupledState = couplerFCoupled;
            spawnedCar.rearCoupler.forceCoupleStateOnLoad = true;
            spawnedCar.rearCoupler.loadedCoupledState = couplerRCoupled;

            spawnedCar.OnDestroyCar += Main.CustomCarManagerInstance.DeregisterCar;
            Main.CustomCarManagerInstance.RegisterSpawnedCar(spawnedCar, identifier);

            RaiseCarSpawned(spawnedCar);
            return spawnedCar;
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