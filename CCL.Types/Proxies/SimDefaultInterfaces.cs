using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public static class VehicleDefaultsHelper
    {
        public static IEnumerable<(string ActionName, Action Action)> GetActionsForType<T>(T defaults)
        {
            if (defaults is ILocoDefaults loco) yield return ("Apply Generic Loco Defaults", loco.ApplyLocoDefaults);

            if (defaults is IDE2Defaults de2) yield return ("Apply DE2 Defaults", de2.ApplyDE2Defaults);
            if (defaults is IDE6Defaults de6) yield return ("Apply DE6 Defaults", de6.ApplyDE6Defaults);
            if (defaults is IDH4Defaults dh4) yield return ("Apply DH4 Defaults", dh4.ApplyDH4Defaults);
            if (defaults is IDM3Defaults dm3) yield return ("Apply DM3 Defaults", dm3.ApplyDM3Defaults);
            if (defaults is IDM1UDefaults dm1u) yield return ("Apply DM1U Defaults", dm1u.ApplyDM1UDefaults);

            if (defaults is IS060Defaults s060) yield return ("Apply S060 Defaults", s060.ApplyS060Defaults);
            if (defaults is IS282Defaults s282) yield return ("Apply S282 Defaults", s282.ApplyS282Defaults);

            if (defaults is IBE2Defaults be2) yield return ("Apply BE2 Defaults", be2.ApplyBE2Defaults);

            if (defaults is IH1Defaults h1) yield return ("Apply H1 Defaults", h1.ApplyH1Defaults);

            if (defaults is IWagonDefaults wagon) yield return ("Apply Generic Wagon Defaults", wagon.ApplyWagonDefaults);
        }
    }

    /// <summary>
    /// Dummy class to easily add default buttons.
    /// </summary>
    [NotProxied]
    public abstract class MonoBehaviourWithVehicleDefaults : MonoBehaviour { }

    public interface ILocoDefaults
    {
        void ApplyLocoDefaults();
    }

    public interface IDE2Defaults
    {
        void ApplyDE2Defaults();
    }

    public interface IDE6Defaults
    {
        void ApplyDE6Defaults();
    }

    public interface IDH4Defaults
    {
        void ApplyDH4Defaults();
    }

    public interface IDM3Defaults
    {
        void ApplyDM3Defaults();
    }

    public interface IDM1UDefaults
    {
        void ApplyDM1UDefaults();
    }

    public interface IS060Defaults
    {
        void ApplyS060Defaults();
    }

    public interface IS282Defaults
    {
        void ApplyS282Defaults();
    }

    public interface IBE2Defaults
    {
        void ApplyBE2Defaults();
    }

    public interface IH1Defaults
    {
        void ApplyH1Defaults();
    }

    public interface IWagonDefaults
    {
        void ApplyWagonDefaults();
    }
}
