using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Explosion Model Handler Proxy")]
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

        [SerializeField, HideInInspector]
        private Material[] mats = new Material[0];
        [SerializeField, HideInInspector]
        private List<GameObject> affectedGos = new List<GameObject>();
        [SerializeField, HideInInspector]
        private List<int> affectedGosLengths = new List<int>();
        [SerializeField, HideInInspector]
        private GameObject[] go2replace = new GameObject[0];
        [SerializeField, HideInInspector]
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
            var affectedGosTemp = new GameObject[length][];

            for (int i = 0; i < length; i++)
            {
                mats[i] = materialSwaps[i].swapMaterial;
                affectedGosTemp[i] = materialSwaps[i].affectedGameObjects;
            }

            SerializationUtility.SerializeJaggedArray(affectedGosTemp, out affectedGos, out affectedGosLengths);
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

            SerializationUtility.DeserializeJaggedArray(affectedGos, affectedGosLengths, out var affectedGosTemp);
            length = Mathf.Min(mats.Length, affectedGosTemp.Length);
            materialSwaps = new MaterialSwapData[length];

            for(int i = 0; i < length; i++)
            {
                materialSwaps[i] = new MaterialSwapData()
                {
                    swapMaterial = mats[i],
                    affectedGameObjects = affectedGosTemp[i]
                };
            }
        }
    }
}
