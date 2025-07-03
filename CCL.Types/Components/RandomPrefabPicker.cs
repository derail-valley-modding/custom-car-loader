using System;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Random Prefab Picker")]
    public class RandomPrefabPicker : MonoBehaviour, ICustomSerialized
    {
        private static System.Random s_random = new System.Random();

        public PrefabWeight[] Prefabs = new PrefabWeight[0];

        [Tooltip("The chance for the prefab to be rotated 180 degrees around the X axis"), Range(0, 1)]
        public float ChanceX = 0.0f;
        [Tooltip("The chance for the prefab to be rotated 180 degrees around the Y axis"), Range(0, 1)]
        public float ChanceY = 0.0f;
        [Tooltip("The chance for the prefab to be rotated 180 degrees around the Z axis"), Range(0, 1)]
        public float ChanceZ = 0.0f;

        [SerializeField, HideInInspector]
        private GameObject[] _prefabs = new GameObject[0];
        [SerializeField, HideInInspector]
        private float[] _weights = new float[0];

        private void Start()
        {
            AfterImport();
            InstantiateOne();
        }

        public GameObject InstantiateOne()
        {
            if (Prefabs.Length <= 0)
            {
                Debug.Log("Prefab array is empty.");
                return null!;
            }

            int choice = PickOne();
            GameObject go = Instantiate(Prefabs[choice].Prefab, transform);

            if (!go)
            {
                Debug.LogWarning($"Instanced object is null (index {choice}), check if something is wrong");
                return null!;
            }

            var (x, y, z) = GetFlips();
            go.transform.localRotation *= Quaternion.Euler(x ? 180 : 0, y ? 180 : 0, z ? 180 : 0);

            return go;
        }

        public int PickOne()
        {
            // Result is somewhere within range of the total sum of the weights.
            float sum = Prefabs.Sum(x => x.Weight);
            float result = (float)(s_random.NextDouble() * sum);

            for (int i = 0; i < Prefabs.Length; i++)
            {
                // Check if result is under the weight.
                // If not, remove the weight from it so it can be checked
                // against the next weight.
                if (result < Prefabs[i].Weight)
                {
                    return i;
                }

                result -= Prefabs[i].Weight;
            }

            // With the exclusive upper bound of NextDouble() this should never happen.
            Debug.LogWarning("Reached end of spawning loop with no spawning, selecting last prefab");
            return Prefabs.Length - 1;
        }

        public (bool X, bool Y, bool Z) GetFlips()
        {
            return (s_random.NextDouble() < ChanceX,
                s_random.NextDouble() < ChanceY,
                s_random.NextDouble() < ChanceZ);
        }

        public float[] GetPercentagesFromWeights()
        {
            if (Prefabs.Length == 0)
            {
                return new float[0];
            }

            float sum = Prefabs.Sum(x => x.Weight);
            float[] percents = new float[Prefabs.Length];

            // If there's no chance for anything, return 0% except for the
            // last, since it will always be picked (with a warning).
            if (sum == 0)
            {
                percents[percents.Length - 1] = 1.0f;
                return percents;
            }

            for (int i = 0; i < Prefabs.Length; i++)
            {
                percents[i] = Prefabs[i].Weight / sum;
            }

            return percents;
        }

        public void OnValidate()
        {
            _prefabs = new GameObject[Prefabs.Length];
            _weights = new float[Prefabs.Length];

            for (int i = 0; i < Prefabs.Length; i++)
            {
                _prefabs[i] = Prefabs[i].Prefab;
                _weights[i] = Prefabs[i].Weight;
            }
        }

        public void AfterImport()
        {
            Prefabs = new PrefabWeight[_prefabs.Length];

            for (int i = 0; i < Prefabs.Length; i++)
            {
                Prefabs[i] = new PrefabWeight()
                {
                    Prefab = _prefabs[i],
                    Weight = _weights[i]
                };
            }
        }
    }

    [Serializable]
    public class PrefabWeight
    {
        [Tooltip("The prefab to pick, and its weight for being chosen")]
        public GameObject Prefab = null!;
        [Min(0)]
        public float Weight = 1;
    }
}
