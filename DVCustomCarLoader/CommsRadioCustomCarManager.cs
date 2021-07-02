using System;
using System.Collections;
using System.Collections.Generic;
using DV;
using DV.PointSet;
using UnityEngine;

namespace DVCustomCarLoader
{
    
    /// <summary>
    ///     Allows use of the comms radio to spawn custom cars.
    /// </summary>
    public class CommsRadioCustomCarManager : MonoBehaviour, ICommsRadioMode
    {
        private static readonly Color laserColor = new Color32(0, 238, 255, 255);
        private bool spawnWithTrackDirection = true;
        private readonly List<RailTrack> potentialTracks = new List<RailTrack>();
        private const float POTENTIAL_TRACKS_RADIUS = 200f;
        private const float MAX_DISTANCE_FROM_TRACK_POINT = 3f;
        private const float TRACK_POINT_POSITION_Y_OFFSET = -1.75f;
        private const float SIGNAL_RANGE = 100f;
        private const float INVALID_DESTINATION_HIGHLIGHTER_DISTANCE = 20f;
        private const float UPDATE_TRACKS_PERIOD = 2.5f;
        private const string MODE_NAME = "CUSTOM SPAWNER";
        private const string CONFIRM_TEXT = "confirm";
        private const string CANCEL_TEXT = "cancel";
        public Transform signalOrigin;
        public CommsRadioDisplay display;
        public Material validMaterial;
        public Material invalidMaterial;
        public ArrowLCD lcdArrow;
        [Header("Sounds")] public AudioClip spawnModeEnterSound;
        public AudioClip spawnVehicleSound;
        public AudioClip confirmSound;
        public AudioClip cancelSound;
        [Header("Highlighters")] public GameObject destinationHighlighterGO;
        public GameObject directionArrowsHighlighterGO;
        private CarDestinationHighlighter destHighlighter;
        private RaycastHit hit;
        private LayerMask trackMask;
        private int selectedCarTypeIndex;
        private CustomCar carToSpawn;
        private Bounds carBounds;
        private bool canSpawnAtPoint;
        private RailTrack destinationTrack;
        private EquiPointSet.Point? closestPointOnDestinationTrack;
        private Coroutine trackUpdateCoro;
        private State state;

    
        
        private void SetState(State newState)
        {
            if (state == newState) return;
            state = newState;
            switch (state)
            {
                case State.EnterSpawnMode:
                    SetStartingDisplay();
                    lcdArrow.TurnOff();
                    ButtonBehaviour = ButtonBehaviourType.Regular;
                    break;
                case State.PickCar:
                    if (trackUpdateCoro == null)
                        trackUpdateCoro = StartCoroutine(PotentialTracksUpdateCoro());
                    ButtonBehaviour = ButtonBehaviourType.Override;
                    CommsRadioController.PlayAudioFromRadio(spawnModeEnterSound, transform);
                    break;
                case State.PickDestination:
                    ButtonBehaviour = ButtonBehaviourType.Override;
                    break;
            }
        }

        public void Setup()
        {
            if (!(bool) signalOrigin)
            {
                Debug.LogError("signalOrigin on CommsRadioCrewVehicle isn't set, using this.transform!",
                    this);
                signalOrigin = transform;
            }

            if (display == null)
                Debug.LogError("display not set, can't function properly!", this);
            if (validMaterial == null ||
                invalidMaterial == null)
                Debug.LogError("Some of the required materials isn't set. Visuals won't be correct.",
                    this);
            if (lcdArrow == null)
                Debug.LogError("lcdArrow not set, can't display arrow!", this);
            if (destinationHighlighterGO == null ||
                directionArrowsHighlighterGO == null)
                Debug.LogError(
                    "destinationHighlighterGO or directionArrowsHighlighterGO is not set, can't function properly!!",
                    this);
            if (spawnVehicleSound == null ||
                spawnModeEnterSound == null || confirmSound == null || cancelSound == null)
                Debug.LogError("Not all audio clips set, some sounds won't be played!",
                    this);
            trackMask = LayerMask.GetMask("Default");
            destHighlighter =
                new CarDestinationHighlighter(destinationHighlighterGO, directionArrowsHighlighterGO);
        }

        private void OnDestroy()
        {
            if (AppQuitWatcher.isQuitting) return;
            destHighlighter.Destroy();
            destHighlighter = null;
        }

        public ButtonBehaviourType ButtonBehaviour { get; private set; }

        public void Enable()
        {
        }

        public void Disable()
        {
            ClearFlags();
            trackUpdateCoro = null;
            StopAllCoroutines();
        }

        public void OverrideSignalOrigin(Transform signalOrigin)
        {
            this.signalOrigin = signalOrigin;
        }

        public void OnUse()
        {
            switch (state)
            {
                case State.EnterSpawnMode:
                    SetCarToSpawn(Main.CustomCarManagerInstance.CustomCarsToSpawn[selectedCarTypeIndex]);
                    SetState(State.PickCar);
                    break;
                case State.PickCar:
                    if (carToSpawn == null)
                    {
                        Debug.LogError("carPrefabToSpawn is null! Something is wrong, can't spawn this car.",
                            this);
                        ClearFlags();
                        break;
                    }

                    if (canSpawnAtPoint)
                    {
                        SetState(State.PickDestination);
                        CommsRadioController.PlayAudioFromRadio(confirmSound, transform);
                        break;
                    }

                    ClearFlags();
                    CommsRadioController.PlayAudioFromRadio(cancelSound, transform);
                    break;
                case State.PickDestination:
                    if (canSpawnAtPoint)
                    {
                        var forward = closestPointOnDestinationTrack.Value.forward;
                        if (!spawnWithTrackDirection) forward = -forward;

                        //bool poolingBackup = CarSpawner.useCarPooling;
                        //CarSpawner.useCarPooling = false; // need to bypass pooling to force it to use our prefab

                        //var trainCar = CarSpawner.SpawnCar(carPrefabToSpawn, destinationTrack,
                        //    (Vector3) closestPointOnDestinationTrack.Value.position + WorldMover.currentMove,
                        //    forward, true);
                        var trainCar = carToSpawn.SpawnCar(destinationTrack,
                            (Vector3)closestPointOnDestinationTrack.Value.position + WorldMover.currentMove,
                            forward, true);

                        //CarSpawner.useCarPooling = poolingBackup; // set this back to normal value

                        if( trainCar != null)
                        {
                            SingletonBehaviour<UnusedTrainCarDeleter>.Instance.MarkForDelete(trainCar);
                            CommsRadioController.PlayAudioFromCar(spawnVehicleSound, trainCar);
                            CommsRadioController.PlayAudioFromRadio(confirmSound, transform);
                            canSpawnAtPoint = false;
                            //Main.CustomCarManagerInstance.CustomCarsToSpawn[selectedCarTypeIndex].Spawn(trainCar);
                            break;
                        }

                        Debug.LogError("Couldn't spawn car!", carToSpawn.CarPrefab);
                        ClearFlags();
                        break;
                    }

                    ClearFlags();
                    CommsRadioController.PlayAudioFromRadio(cancelSound, transform);
                    break;
            }
        }

        public void OnUpdate()
        {
            if (potentialTracks.Count == 0 || (uint) (state - 1) > 1U) return;
            if (Physics.Raycast(signalOrigin.position, signalOrigin.forward, out hit, 100f,
                trackMask))
            {
                var point = hit.point;
                foreach (var potentialTrack in potentialTracks)
                {
                    var rangeWithYoffset =
                        RailTrack.GetPointWithinRangeWithYOffset(potentialTrack, point, 3f, -1.75f);
                    if (rangeWithYoffset.HasValue)
                    {
                        destinationTrack = potentialTrack;
                        var startingFromIndex =
                            CarSpawner.FindClosestValidPointForCarStartingFromIndex(
                                potentialTrack.GetPointSet().points, rangeWithYoffset.Value.index,
                                carBounds.extents);
                        var hasValue = startingFromIndex.HasValue;
                        closestPointOnDestinationTrack = !hasValue ? rangeWithYoffset : startingFromIndex;
                        canSpawnAtPoint = hasValue;
                        var position = (Vector3) closestPointOnDestinationTrack.Value.position +
                                       WorldMover.currentMove;
                        var forward = closestPointOnDestinationTrack.Value.forward;
                        if (!spawnWithTrackDirection) forward *= -1f;
                        destHighlighter.Highlight(position, forward, carBounds,
                            canSpawnAtPoint ? validMaterial : invalidMaterial);
                        display.SetAction(canSpawnAtPoint ? "confirm" : "cancel");
                        if (canSpawnAtPoint && state == State.PickDestination)
                        {
                            UpdateLCDRerailDirectionArrow();
                            return;
                        }

                        lcdArrow.TurnOff();
                        return;
                    }
                }
            }

            canSpawnAtPoint = false;
            destinationTrack = null;
            destHighlighter.Highlight(signalOrigin.position + signalOrigin.forward * 20f,
                signalOrigin.right, carBounds, invalidMaterial);
            display.SetAction("cancel");
            lcdArrow.TurnOff();
        }

        public bool ButtonACustomAction()
        {
            switch (state)
            {
                case State.PickCar:
                    selectedCarTypeIndex = selectedCarTypeIndex <= 0
                        ? Main.CustomCarManagerInstance.CustomCarsToSpawn.Count - 1
                        : selectedCarTypeIndex - 1;
                    SetCarToSpawn(Main.CustomCarManagerInstance.CustomCarsToSpawn[selectedCarTypeIndex]);
                    return true;
                case State.PickDestination:
                    if (!canSpawnAtPoint) return false;
                    spawnWithTrackDirection = !spawnWithTrackDirection;
                    return true;
                default:
                    Debug.LogError(string.Format("Unexpected state {0}!", state),
                        this);
                    return false;
            }
        }

        public bool ButtonBCustomAction()
        {
            switch (state)
            {
                case State.PickCar:
                    selectedCarTypeIndex = (selectedCarTypeIndex + 1) % Main.CustomCarManagerInstance.CustomCarsToSpawn.Count;
                    SetCarToSpawn(Main.CustomCarManagerInstance.CustomCarsToSpawn[selectedCarTypeIndex]);
                    return true;
                case State.PickDestination:
                    if (!canSpawnAtPoint) return false;
                    spawnWithTrackDirection = !spawnWithTrackDirection;
                    return true;
                default:
                    Debug.LogError(string.Format("Unexpected state {0}!", state),
                        this);
                    return false;
            }
        }

        public void SetStartingDisplay()
        {
            display.SetDisplay("CUSTOM CAR SPAWNER", "Enable custom car spawner?");
        }

        public Color GetLaserBeamColor()
        {
            return laserColor;
        }

        private void SetCarToSpawn(CustomCar car)
        {
            //carPrefabToSpawn = CarTypes.GetCarPrefab(car.TrainCarType);
            carToSpawn = car;

            if( carToSpawn == null)
            {
                Debug.LogError(
                    $"Couldn't load car prefab: {car.identifier}! Won't be able to spawn this car.", this);
            }
            else
            {
                //var component = carPrefabToSpawn.GetComponent<TrainCar>();
                carBounds = car.Bounds;
                display.SetContent(car.identifier + "\n" + car.InterCouplerDistance.ToString("F") + "m");
            }
        }

        private void UpdatePotentialTracks()
        {
            potentialTracks.Clear();
            var num = 1f;
            while (true)
            {
                foreach (var allTrack in RailTrackRegistry.AllTracks)
                    if (RailTrack.GetPointWithinRangeWithYOffset(allTrack, transform.position, num * 200f)
                        .HasValue)
                        potentialTracks.Add(allTrack);

                if (potentialTracks.Count <= 0 && num <= 4.0)
                {
                    Debug.LogWarning(
                        string.Format("No tracks in {0} radius. Expanding radius!",
                            (float) (num * 200.0)), this);
                    num += 0.2f;
                }
                else
                {
                    break;
                }
            }

            if (potentialTracks.Count != 0) return;
            Debug.LogError("No near tracks found. Can't spawn crew vehicle");
        }

        private IEnumerator PotentialTracksUpdateCoro()
        {
            var CommsRadioCustomCarManager = this;
            var lastUpdatedTracksWorldPosition = Vector3.positiveInfinity;
            while (true)
            {
                if ((CommsRadioCustomCarManager.transform.position - WorldMover.currentMove -
                     lastUpdatedTracksWorldPosition).magnitude > 100.0)
                {
                    CommsRadioCustomCarManager.UpdatePotentialTracks();
                    lastUpdatedTracksWorldPosition =
                        CommsRadioCustomCarManager.transform.position - WorldMover.currentMove;
                }

                yield return WaitFor.Seconds(2.5f);
            }
        }

        private void UpdateLCDRerailDirectionArrow()
        {
            lcdArrow.TurnOn(Mathf.Sin(
                                Vector3.SignedAngle(
                                    spawnWithTrackDirection
                                        ? closestPointOnDestinationTrack.Value.forward
                                        : -closestPointOnDestinationTrack.Value.forward,
                                    signalOrigin.forward, Vector3.up) * ((float) Math.PI / 180f)) > 0.0);
        }

        private void ClearFlags()
        {
            destinationTrack = null;
            canSpawnAtPoint = false;
            destHighlighter.TurnOff();
            SetState(State.EnterSpawnMode);
        }

        private enum State
        {
            EnterSpawnMode,
            PickCar,
            PickDestination
        }
    }
}