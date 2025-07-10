using CCL.Types;
using System.Collections;
using UnityEngine;
using VerletRope;

namespace CCL.Importer.Components.Controllers
{
    internal class RopeBehaviourOptimiserInternal : MonoBehaviour
    {
        private const int Disabled = 0;
        private const int Slow = 1;
        private const int Regular = 2;

        public RopeBehaviour[] Ropes = new RopeBehaviour[0];
        public int FullIterations = 100;
        public int SlowIterations = 5;
        public float DistanceSlowSqr = 150.0f * 150.0f;
        public float DistanceDisableSqr = 500.0f * 500.0f;

        private int _state = 2;
        private Coroutine? _coro;

        private void OnEnable()
        {
            _coro = StartCoroutine(Routine());
        }

        private void OnDisable()
        {
            if (_coro != null)
            {
                StopCoroutine(_coro);
            }
        }

        private IEnumerator Routine()
        {
            while (true)
            {
                yield return WaitFor.Seconds(1);

                if (PlayerManager.ActiveCamera == null) continue;

                var distSqr = MathHelper.DistanceSqr(PlayerManager.ActiveCamera.transform.position, transform.position);
                int state = distSqr > DistanceDisableSqr ? Disabled : distSqr > DistanceSlowSqr ? Slow : Regular;

                if (_state == state) continue;

                switch (state)
                {
                    case Disabled:
                        foreach (var rope in Ropes)
                        {
                            rope.gameObject.SetActive(false);
                        }
                        break;
                    case Slow:
                        foreach (var rope in Ropes)
                        {
                            if (_state == Disabled)
                            {
                                rope.gameObject.SetActive(true);
                            }

                            rope.ropeParams.solverIterations = SlowIterations;
                        }
                        break;
                    case Regular:
                        foreach (var rope in Ropes)
                        {
                            if (_state == Disabled)
                            {
                                rope.gameObject.SetActive(true);
                            }

                            rope.ropeParams.solverIterations = FullIterations;
                        }
                        break;
                    default:
                        Debug.LogWarning("Unknown state in RopeBehaviourOptimiser", this);
                        continue;
                }

                _state = state;
            }
        }
    }
}
