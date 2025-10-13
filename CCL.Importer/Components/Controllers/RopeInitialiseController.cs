using DV.Simulation.Controllers;
using System.Collections;
using VerletRope;

namespace CCL.Importer.Components.Controllers
{
    internal class RopeInitialiseController : ARefreshableChildrenController<RopeBehaviour>
    {
        private IEnumerator Start()
        {
            foreach (var item in entries)
            {
                item.enabled = false;
            }

            yield return WaitFor.Seconds(1);

            foreach (var item in entries)
            {
                item.solver = CouplingHoseSolverManager.Solver;
                item.enabled = true;
            }
        }
    }
}
