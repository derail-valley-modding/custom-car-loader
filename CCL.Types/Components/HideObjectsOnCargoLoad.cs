using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Hide Objects On Cargo Load")]
    public class HideObjectsOnCargoLoad : MonoBehaviour
    {
        public GameObject[] Objects = new GameObject[0];
    }
}
