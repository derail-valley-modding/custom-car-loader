using CCL.Types;
using DV;
using DV.Customization.Paint;
using DV.ServicePenalty.UI;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using LocoSim.Definitions;
using UnityEngine;

namespace CCL.Importer
{
    /// <summary>
    /// Class for quick references to certain objects.
    /// </summary>
    public static class QuickAccess
    {
        private static TrainCarLivery GetCarLivery(string id)
        {
            if (!Globals.G.Types.TryGetLivery(id, out var livery))
            {
                CCLPlugin.Error($"Failed to find car livery with ID {id}");
            }

            return livery;
        }

        private static TrainCarType_v2 GetCarType(string id)
        {
            if (!Globals.G.Types.TryGetCarType(id, out var car))
            {
                CCLPlugin.Error($"Failed to find car type with ID {id}");
            }

            return car;
        }

        /// <summary>
        /// References to locomotive liveries.
        /// </summary>
        public static class Locomotives
        {
            private static TrainCarLivery? s_de2;
            private static TrainCarLivery? s_de6;
            private static TrainCarLivery? s_dh4;
            private static TrainCarLivery? s_dm3;
            private static TrainCarLivery? s_s060;
            private static TrainCarLivery? s_s282a;
            private static TrainCarLivery? s_s282b;

            public static TrainCarLivery DE2 => Extensions.GetCached(ref s_de2, () => GetCarLivery("LocoDE2"));
            public static TrainCarLivery DE6 => Extensions.GetCached(ref s_de6, () => GetCarLivery("LocoDE6"));
            public static TrainCarLivery DH4 => Extensions.GetCached(ref s_dh4, () => GetCarLivery("LocoDH4"));
            public static TrainCarLivery DM3 => Extensions.GetCached(ref s_dm3, () => GetCarLivery("LocoDM3"));
            public static TrainCarLivery S060 => Extensions.GetCached(ref s_s060, () => GetCarLivery("LocoS060"));
            public static TrainCarLivery S282A => Extensions.GetCached(ref s_s282a, () => GetCarLivery("LocoS282A"));
            public static TrainCarLivery S282B => Extensions.GetCached(ref s_s282b, () => GetCarLivery("LocoS282B"));

            private static TrainCarLivery? s_de6slug;
            private static TrainCarLivery? s_microshunter;
            private static TrainCarLivery? s_dm1u;
            private static TrainCarLivery? s_h1;

            public static TrainCarLivery DE6Slug => Extensions.GetCached(ref s_de6slug, () => GetCarLivery("LocoDE6Slug"));
            public static TrainCarLivery Microshunter => Extensions.GetCached(ref s_microshunter, () => GetCarLivery("LocoMicroshunter"));
            public static TrainCarLivery DM1U => Extensions.GetCached(ref s_dm1u, () => GetCarLivery("LocoDM1U"));
            public static TrainCarLivery Handcar => Extensions.GetCached(ref s_h1, () => GetCarLivery("LocoHandcar"));
        }

        /// <summary>
        /// References to wagon liveries.
        /// </summary>
        public static class Wagons
        {
            private static TrainCarLivery? s_flatbed;
            private static TrainCarLivery? s_caboose;
            private static TrainCarLivery? s_hopperBrown;
            private static TrainCarLivery? s_utilityFlatbed;

            public static TrainCarLivery Flatbed => Extensions.GetCached(ref s_flatbed, () => GetCarLivery("FlatbedEmpty"));
            public static TrainCarLivery Caboose => Extensions.GetCached(ref s_caboose, () => GetCarLivery("CabooseRed"));
            public static TrainCarLivery HopperBrown => Extensions.GetCached(ref s_hopperBrown, () => GetCarLivery("HopperBrown"));
            public static TrainCarLivery UtilityFlatbed => Extensions.GetCached(ref s_utilityFlatbed, () => GetCarLivery("FlatbedShort"));
        }

        /// <summary>
        /// References to explosion prefabs.
        /// </summary>
        public static class Explosions
        {
            private static DeadTractionMotorsController? s_de6deadTM;
            private static GameObject? s_boilerExplosion;
            private static GameObject? s_electricExplosion;
            private static GameObject? s_hydraulicExplosion;
            private static GameObject? s_mechanicalExplosion;
            private static GameObject? s_tmOverspeedExplosion;
            private static GameObject? s_fireExplosion;
            private static GameObject? s_dieselLocomotiveExplosion;

            public static DeadTractionMotorsController DE6DeadTM => Extensions.GetCached(ref s_de6deadTM,
                () => Locomotives.DE6.prefab.GetComponentInChildren<DeadTractionMotorsController>());
            public static GameObject BoilerExplosion => Extensions.GetCached(ref s_boilerExplosion,
                () => Locomotives.S282A.prefab.GetComponentInChildren<BoilerDefinition>()
                    .GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
            public static GameObject ElectricExplosion => Extensions.GetCached(ref s_electricExplosion,
                () => DE6DeadTM.tmBlowPrefab);
            public static GameObject HydraulicExplosion => Extensions.GetCached(ref s_hydraulicExplosion,
                () => Locomotives.DH4.prefab.GetComponentInChildren<HydraulicTransmissionDefinition>()
                    .GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
            public static GameObject MechanicalExplosion => Extensions.GetCached(ref s_mechanicalExplosion,
                () => Locomotives.DM3.prefab.GetComponentInChildren<DieselEngineDirectDefinition>()
                    .GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
            public static GameObject TMOverspeedExplosion => Extensions.GetCached(ref s_tmOverspeedExplosion,
                () => DE6DeadTM.GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
            public static GameObject FireExplosion => Extensions.GetCached(ref s_fireExplosion,
                () => Locomotives.S282A.prefab.GetComponentInChildren<BlowbackParticlePortReader>().blowbackParticlesPrefab);
            public static GameObject DieselLocomotiveExplosion => Extensions.GetCached(ref s_dieselLocomotiveExplosion,
                () => Locomotives.DE2.prefab.GetComponent<ResourceExplosionBase>().explosionPrefab);

            public static GameObject GetExplosionPrefab(ExplosionPrefab explosionPrefab) => explosionPrefab switch
            {
                ExplosionPrefab.Boiler => BoilerExplosion,
                ExplosionPrefab.Electric => ElectricExplosion,
                ExplosionPrefab.Hydraulic => HydraulicExplosion,
                ExplosionPrefab.Mechanical => MechanicalExplosion,
                ExplosionPrefab.TMOverspeed => TMOverspeedExplosion,
                ExplosionPrefab.Fire => FireExplosion,
                ExplosionPrefab.DieselLocomotive => DieselLocomotiveExplosion,
                _ => null!
            };
        }

        /// <summary>
        /// References to materials.
        /// </summary>
        public static class Materials
        {
            private static Material? s_explodedDE2cab;
            private static Material? s_sightGlassS060;
            private static Material? s_bodyDE2;
            private static Material? s_bodyDE2new;
            private static Material? s_primerDE2;

            public static Material ExplodedDE2Cab => Extensions.GetCached(ref s_explodedDE2cab,
                () => Locomotives.DE2.explodedInteriorPrefab.transform.Find("Cab").GetComponent<Renderer>().sharedMaterial);
            public static Material SightGlassS060 => Extensions.GetCached(ref s_sightGlassS060,
                () => Locomotives.S060.interiorPrefab.transform.Find("Swivelables/WaterMeter/I_BoilerWater/sight_glass").GetComponent<Renderer>().sharedMaterial);
            public static Material BodyDE2 => Extensions.GetCached(ref s_bodyDE2,
                () => GetDVRTNew().substitutions[0].original);
            public static Material BodyDE2New => Extensions.GetCached(ref s_bodyDE2new,
                () => GetDVRTNew().substitutions[0].substitute);
            public static Material PrimerDE2 => Extensions.GetCached(ref s_primerDE2,
                () => GetPrimer().substitutions[0].substitute);

            private static PaintTheme GetDVRTNew()
            {
                PaintTheme.TryLoad("DVRT_New", out var theme);
                return theme;
            }

            private static PaintTheme GetPrimer()
            {
                PaintTheme.TryLoad("Null", out var theme);
                return theme;
            }
        }

        /// <summary>
        /// References to audio clips.
        /// </summary>
        public static class Audio
        {
            private static AudioClip? s_winSound;
            public static AudioClip WinSound => Extensions.GetCached(ref s_winSound,
                () => Wagons.Caboose.prefab.GetComponentInChildren<CareerManagerFeesScreen>().feesClearedSound);
        }
    }
}
