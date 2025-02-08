using DV.Simulation.Cars;

namespace CCL.Importer.Components.Headlights
{
    internal class FrontConnectedDualCarAutomaticHeadlightsControllerInternal : AutomaticHeadlightsController
    {
        protected override bool HoseConnected(bool front)
        {
            Coupler? coupler;

            if (!front)
            {
                coupler = trainCar.rearCoupler;
            }
            else
            {
                Coupler frontCoupler = trainCar.frontCoupler;
                if (frontCoupler == null)
                {
                    return false;
                }

                Coupler coupledTo = frontCoupler.coupledTo;
                if (coupledTo == null)
                {
                    return false;
                }

                TrainCar train = coupledTo.train;
                coupler = train != null ? train.frontCoupler : null;
            }

            if (coupler != null && coupler.hoseAndCock != null)
            {
                return coupler.hoseAndCock.IsHoseConnected;
            }

            return false;
        }
    }
}
