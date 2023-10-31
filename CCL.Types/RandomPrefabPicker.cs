using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CCL.Types
{
    public class RandomPrefabPicker : MonoBehaviour
    {
        private static System.Random s_random = new System.Random();

        public GameObject[] Prefabs = new GameObject[0];
        public float[] Weights = new float[0];

        public float ChanceX = 0.0f;
        public float ChanceY = 0.0f;
        public float ChanceZ = 0.0f;

        private void Start()
        {
            InstantiateOne();
        }

        public void ResizeArrays(int newLength)
        {
            GameObject[] prefabs = new GameObject[newLength];
            float[] weights = new float[newLength];

            for (int i = 0; i < newLength; i++)
            {
                if (i < Prefabs.Length)
                {
                    // Copy the existing values.
                    prefabs[i] = Prefabs[i];
                }
                else
                {
                    if (Prefabs.Length > 0)
                    {
                        // Copy the last value for any new slots.
                        prefabs[i] = Prefabs[Prefabs.Length - 1];
                    }
                    else
                    {
                        // No prefab.
                        prefabs[i] = null!;
                    }
                }

                // Same for the weights.
                if (i < Weights.Length)
                {
                    weights[i] = Weights[i];
                }
                else
                {
                    if (Weights.Length > 0)
                    {
                        weights[i] = Weights[Weights.Length - 1];
                    }
                    else
                    {
                        weights[i] = 1.0f;
                    }
                }
            }

            Prefabs = prefabs;
            Weights = weights;
        }

        public GameObject InstantiateOne()
        {
            if (Prefabs.Length <= 0)
            {
                Debug.Log("Prefab array is empty.");
                return null!;
            }

            if (Prefabs.Length != Weights.Length)
            {
                Debug.Log($"Array length mismatch (P:{Prefabs.Length}|W:{Weights.Length})");
                return null!;
            }

            int choice = PickOne();
            GameObject go = Instantiate(Prefabs[choice], transform);

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
            float sum = Weights.Sum();
            float result = (float)(s_random.NextDouble() * sum);

            for (int i = 0; i < Prefabs.Length; i++)
            {
                // Check if result is under the weight.
                // If not, remove the weight from it so it can be checked
                // against the next weight.
                if (result < Weights[i])
                {
                    return i;
                }

                result -= Weights[i];
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
            float sum = Weights.Sum();
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
                percents[i] = Weights[i] / sum;
            }

            return percents;
        }
    }
}
