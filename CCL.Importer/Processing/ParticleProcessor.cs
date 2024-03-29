using CCL.Types.Components;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class ParticleProcessor : ModelProcessorStep
    {
        private static Dictionary<VanillaParticleSystem, ParticleSystem> s_particles = new Dictionary<VanillaParticleSystem, ParticleSystem>();

        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var item in context.Car.AllPrefabs)
            {
                ProcessParticles(item);
            }
        }

        public static void ProcessParticles(GameObject prefab)
        {
            foreach (var item in prefab.GetComponentsInChildren<CopyVanillaParticleSystem>())
            {
                //CopyParticleSystem(GetSystem(item.SystemToCopy), item.ParticleSystem);
                var system = Object.Instantiate(GetSystem(item.SystemToCopy), item.transform);
                system.transform.localPosition = Vector3.zero;
                system.transform.localRotation = Quaternion.identity;
                Object.Destroy(item);
            }
        }

        private static ParticleSystem GetSystem(VanillaParticleSystem system)
        {
            ParticleSystem ps;

            if (s_particles.TryGetValue(system, out ps))
            {
                return ps;
            }

            switch (system)
            {
                #region Diesel

                case VanillaParticleSystem.DieselExhaustSmoke1:
                    ps = TrainCarType.LocoShunter.ToV2().prefab.transform.Find(
                        "[particles]/ExhaustEngineSmoke").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.DieselExhaustSmoke2:
                    ps = TrainCarType.LocoDiesel.ToV2().prefab.transform.Find(
                        "[particles]/ExhaustEngineSmoke").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.DieselHighTempSmoke:
                    ps = TrainCarType.LocoShunter.ToV2().prefab.transform.Find(
                        "[particles]/HighTempEngineSmoke").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.DieselDamageSmoke:
                    ps = TrainCarType.LocoShunter.ToV2().prefab.transform.Find(
                        "[particles]/DamagedEngineSmoke").GetComponent<ParticleSystem>();
                    break;

                #endregion

                #region Steam

                case VanillaParticleSystem.SteamerSteamSmoke:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/SteamSmoke/SteamSmoke/SteamSmoke").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerSteamSmokeThick:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/SteamSmoke/SteamSmoke/SteamSmokeThick").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerEmberClusters:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/SteamSmoke/SmokeEmbers/EmberClusters").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerEmberSparks:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/SteamSmoke/SmokeEmbers/EmberSparks").GetComponent<ParticleSystem>();
                    break;

                case VanillaParticleSystem.SteamerCylCockWaterDripParticles:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/CylCockWaterDrip/CylCockWaterDrip L/CylCockWaterDripParticles").GetComponent<ParticleSystem>();
                    break;

                case VanillaParticleSystem.SteamerExhaustSmallWispy:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/CylCockSteam/CylCockSteam FR/SteamExhaust Small/GasWispy").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerExhaustSmallWave:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/CylCockSteam/CylCockSteam FR/SteamExhaust Small/GasWave").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerExhaustSmallLeak:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/CylCockSteam/CylCockSteam FR/SteamExhaust Small/GasLeak").GetComponent<ParticleSystem>();
                    break;

                case VanillaParticleSystem.SteamerExhaustWispy:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/SteamSafetyRelease/SteamExhaust/GasWispy").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerExhaustWave:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/SteamSafetyRelease/SteamExhaust/GasWave").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerExhaustLeak:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/SteamSafetyRelease/SteamExhaust/GasLeak").GetComponent<ParticleSystem>();
                    break;

                case VanillaParticleSystem.SteamerExhaustLargeWispy:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/Blowdown/SteamExhaust Large/GasWispy").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerExhaustLargeWave:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/Blowdown/SteamExhaust Large/GasWave").GetComponent<ParticleSystem>();
                    break;
                case VanillaParticleSystem.SteamerExhaustLargeLeak:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/Blowdown/SteamExhaust Large/GasLeak").GetComponent<ParticleSystem>();
                    break;

                case VanillaParticleSystem.SteamerSteamLeaks:
                    ps = TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                        "LocoS282A_Particles/RandomLeaks/SteamLeaks/SteamLeaks").GetComponent<ParticleSystem>();
                    break;

                #endregion

                default:
                    throw new System.NotImplementedException();
            }

            s_particles.Add(system, ps);
            return ps;
        }

        //private static void CopyParticleSystem(ParticleSystem from, ParticleSystem to)
        //{
        //    CopyModule(from.collision, to.collision);
        //    CopyModule(from.colorBySpeed, to.colorBySpeed);
        //    CopyModule(from.colorOverLifetime, to.colorOverLifetime);
        //    CopyModule(from.customData, to.customData);
        //    CopyModule(from.emission, to.emission);
        //    CopyModule(from.externalForces, to.externalForces);
        //    CopyModule(from.forceOverLifetime, to.forceOverLifetime);
        //    CopyModule(from.inheritVelocity, to.inheritVelocity);
        //    CopyModule(from.lights, to.lights);
        //    CopyModule(from.limitVelocityOverLifetime, to.limitVelocityOverLifetime);
        //    CopyModule(from.main, to.main);
        //    CopyModule(from.noise, to.noise);
        //    CopyModule(from.rotationBySpeed, to.rotationBySpeed);
        //    CopyModule(from.rotationOverLifetime, to.rotationOverLifetime);
        //    CopyModule(from.shape, to.shape);
        //    CopyModule(from.sizeBySpeed, to.sizeBySpeed);
        //    CopyModule(from.sizeOverLifetime, to.sizeOverLifetime);
        //    //CopyModule(from.subEmitters, to.subEmitters);
        //    CopyModule(from.textureSheetAnimation, to.textureSheetAnimation);
        //    CopyModule(from.trails, to.trails);
        //    CopyModule(from.trigger, to.trigger);
        //    CopyModule(from.velocityOverLifetime, to.velocityOverLifetime);

        //    to.hideFlags = from.hideFlags;
        //    to.name = from.name;
        //    to.randomSeed = from.randomSeed;
        //    to.tag = from.tag;
        //    to.time = from.time;
        //    to.useAutoRandomSeed = from.useAutoRandomSeed;
        //}

        //private static void CopyModule<T>(T src, T dest)
        //{
        //    var type = typeof(T);
        //    //var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        //    foreach (var p in type.GetProperties())
        //    {
        //        if (p.CanWrite)
        //        {
        //            p.SetValue(dest, p.GetValue(src));
        //        }
        //    }
        //}
    }
}
