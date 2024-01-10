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
            public GameObject gameObjectToReplace;

            public GameObject replacePrefab;

            public GameObjectSwapData(GameObject gameObjectToReplace, GameObject replacePrefab)
            {
                this.gameObjectToReplace = gameObjectToReplace;
                this.replacePrefab = replacePrefab;
            }
        }

        [Tooltip("All of these GameObjects will be disabled on explosion")]
        public GameObject[] gameObjectsToDisable = new GameObject[0];

        [Tooltip("These will swap 2 GameObjects")]
        public GameObjectSwapData[] gameObjectSwaps = new GameObjectSwapData[0];

        [Tooltip("These will replace the material on all renderers of a GameObject")]
        public MaterialSwapData[] materialSwaps = new MaterialSwapData[0];

        [HideInInspector]
        public string JsonGoSwap = string.Empty;
        [HideInInspector]
        public string JsonMatSwap = string.Empty;

        public void OnValidate()
        {
            JsonGoSwap = JSONObject.ToJson(gameObjectSwaps);
            JsonMatSwap = JSONObject.ToJson(materialSwaps);
        }

        public void AfterImport()
        {
            if (JsonGoSwap != null)
            {
                gameObjectSwaps = JSONObject.FromJson<GameObjectSwapData[]>(JsonGoSwap);
            }

            if (JsonMatSwap != null)
            {
                materialSwaps = JSONObject.FromJson<MaterialSwapData[]>(JsonMatSwap);
            }
        }
    }
}
