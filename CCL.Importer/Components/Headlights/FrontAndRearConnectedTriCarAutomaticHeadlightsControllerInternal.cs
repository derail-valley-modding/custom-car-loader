using DV.Simulation.Cars;

namespace CCL.Importer.Components.Headlights
{
    internal class FrontAndRearConnectedTriCarAutomaticHeadlightsControllerInternal : AutomaticHeadlightsController
    {
        protected override bool HoseConnected(bool front)
        {
            Coupler? coupler;

            if (front)
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
            else
            {
                Coupler rearCoupler = trainCar.rearCoupler;
                if (rearCoupler == null)
                {
                    return false;
                }

                Coupler coupledTo = rearCoupler.coupledTo;
                if (coupledTo == null)
                {
                    return false;
                }

                TrainCar train = coupledTo.train;
                coupler = train != null ? train.rearCoupler : null;
            }

            if (coupler != null && coupler.hoseAndCock != null)
            {
                return coupler.hoseAndCock.IsHoseConnected;
            }

            return false;
        }
    }
}
