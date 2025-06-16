using DV.Localization;

namespace CCL.Importer
{
    public static class Localization
    {
        public class WorkTrains
        {
            public static string PurchaseModeTitle => LocalizationAPI.L("ccl/comms/work_train_purchase_mode");
            public static string EnterSelection => LocalizationAPI.L("ccl/comms/work_train_purchase_enter");
            public static string ExitSelection => LocalizationAPI.L("ccl/comms/work_train_purchase_exit");
            public static string NoneForPurchase => LocalizationAPI.L("ccl/comms/no_work_train_available");
            public static string PurchaseComplete => LocalizationAPI.L("ccl/comms/purchase_complete");

            public static string SelectedCarDisplay(string car, float price)
            {
                return LocalizationAPI.L("ccl/comms/car_purchase_prompt", car, price.ToString("N2", LocalizationAPI.CC));
            }

            public static string PurchaseConfirm(float price)
            {
                return LocalizationAPI.L("ccl/comms/purchase_confirm_prompt", price.ToString("N2", LocalizationAPI.CC));
            }
        }
    }
}
