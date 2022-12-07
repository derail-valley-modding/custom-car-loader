using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.Effects
{
    public class ExtendedCabTeleportDestination : MonoBehaviour, ITeleportDestination, IPointable
    {
        public Renderer[] HoverRenderers;
        protected TrainCar car;
        protected bool hovered;

        #region Events

        protected void Awake()
        {
            car = TrainCar.Resolve(gameObject);
            if (car == null)
            {
                Main.Error("Car couldn't be found on ExtendedCabTeleportDestination! Text display won't work properly.");
            }

            HoverRenderers = GetComponentsInChildren<Renderer>(true);
        }

        protected void OnEnable()
        {
            if (GamePreferences.Get<bool>(Preferences.HighlightCabToggle))
            {
                foreach (var renderer in HoverRenderers)
                {
                    renderer.enabled = true;
                }
            }
        }

        protected void OnDisable()
        {
            foreach (var renderer in HoverRenderers)
            {
                renderer.enabled = false;
            }
        }

        protected void LateUpdate()
        {
            if (!hovered)
            {
                enabled = false;
            }
            hovered = false;
        }

        #endregion

        #region IPointable

        public void Hover(Vector3 point, Vector3 normal)
        {
            hovered = true;
            enabled = true;
            if (!VRManager.IsVREnabled() && !(PlayerManager.Car == car))
            {
                InteractionTextControllerNonVr.Instance.DisplayText(GetHoverInfo());
            }
        }

        public void Unhover() { }

        public InteractionInfoType GetHoverInfo()
        {
            return InteractionInfoType.TeleportToTrain;
        }

        #endregion

        #region ITeleportDestination

        public bool IsTeleportAllowed() => true;

        public bool ShouldRotatePlayerOnTeleport() => true;

        public Vector3 GetTeleportPosition() => transform.position;

        public Quaternion GetTeleportRotation() => transform.rotation;

        #endregion
    }
}
