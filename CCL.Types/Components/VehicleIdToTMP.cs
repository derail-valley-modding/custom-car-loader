using TMPro;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Vehicle ID To TMP")]
    public class VehicleIdToTMP : MonoBehaviour
    {
        public enum CopyId
        {
            None,
            Front,
            Rear,
            Trainset
        }

        public TMP_Text[] Texts = new TMP_Text[0];
        public string FormatString = string.Empty;
        public int Offset = 0;
        public CopyId CopyIdFrom;
        [Min(0), EnableIf(nameof(EnableIndex))]
        public int TrainsetIndex;

        private bool EnableIndex() => CopyIdFrom == CopyId.Trainset;
    }
}
