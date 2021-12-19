using System.Collections.Generic;
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
        public bool ReplaceBaseType = false;
        public BaseCargoContainerType CargoClass = BaseCargoContainerType.None;

        public GameObject InteriorPrefab;
        public Sprite BookletSprite = null;

        public float FullDamagePrice = 10000f;

        [Header("Bogie Replacement")]
        public Transform FrontBogie;
        public Transform RearBogie;
        public bool UseCustomFrontBogie;
        public bool UseCustomRearBogie;
        public CapsuleCollider FrontBogieCollider;
        public CapsuleCollider RearBogieCollider;

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
        public float MassRatioPerBogie = 0.5f;

        [ProxyField("bogieSpring", 1)]
        public float BogieSpring = 2e7f;

        [ProxyField("bogieDamping", 1)]
        public float BogieDamping = 5;

        #endregion

        #region Helpers

        [MethodButton(
            nameof(CreateAssetBundleForTrainCar),
            nameof(AlignBogieColliders),
            nameof(AddLocoSimulation),
            nameof(ExportCar))]
        [SerializeField]
        private bool editorFoldout = true;

        public void CreateAssetBundleForTrainCar()
        {
            string assetPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Asset path is null! Make sure the TrainCar is a prefab!");
                return;
            }
        
            //Change name of asset bundle to this GameObject.
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(name, "");
        
            //Remove unused assetBundle names.
            AssetDatabase.RemoveUnusedAssetBundleNames();
        
            EditorUtility.DisplayDialog("Created AssetBundle",
                $"An AssetBundle with the name {name} was created successfully.", "OK");
        }

        /// <summary>
        /// This will properly align the bogie colliders to the bogie along the x and z axes.
        /// </summary>
        public void AlignBogieColliders()
        {
            List<Object> objectsToUndo = new List<Object>();

            if( FrontBogieCollider )
            {
                //var frontCenter = FrontBogieCollider.center;
                var frontCenter = new Vector3(0, WheelRadius, FrontBogie.localPosition.z);
                FrontBogieCollider.center = frontCenter;
                FrontBogieCollider.radius = WheelRadius;
                objectsToUndo.Add(FrontBogieCollider.transform);
            }

            if( RearBogieCollider )
            {
                //var rearCenter = RearBogieCollider.center;
                var rearCenter = new Vector3(0, WheelRadius, RearBogie.localPosition.z);
                RearBogieCollider.center = rearCenter;
                RearBogieCollider.radius = WheelRadius;
                objectsToUndo.Add(RearBogieCollider.transform);
            }

            Undo.RecordObjects(objectsToUndo.ToArray(), "Undo Align Bogies");
        }

        private void ExportCar()
        {
            ExportTrainCar.ShowWindow(this);
        }

        private void AddLocoSimulation()
        {
            AddLocoParams.ShowWindow(this);
        }

        #endregion

        #region Gizmos

    //private void OnDrawGizmos()
    //{
    //    #region Coupler Gizmos
    //    if (FrontCoupler != null) Gizmos.DrawWireCube(FrontCoupler.position, new Vector3(0.3f, 0.3f, 0.3f));
    //    if (RearCoupler != null) Gizmos.DrawWireCube(RearCoupler.position, new Vector3(0.3f, 0.3f, 0.3f));
    //    #endregion

    //    #region Bogie Gizmos

    //    //if (FrontBogieCollider != null)
    //    //{
    //    //    var frontBogiePos = FrontBogieCollider.transform.position + FrontBogieCollider.transform.TransformPoint(FrontBogieCollider.bounds.center);
    //    //    Gizmos.DrawWireCube(frontBogiePos, FrontBogieCollider.bounds.size);
    //    //}

    //    //if (RearBogieCollider != null)
    //    //{
    //    //    var rearBogiePos = RearBogieCollider.transform.position + RearBogieCollider.transform.TransformPoint(RearBogieCollider.bounds.center);
    //    //    Gizmos.DrawWireCube(rearBogiePos, RearBogieCollider.bounds.size);
    //    //}

    //    #endregion
    //}

        #endregion
    }
}