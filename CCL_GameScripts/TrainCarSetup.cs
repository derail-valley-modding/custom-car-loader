using System.Collections.Generic;
using System.Linq;
using CCL_GameScripts.Attributes;
using UnityEditor;
using UnityEngine;

namespace CCL_GameScripts
{
    /// <summary>
    ///     Holds all the references for setup of a TrainCar.
    /// </summary>
    public class TrainCarSetup : ComponentInitSpec
    {
        public static readonly System.Version ExporterVersion = new System.Version(1, 7);

        #region Internal

        public delegate void BringUpWindowDelegate( TrainCarSetup trainCarSetup );

        public static BringUpWindowDelegate LaunchExportWindow;
        public static BringUpWindowDelegate LaunchLocoSetupWindow;

        [ContextMenu("Prepare Car for Export")]
        private void BringUpExportWindow()
        {
            LaunchExportWindow?.Invoke(this);
        }

        [ContextMenu("Add Locomotive Parameters")]
        private void BringUpLocoSetup()
        {
            LaunchLocoSetupWindow?.Invoke(this);
        }

        #endregion

        #region TrainCar Init Spec

        public override string TargetTypeName => "TrainCar";

        public override bool IsOverrideSet( int index ) => (index == 1) && OverridePhysics;

        public override bool DestroyAfterCreation => false;

        [Header("Basic")]
        public string Identifier = "My New Car";
        public BaseTrainCarType BaseCarType;

        public GameObject InteriorPrefab;
        [ProxyField]
        public bool keepInteriorLoaded = false;
        public bool MuffleInteriorAudio = true;

        [Header("Auto Spawning (Locos Only)")]
        [InspectorName("Spawn Locations")]
        public StationYard LocoSpawnLocations = StationYard.None;
        public string TenderID;

        [Header("Cargo & Jobs")]
        public BaseCargoContainerType CargoClass = BaseCargoContainerType.None;
        [ProxyField("cargoCapacity")]
        public float CargoCapacity = 1f;
        public Sprite BookletSprite = null;
        public float FullDamagePrice = 10000f;

        [Header("Bogie Replacement")]
        public bool UseCustomFrontBogie;
        public bool UseCustomRearBogie;

        [Header("Buffer Replacement")]
        public bool UseCustomBuffers = false;
        public bool UseCustomHosePositions = false;

        [Header("Physics")]
        public bool OverridePhysics = false; // override flag 1

        [ProxyField("totalMass", 1)]
        public float TotalMass = 50000;

        [ProxyField("wheelRadius", 1)]
        public float WheelRadius = 0.459f;

        [Range(0, 1)]
        [ProxyField("bogieMassRatio", 1)]
        public float AdhesiveMassRatio = 0.5f;

        [ProxyField("bogieSpring", 1)]
        public float BogieSpring = 2e7f;

        [ProxyField("bogieDamping", 1)]
        public float BogieDamping = 5;

        #endregion

        #region Helpers

        [MethodButton(
            nameof(AlignBogieColliders),
            nameof(AddLocoSimulation),
            nameof(ExportCar))]
        [SerializeField]
        private bool editorFoldout = true;

        private const string BOGIE_COLLIDERS = CarPartNames.COLLIDERS_ROOT + "/" + CarPartNames.BOGIE_COLLIDERS;

        public CapsuleCollider GetFrontBogieCollider()
        {
            var bogies = transform.FindSafe(BOGIE_COLLIDERS);
            if (bogies)
            {
                var bogieColliders = bogies.GetComponentsInChildren<CapsuleCollider>().OrderBy(c => c.center.z);
                return bogieColliders.LastOrDefault();
            }
            return null;
        }

        public CapsuleCollider GetRearBogieCollider()
        {
            var bogies = transform.FindSafe(BOGIE_COLLIDERS);
            if (bogies)
            {
                var bogieColliders = bogies.GetComponentsInChildren<CapsuleCollider>().OrderBy(c => c.center.z);
                return bogieColliders.FirstOrDefault();
            }
            return null;
        }

        public Transform GetFrontBogie() => transform.FindSafe(CarPartNames.BOGIE_FRONT);
        public Transform GetRearBogie() => transform.FindSafe(CarPartNames.BOGIE_REAR);

        /// <summary>
        /// This will properly align the bogie colliders to the bogie along the x and z axes.
        /// </summary>
        public void AlignBogieColliders()
        {
            List<Object> objectsToUndo = new List<Object>();

            var frontCollider = GetFrontBogieCollider();
            var frontBogie = GetFrontBogie();
            if (frontCollider && frontBogie)
            {
                var frontCenter = new Vector3(0, WheelRadius, frontBogie.localPosition.z);
                frontCollider.center = frontCenter;
                frontCollider.radius = WheelRadius;
                objectsToUndo.Add(frontCollider.transform);
            }

            var rearCollider = GetRearBogieCollider();
            var rearBogie = GetRearBogie();
            if (rearCollider)
            {
                var rearCenter = new Vector3(0, WheelRadius, rearBogie.localPosition.z);
                rearCollider.center = rearCenter;
                rearCollider.radius = WheelRadius;
                objectsToUndo.Add(rearCollider.transform);
            }

            Undo.RecordObjects(objectsToUndo.ToArray(), "Undo Align Bogies");
        }

        private void ExportCar()
        {
            TrainCarValidator.ValidateExport(this);
        }

        private void AddLocoSimulation()
        {
            AddLocoParams.ShowWindow(this);
        }

        #endregion
    }
}