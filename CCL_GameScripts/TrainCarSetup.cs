using System.Collections.Generic;
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

        protected override string TargetTypeName => "TrainCar";

        protected override bool DestroyAfterCreation => false;

        [Header("Bogie Replacement")]
        public Transform FrontBogie;
        public Transform RearBogie;
        public bool ReplaceFrontBogie;
        public bool ReplaceRearBogie;
        public CapsuleCollider FrontBogieCollider;
        public CapsuleCollider RearBogieCollider;

        [Header("Bogie Physics")]
        public bool OverridePhysics = false;

        [ProxyField("wheelRadius")]
        public float WheelRadius = 0.459f;

        [Range(0, 1)]
        [ProxyField("bogieMassRatio")]
        public float MassRatioPerBogie = 0.5f;

        [ProxyField("bogieSpring")]
        public float BogieSpring = 200;

        [ProxyField("bogieDamping")]
        public float BogieDamping = 5;

        public GameObject InteriorPrefab;

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
                var frontCenter = FrontBogieCollider.center;
                frontCenter = new Vector3(0, frontCenter.y, FrontBogie.localPosition.z);
                FrontBogieCollider.center = frontCenter;
                objectsToUndo.Add(FrontBogieCollider.transform);
            }

            if( RearBogieCollider )
            {
                var rearCenter = RearBogieCollider.center;
                rearCenter = new Vector3(0, rearCenter.y, RearBogie.localPosition.z);
                RearBogieCollider.center = rearCenter;
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