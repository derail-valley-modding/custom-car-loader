using DV;
using DV.UI;
using DV.UIFramework;
using System.Collections;

namespace CCL.Importer
{
    internal static class PopupHelper
    {
        private static PopupManager? s_manager;
        private static PopupNotificationReferences? s_references;

        public static PopupManager Manager => Extensions.GetCached(ref s_manager, () =>
            ACanvasController<CanvasController.ElementType>.Instance.PopupManager);
        public static PopupNotificationReferences PopupReferences => Extensions.GetCached(ref s_references, () =>
            ACanvasController<CanvasController.ElementType>.Instance.uiReferences);
        public static bool CanvasBlockers => ACanvasController<CanvasController.ElementType>.Instance.IsOn(CanvasController.ElementType.Blockers);

        public static void ShowOk(string message, PopupClosedDelegate? onClose = null, string? title = "", string? positive = "Ok")
        {
            ShowPopup(PopupReferences.popupOk,
                new PopupLocalizationKeys
                {
                    titleKey = title,
                    labelKey = message,
                    positiveKey = positive
                },
                onClose);
        }

        private static void ShowPopup(Popup prefab, PopupLocalizationKeys keys, PopupClosedDelegate? onClose = null)
        {
            CoroutineManager.Instance.Run(Coro(prefab, keys, onClose));
        }

        private static IEnumerator Coro(Popup prefab, PopupLocalizationKeys keys, PopupClosedDelegate? onClose)
        {
            while (AppUtil.Instance.IsTimePaused || !Manager.CanShowPopup()) yield return null;

            Popup popup = Manager.ShowPopup(prefab, keys, keepLiteralData: true);

            if (onClose != null)
            {
                popup.Closed += onClose;
            }
        }
    }
}
