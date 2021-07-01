using System;
using System.Linq;
using UnityEngine;
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
        public TrainCarType TrainCarType = TrainCarType.FlatbedEmpty;

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

        //Chains
        public Vector3 FrontChainPosition;
        public Vector3 RearChainPosition;

        //Hoses
        public Vector3 FrontHosePosition;
        public Vector3 RearHosePosition;
        
        //Buffers
        public Vector3 FrontBufferPosition;
        public Vector3 RearBufferPosition;

        //Name Plates
        public Vector3 SidePlate1Position;
        public Vector3 SidePlate2Position;
        
        public void Spawn(TrainCar trainCar)
        {

            #region Spawn Car

            if( CarPrefab == null )
            {
                Main.ModEntry.Logger.Warning($"{identifier} has a missing prefab");
            }

#if DEBUG
            Main.ModEntry.Logger.Log($"Adding custom model {identifier} to traincar");
#endif
            var newCar = Object.Instantiate(CarPrefab, trainCar.transform.root, true);
            newCar.SetActive(true);
            newCar.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            newCar.transform.localRotation = Quaternion.identity;
            
            //Deactivate underlying train car model
            switch (TrainCarType)
            {
                case TrainCarType.NotSet:
                    throw new ArgumentOutOfRangeException();
                case TrainCarType.LocoShunter:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.LocoSteamHeavy:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.Tender:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.LocoSteamHeavyBlue:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.TenderBlue:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.LocoRailbus:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.LocoDiesel:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.FlatbedEmpty:
                case TrainCarType.FlatbedStakes:
                case TrainCarType.FlatbedMilitary:
                    trainCar.transform.Find("car_flatcar_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.AutorackRed:
                case TrainCarType.AutorackBlue:
                case TrainCarType.AutorackGreen:
                case TrainCarType.AutorackYellow:
                    trainCar.transform.Find("car_autorack_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.TankOrange:
                case TrainCarType.TankWhite:
                case TrainCarType.TankYellow:
                case TrainCarType.TankBlue:
                case TrainCarType.TankChrome:
                case TrainCarType.TankBlack:
                    trainCar.transform.Find("car_tanker_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.BoxcarBrown:
                case TrainCarType.BoxcarGreen:
                case TrainCarType.BoxcarPink:
                case TrainCarType.BoxcarRed:
                    trainCar.transform.Find("car_boxcar_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.BoxcarMilitary:
                    trainCar.transform.Find("CarMilitaryBoxcar_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.RefrigeratorWhite:
                    trainCar.transform.Find("car_refrigerated_boxcar_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.HopperBrown:
                case TrainCarType.HopperTeal:
                case TrainCarType.HopperYellow:
                    trainCar.transform.Find("car_hopper_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.PassengerRed:
                case TrainCarType.PassengerGreen:
                case TrainCarType.PassengerBlue:
                    trainCar.transform.Find("car_passenger_lod").gameObject.SetActive(false);
                    break;
                case TrainCarType.HandCar:
                    Debug.LogError("Not supported.");
                    break;
                case TrainCarType.NuclearFlask:
                    trainCar.transform.Find("CarNuclearFlask").gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion

            #region Bogies

            // Front Bogie
            Bogie bogieF = trainCar.Bogies[0];
            bogieF.gameObject.SetActive(false);
            
            if( (FrontBogieReplacement != null) && (newCar.transform.Find(FrontBogieReplacement) is Transform frontBogieTransform) )
            {
#if DEBUG
                Main.ModEntry.Logger.Log($"Replacing front bogie on {identifier}");
#endif
                // custom bogie object
                // we'll copy the bogie component from the original prefab
                bogieF = Object.Instantiate(bogieF, frontBogieTransform);
                if( FrontBogieConfig != null )
                {
                    // replace parameters
                    FrontBogieConfig.ApplyToBogie(bogieF);
                }
            }
            else
            {
                // use existing bogie
                bogieF.transform.localPosition = FrontBogiePosition;
                bogieF.gameObject.SetActive(true);
            }

            // Rear Bogie
            int bogie2idx = trainCar.Bogies.Length - 1;
            Bogie bogieR = trainCar.Bogies[bogie2idx];
            bogieR.gameObject.SetActive(false);

            if( (RearBogieReplacement != null) && (newCar.transform.Find(RearBogieReplacement) is Transform rearBogieTransform) )
            {
#if DEBUG
                Main.ModEntry.Logger.Log($"Replacing rear bogie on {identifier}");
#endif
                bogieR = Object.Instantiate(bogieR, rearBogieTransform);
                if( RearBogieConfig != null )
                {
                    RearBogieConfig.ApplyToBogie(bogieR);
                }
            }
            else
            {
                // use existing bogie
                bogieR.transform.localPosition = RearBogiePosition;
                bogieR.gameObject.SetActive(true);
            }

            #endregion
            
            #region Couplers
            
            var frontCoupler = trainCar.frontCoupler;
            var rearCoupler = trainCar.rearCoupler;
            
            if (FrontCouplerPosition != Vector3.zero)
            {
                frontCoupler.transform.localPosition = FrontCouplerPosition;
            }

            if (RearCouplerPosition != Vector3.zero)
            {
                rearCoupler.transform.localPosition = RearCouplerPosition;
            }
            
            #endregion

            #region Chains

            var frontChain = frontCoupler.visualCoupler.chain;
            var rearChain = rearCoupler.visualCoupler.chain;
            
            if (FrontChainPosition != Vector3.zero)
            {
                frontChain.transform.localPosition = FrontChainPosition;
            }

            if (RearChainPosition != Vector3.zero)
            {
                rearChain.transform.localPosition = RearChainPosition;
            }
            
            #endregion
            
            #region Hoses
            
            var frontHose = frontCoupler.visualCoupler.hoses;
            var rearHose = rearCoupler.visualCoupler.hoses;
            
            if (FrontHosePosition != Vector3.zero)
            {
                frontHose.localPosition = FrontHosePosition;
            }

            if (RearHosePosition != Vector3.zero)
            {
                rearHose.localPosition = RearHosePosition;
            }
            
            #endregion
            
            #region Buffers
            
            if (FrontBufferPosition != Vector3.zero)
            {
                frontCoupler.visualCoupler.transform.localPosition = FrontBufferPosition;
            }

            if (RearBufferPosition != Vector3.zero)
            {
                rearCoupler.visualCoupler.transform.localPosition = RearBufferPosition;
            }
            
            #endregion
            
            #region Name Plates
            
            var plate1 = trainCar.transform.Find("[car plate anchor1]");
            var plate2 = trainCar.transform.Find("[car plate anchor2]");

            if (SidePlate1Position != Vector3.zero && plate1)
            {
                plate1.localPosition = SidePlate1Position;
            }

            if (SidePlate2Position != Vector3.zero && plate2)
            {
                plate2.localPosition = SidePlate2Position;
            }

            #endregion

            trainCar.OnDestroyCar += Main.CustomCarManagerInstance.DeregisterCar;
            Main.CustomCarManagerInstance.RegisterSpawnedCar(trainCar, identifier);
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