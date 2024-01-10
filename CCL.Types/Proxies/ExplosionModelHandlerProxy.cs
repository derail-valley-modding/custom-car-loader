using CCL.Types.Json;
using System;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public class ExplosionModelHandlerProxy : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class MaterialSwapData
        {
            public Material swapMaterial = null!;
            public GameObject[] affectedGameObjects = new GameObject[0];
        }

        [Serializable]
        public class GameObjectSwapData
        {
            public GameObject gameObjectToReplace = null!;
            public GameObject replacePrefab = null!;
        }

        [Tooltip("All of these GameObjects will be disabled on explosion")]
        public GameObject[] gameObjectsToDisable = new GameObject[0];

        [Tooltip("These will swap 2 GameObjects")]
        public GameObjectSwapData[] gameObjectSwaps = new GameObjectSwapData[0];

        [Tooltip("These will replace the material on all renderers of a GameObject")]
        public MaterialSwapData[] materialSwaps = new MaterialSwapData[0];

        [HideInInspector]
        [SerializeField]
        private Material[] mats = new Material[0];
        [HideInInspector]
        [SerializeField]
        private GameObject[][] affectedGos = new GameObject[0][];
        [HideInInspector]
        [SerializeField]
        private GameObject[] go2replace = new GameObject[0];
        [HideInInspector]
        [SerializeField]
        private GameObject[] replaceFabs = new GameObject[0];

        public void OnValidate()
        {
            int length = gameObjectSwaps.Length;
            go2replace = new GameObject[length];
            replaceFabs = new GameObject[length];

            for (int i = 0; i < length; i++)
            {
                go2replace[i] = gameObjectSwaps[i].gameObjectToReplace;
                replaceFabs[i] = gameObjectSwaps[i].replacePrefab;
            }

            length = materialSwaps.Length;
            mats = new Material[length];
            affectedGos = new GameObject[length][];

            for (int i = 0; i < length; i++)
            {
                mats[i] = materialSwaps[i].swapMaterial;
                affectedGos[i] = materialSwaps[i].affectedGameObjects;
            }
        }

        public void AfterImport()
        {
            int length = Mathf.Min(go2replace.Length, replaceFabs.Length);
            gameObjectSwaps = new GameObjectSwapData[length];

            for (int i = 0; i < length; i++)
            {
                gameObjectSwaps[i] = new GameObjectSwapData()
                {
                    gameObjectToReplace = go2replace[i],
                    replacePrefab = replaceFabs[i]
                };
            }

            length = Mathf.Min(mats.Length, affectedGos.Length);
            materialSwaps = new MaterialSwapData[length];

            for(int i = 0;i < length; i++)
            {
                materialSwaps[i] = new MaterialSwapData()
                {
                    swapMaterial = mats[i],
                    affectedGameObjects = affectedGos[i]
                };
            }
        }
    }
}
