using CCL.Types.Components.HUD;
using DV.CabControls;
using DV.HUD;
using UnityEngine;

namespace CCL.Importer.Components.HUD
{
    internal class ExtraInteriorControlsManager : MonoBehaviour
    {
        public InteriorControlsManager Manager = null!;

        public InteriorControlsManager.ControlReference? Alerter;
        public InteriorControlsManager.ControlReference? CabOrient;
        public InteriorControlsManager.ControlReference? Pantograph;

        private void Setup()
        {
            SetupExtraControlReader(GetComponent<ExtraLocoControlsReader>());
            SetupExtraControlReader(Manager.Car.loadedExternalInteractables?.GetComponent<ExtraLocoControlsReader>());
        }

        private void SetupExtraControlReader(ExtraLocoControlsReader? reader)
        {
            if (reader == null) return;

            Alerter = SetupRef(reader.Alerter);
            CabOrient = SetupRef(reader.CabOrient);
            Pantograph = SetupRef(reader.Pantograph);

            static InteriorControlsManager.ControlReference? SetupRef(GameObject? go)
            {
                if (go != null && go.TryGetComponent(out ControlImplBase impl))
                {
                    return new InteriorControlsManager.ControlReference()
                    {
                        controlImplBase = impl,
                        scrollable = impl.GetComponent<IScrollable>()
                    };
                }

                return null;
            }
        }

        public static ExtraInteriorControlsManager GetOrAddToManager(InteriorControlsManager manager)
        {
            if (!manager.TryGetComponent(out ExtraInteriorControlsManager extra))
            {
                extra = manager.gameObject.AddComponent<ExtraInteriorControlsManager>();
                extra.Manager = manager;
                extra.Setup();
            }

            return extra;
        }
    }
}
