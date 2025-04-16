using UnityEngine;

namespace CCL.Types.Components.MultipleUnit
{
    public abstract class MultipleUnitExtraControl<T> : MonoBehaviour
        where T : MultipleUnitExtraControl<T> { }
}
