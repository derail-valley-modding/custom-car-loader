using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Catalog;
using System.Linq;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class CatalogValidator : LiveryValidator
    {
        public override string TestName => "Catalog";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var page = livery.CatalogPage;

            if (page == null || livery.parentType == null) return Skip();

            var result = Pass();
            var cargo = livery.parentType.CargoSetup;

            if (HasRole(page, VehicleRole.PassengerTransport))
            {
                if (cargo == null || !cargo.Entries.Any(x => x.CargoId == OtherMods.PassengerJobs.CARGO_ID))
                {
                    result.Warning("Catalog page has Passenger Transport role, yet vehicle does not have passenger cargo");
                }
            }

            if (HasRole(page, VehicleRole.FreightTransport))
            {
                if (cargo == null || cargo.Entries.All(x => x.CargoId == OtherMods.PassengerJobs.CARGO_ID))
                {
                    result.Warning("Catalog page has Freight Transport role, yet vehicle does not have non-passenger cargo");
                }
            }

            if (page.LoadFlat.Tonnage < page.LoadIncline.Tonnage)
            {
                result.Warning("Flat load rating is lower than 2% incline load rating");
            }

            if (page.LoadFlat.Tonnage < page.LoadInclineWet.Tonnage)
            {
                result.Warning("Flat load rating is lower than 2% wet incline load rating");
            }

            if (page.LoadIncline.Tonnage < page.LoadInclineWet.Tonnage)
            {
                result.Warning("2% incline load rating is lower than 2% wet incline load rating");
            }

            return result;
        }

        private static bool HasRole(CatalogPage page, VehicleRole role)
        {
            return page.Role1 == role || page.Role2 == role;
        }
    }
}
