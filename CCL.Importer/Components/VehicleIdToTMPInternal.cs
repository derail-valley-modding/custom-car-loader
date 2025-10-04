using System.Collections;
using TMPro;
using UnityEngine;

using static CCL.Types.Components.VehicleIdToTMP;

namespace CCL.Importer.Components
{
    internal class VehicleIdToTMPInternal : MonoBehaviour
    {
        public TMP_Text[] Texts = new TMP_Text[0];
        public string FormatString = string.Empty;
        public int Offset = 0;
        public CopyId CopyIdFrom;
        public int TrainsetIndex;

        private IEnumerator Start()
        {
            var car = TrainCar.Resolve(gameObject);

            if (car == null)
            {
                Debug.LogError("Could not find TrainCar for VehicleIdToTMP", this);
                yield break;
            }

            var found = false;
            var count = 0;

            while (true)
            {
                switch (CopyIdFrom)
                {
                    case CopyId.Front:
                        if (car.frontCoupler == null)
                        {
                            Debug.LogError("VehicleIdToTMP asked for front copy, but has no front coupler!", this);
                            found = true;
                        }
                        else if (car.frontCoupler.coupledTo != null)
                        {
                            found = true;
                            car = car.frontCoupler.coupledTo.train;
                        }
                        break;
                    case CopyId.Rear:
                        if (car.rearCoupler == null)
                        {
                            Debug.LogError("VehicleIdToTMP asked for rear copy, but has no rear coupler!", this);
                            found = true;
                        }
                        else if (car.rearCoupler.coupledTo != null)
                        {
                            found = true;
                            car = car.rearCoupler.coupledTo.train;
                        }
                        break;
                    case CopyId.Trainset:
                        switch (CarManager.TryGetInstancedTrainset(car, out var set))
                        {
                            case CarManager.TrainSetCompleteness.NotCCL:
                            case CarManager.TrainSetCompleteness.NotPartOfTrainset:
                                Debug.LogError("VehicleIdToTMP asked for trainset, but is not part of one!", this);
                                found = true;
                                break;
                            case CarManager.TrainSetCompleteness.Complete:
                                found = true;

                                if (TrainsetIndex < 0 || TrainsetIndex >= set.Length)
                                {
                                    Debug.LogWarning($"Trainset index in VehicleIdToTMP is out of range, clamping to range: {TrainsetIndex} - {set.Length}");
                                    TrainsetIndex = Mathf.Clamp(TrainsetIndex, 0, set.Length - 1);
                                }

                                car = set[TrainsetIndex];
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        found = true;
                        break;
                }

                if (count++ > 25)
                {
                    Debug.LogWarning("VehicleIdToTMP could not meet conditions in time, using self for ID", this);
                    break;
                }

                if (found)
                {
                    break;
                }

                yield return WaitFor.Seconds(1);
            }

            count = 0;

            while (car.logicCar == null)
            {
                yield return null;

                if (count++ > 100)
                {
                    Debug.LogError("Could not find logic car for VehicleIdToTMP", this);
                    yield break;
                }
            }

            var fullId = car.ID;
            var partId = fullId.Substring(fullId.Length - 3);
            var number = int.Parse(partId) + Offset;

            SetTexts($"{(string.IsNullOrWhiteSpace(FormatString) ? number : string.Format(FormatString, number))}");

            Destroy(this);
        }

        private void SetTexts(string text)
        {
            foreach (var t in Texts)
            {
                t.text = text;
            }
        }
    }
}
