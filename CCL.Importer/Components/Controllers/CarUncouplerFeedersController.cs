using CCL.Types;
using DV.Simulation.Controllers;

namespace CCL.Importer.Components.Controllers
{
    internal class CarUncouplerFeedersController : ARefreshableChildrenController<CarUncouplerFeederInternal>
    {
        private void Start()
        {
            var car = TrainCar.Resolve(gameObject);

            foreach (var item in entries)
            {
                switch (item.Coupler)
                {
                    case CouplerDirection.Front:
                        item.Init(car.frontCoupler);
                        goto default;
                    case CouplerDirection.Rear:
                        item.Init(car.rearCoupler);
                        goto default;
                    default:
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var item in entries)
            {
                item.Deinit();
            }
        }
    }
}
