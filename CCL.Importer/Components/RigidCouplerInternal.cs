using CCL.Types;
using DV.ThingTypes;
using System.Collections;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class RigidCouplerInternal : MonoBehaviour
    {
        private Coupler _coupler = null!;
        private Coroutine? _enstrongCoro;

        public CouplerDirection Direction;
        public bool AlwaysRigid = false;
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];

        private void Start()
        {
            _coupler = transform.Find(Direction == CouplerDirection.Front ? "[coupler front]" : "[coupler rear]").GetComponent<Coupler>();

            if (!_coupler)
            {
                Debug.LogError("RigidCouplerInternal couldn't find Coupler component during init", this);
                return;
            }

            SetupListeners(true);
        }

        private void SetupListeners(bool on)
        {
            if (on)
            {
                _coupler.Coupled += OnCoupled;
                _coupler.Uncoupled += OnUncouple;
            }
            else
            {
                _coupler.Coupled -= OnCoupled;
                _coupler.Uncoupled -= OnUncouple;
            }
        }

        private void OnUncouple(object _, UncoupleEventArgs e)
        {
            if (_enstrongCoro != null)
            {
                StopCoroutine(_enstrongCoro);
            }

            _enstrongCoro = null;
        }

        private void OnCoupled(object _, CoupleEventArgs e)
        {
            if (MeetsConditions(e.otherCoupler.train.carLivery))
            {
                Coupler coupler = (e.thisCoupler.rigidCJ != null) ? e.thisCoupler : e.otherCoupler;

                if (coupler == null)
                {
                    Debug.LogError("RigidCouplerInternal couldn't find Coupler component during coupling", this);
                    return;
                }

                if (coupler.rigidCJ == null)
                {
                    Debug.LogError("RigidCouplerInternal couldn't find the ConfigurableJoint during coupling", this);
                    return;
                }

                if (_enstrongCoro != null)
                {
                    StopCoroutine(_enstrongCoro);
                }
                
                _enstrongCoro = StartCoroutine(EnstrongJoints(coupler));
            }
        }

        private IEnumerator EnstrongJoints(Coupler coupler)
        {
            while (coupler.IsJointAdaptationActive)
            {
                yield return WaitFor.Seconds(0.5f);
            }

            coupler.rigidCJ.xMotion = ConfigurableJointMotion.Locked;
            coupler.rigidCJ.yMotion = ConfigurableJointMotion.Locked;
            coupler.rigidCJ.zMotion = ConfigurableJointMotion.Locked;
            coupler.rigidCJ.autoConfigureConnectedAnchor = true;
        }

        private bool MeetsConditions(TrainCarLivery car)
        {
            if (AlwaysRigid) return true;

            foreach (var item in CarKinds)
            {
                if (car.parentType.kind.id == item) return true;
            }

            foreach (var item in CarTypes)
            {
                if (car.parentType.id == item) return true;
            }

            foreach (var item in CarLiveries)
            {
                if (car.id == item) return true;
            }

            return false;
        }
    }
}
