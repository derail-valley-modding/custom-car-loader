using System;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using Object = UnityEngine.Object;

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
        ///     The base prefab that will be duplicated from.
        /// </summary>
        public GameObject CarPrefab;

        //Bogies
        public Vector3 FrontBogiePosition;
        public Vector3 RearBogiePosition;

        public string FrontBogieReplacement = null;
        public string RearBogieReplacement = null;

        public CustomBogieParams FrontBogieConfig = null;
        public CustomBogieParams RearBogieConfig = null;
        
        //Couplers
        public Vector3 FrontCouplerPosition;
        public Vector3 RearCouplerPosition;

        ////Chains
        //public Vector3 FrontChainPosition;
        //public Vector3 RearChainPosition;

        ////Hoses
        //public Vector3 FrontHosePosition;
        //public Vector3 RearHosePosition;
        
        ////Buffers
        //public Vector3 FrontBufferPosition;
        //public Vector3 RearBufferPosition;

        //Name Plates
        public Vector3 SidePlate1Position;
        public Vector3 SidePlate2Position;

        public void FinalizePrefab()
        {
            Main.ModEntry.Logger.Log($"Augmenting prefab for {identifier}");

            GameObject newFab = Object.Instantiate(CarPrefab, null);
            newFab.SetActive(false);
            Object.DontDestroyOnLoad(newFab);

            GameObject basePrefab = CarTypes.GetCarPrefab(BaseCarType);
            TrainCar baseCar = basePrefab.GetComponent<TrainCar>();

            GameObject copiedObject;

            // Buffers/Chains
            // copy main buffer part cohort
            GameObject bufferRoot = basePrefab.transform.Find(CarPartNames.BUFFERS_ROOT).gameObject;
            bufferRoot = Object.Instantiate(bufferRoot, newFab.transform);
            bufferRoot.name = CarPartNames.BUFFERS_ROOT;

            // adjust transforms of buffer components
            for( int i = 0; i < bufferRoot.transform.childCount; i++ )
            {
                Transform child = bufferRoot.transform.GetChild(i);
                if( CarPartNames.BUFFER_CHAIN_RIG.Equals(child.name) )
                {
                    // front or rear chain rig
                    // determine whether front or rear chain rig: +z is front
                    child.localPosition = (child.localPosition.z > 0) ? FrontCouplerPosition : RearCouplerPosition;
                }
                else if( CarPartNames.BUFFER_PLATE_FRONT.Equals(child.name) )
                {
                    // front hook plate
                    child.localPosition = FrontCouplerPosition + CarPartOffset.HOOK_PLATE_F;
                }
                else if( CarPartNames.BUFFER_PLATE_REAR.Equals(child.name) )
                {
                    // rear hook plate
                    child.localPosition = RearCouplerPosition + CarPartOffset.HOOK_PLATE_R;
                }
                else if( CarPartNames.BUFFER_FRONT_PADS.Contains(child.name) )
                {
                    // front buffer pads
                    Vector3 xShiftBase = new Vector3(child.localPosition.x, FrontCouplerPosition.y, FrontCouplerPosition.z);
                    child.localPosition = xShiftBase + CarPartOffset.BUFFER_PAD_F;
                }
                else if( CarPartNames.BUFFER_REAR_PADS.Contains(child.name) )
                {
                    // rear buffer pads
                    Vector3 xShiftBase = new Vector3(child.localPosition.x, RearCouplerPosition.y, RearCouplerPosition.z);
                    child.localPosition = xShiftBase + CarPartOffset.BUFFER_PAD_R;
                }
            }

            // Couplers
            GameObject frontCoupler = basePrefab.transform.Find(CarPartNames.COUPLER_FRONT).gameObject;
            copiedObject = Object.Instantiate(frontCoupler, newFab.transform);
            copiedObject.name = CarPartNames.COUPLER_FRONT;
            copiedObject.transform.localPosition = FrontCouplerPosition + CarPartOffset.COUPLER_FRONT;

            GameObject rearCoupler = basePrefab.transform.Find(CarPartNames.COUPLER_REAR).gameObject;
            copiedObject = Object.Instantiate(rearCoupler, newFab.transform);
            copiedObject.name = CarPartNames.COUPLER_REAR;
            copiedObject.transform.localPosition = RearCouplerPosition + CarPartOffset.COUPLER_REAR;

            // Name Plates
            // These should be found when the traincar script initializes them

            #region Bogies

            Bogie frontBogie, rearBogie;

            // Front bogie
            if( FrontBogieReplacement == null )
            {
                // need to steal the original bogie
                GameObject origBogie = baseCar.Bogies[0].gameObject;
                copiedObject = Object.Instantiate(origBogie, newFab.transform);
                copiedObject.transform.localPosition = FrontBogiePosition;

                frontBogie = copiedObject.GetComponent<Bogie>();
            }
            else
            {
                // replacing the original bogie, only steal the script
                Transform newBogieTransform = newFab.transform.Find(FrontBogieReplacement);
                //Bogie origbogie = baseCar.Bogies[0];
                //frontBogie = Object.Instantiate(origbogie, newBogieTransform);
                frontBogie = newBogieTransform.gameObject.AddComponent<Bogie>();
            }

            if( FrontBogieConfig != null )
            {
                FrontBogieConfig.ApplyToBogie(frontBogie);
            }

            // Rear bogie
            if( RearBogieReplacement == null )
            {
                // steal original bogie
                GameObject origBogie = baseCar.Bogies.Last().gameObject;
                copiedObject = Object.Instantiate(origBogie, newFab.transform);
                copiedObject.transform.localPosition = RearBogiePosition;

                rearBogie = copiedObject.GetComponent<Bogie>();
            }
            else
            {
                // use bogie from new prefab
                Transform newBogieTransform = newFab.transform.Find(RearBogieReplacement);
                //Bogie origBogie = baseCar.Bogies.Last();
                //rearBogie = Object.Instantiate(origBogie, newBogieTransform);
                rearBogie = newBogieTransform.gameObject.AddComponent<Bogie>();
            }

            if( RearBogieConfig != null )
            {
                RearBogieConfig.ApplyToBogie(rearBogie);
            }

            #endregion

            // Colliders
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
                Main.ModEntry.Logger.Log($"Adding default bogie colliders to {identifier}");

                GameObject bogiesRoot = new GameObject(CarPartNames.BOGIE_COLLIDERS);
                bogiesRoot.transform.parent = colliderRoot.transform;

                CapsuleCollider bogie1Collider = bogiesRoot.AddComponent<CapsuleCollider>();
                bogie1Collider.center = frontBogie.transform.localPosition;
                bogie1Collider.radius = 0.45f;
                bogie1Collider.height = 2.34f;
                bogie1Collider.direction = 0; // x-axis

                CapsuleCollider bogie2Collider = bogiesRoot.AddComponent<CapsuleCollider>();
                bogie2Collider.center = rearBogie.transform.localPosition;
                bogie2Collider.radius = 0.45f;
                bogie2Collider.height = 2.34f;
                bogie2Collider.direction = 0; // x-axis
            }

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