using CCL.Types.Components;
using DV.Simulation.Ports;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class ObjectInstancerProcessor : ModelProcessorStep
    {
        private static Dictionary<VanillaParticleSystem, ParticleSystem> s_particles = new();
        private static Dictionary<VanillaAudioSystem, LayeredAudio> s_audioSystems = new();

        private static GameObject? s_audioDE2;
        private static GameObject? s_audioDE6;
        private static GameObject? s_audioDH4;
        private static GameObject? s_audioDM3;
        private static GameObject? s_audioS060;
        private static GameObject? s_audioS282;
        private static GameObject? s_audioMicroshunter;

        private static GameObject AudioDE2 =>
            Extensions.GetCached(ref s_audioDE2, () => TrainCarType.LocoShunter.ToV2().parentType.audioPrefab);
        private static GameObject AudioDE6 =>
            Extensions.GetCached(ref s_audioDE6, () => TrainCarType.LocoDiesel.ToV2().parentType.audioPrefab);
        private static GameObject AudioDH4 =>
            Extensions.GetCached(ref s_audioDH4, () => TrainCarType.LocoDH4.ToV2().parentType.audioPrefab);
        private static GameObject AudioDM3 =>
            Extensions.GetCached(ref s_audioDM3, () => TrainCarType.LocoDM3.ToV2().parentType.audioPrefab);
        private static GameObject AudioS060 =>
            Extensions.GetCached(ref s_audioS060, () => TrainCarType.LocoS060.ToV2().parentType.audioPrefab);
        private static GameObject AudioS282 =>
            Extensions.GetCached(ref s_audioS282, () => TrainCarType.LocoSteamHeavy.ToV2().parentType.audioPrefab);
        private static GameObject AudioMicroshunter =>
            Extensions.GetCached(ref s_audioMicroshunter, () => TrainCarType.LocoMicroshunter.ToV2().parentType.audioPrefab);

        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var item in context.Car.AllPrefabs)
            {
                ProcessAll(item);
            }
        }

        public static void ProcessAll(GameObject prefab)
        {
            ProcessParticles(prefab);
            ProcessAudioCopies(prefab);
        }

        private static void ProcessParticles(GameObject prefab)
        {
            foreach (var item in prefab.GetComponentsInChildren<CopyVanillaParticleSystem>(true))
            {
                //CopyParticleSystem(GetSystem(item.SystemToCopy), item.ParticleSystem);
                var system = Object.Instantiate(GetSystem(item.SystemToCopy), item.transform);
                system.transform.localPosition = Vector3.zero;
                system.transform.localRotation = Quaternion.identity;

                item.InstancedObject = system.gameObject;

                if (item.ForcePlay)
                {
                    system.Play();
                }

                Object.Destroy(item);
            }
        }

        private static ParticleSystem GetSystem(VanillaParticleSystem system)
        {
            if (s_particles.TryGetValue(system, out ParticleSystem ps))
            {
                return ps;
            }

            ps = system switch
            {
                #region Diesel

                VanillaParticleSystem.DieselExhaustSmoke1               => TrainCarType.LocoShunter.ToV2().prefab.transform.Find(
                    "[particles]/ExhaustEngineSmoke").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.DieselExhaustSmoke2               => TrainCarType.LocoDiesel.ToV2().prefab.transform.Find(
                    "[particles]/ExhaustEngineSmoke").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.DieselHighTempSmoke               => TrainCarType.LocoShunter.ToV2().prefab.transform.Find(
                    "[particles]/HighTempEngineSmoke").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.DieselDamageSmoke                 => TrainCarType.LocoShunter.ToV2().prefab.transform.Find(
                    "[particles]/DamagedEngineSmoke").GetComponent<ParticleSystem>(),

                #endregion

                #region Steam

                VanillaParticleSystem.SteamerSteamSmoke                 => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/SteamSmoke/SteamSmoke/SteamSmoke").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerSteamSmokeThick            => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/SteamSmoke/SteamSmoke/SteamSmokeThick").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerEmberClusters              => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/SteamSmoke/SmokeEmbers/EmberClusters").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerEmberSparks                => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/SteamSmoke/SmokeEmbers/EmberSparks").GetComponent<ParticleSystem>(),


                VanillaParticleSystem.SteamerCylCockWaterDripParticles  => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/CylCockWaterDrip/CylCockWaterDrip L/CylCockWaterDripParticles").GetComponent<ParticleSystem>(),


                VanillaParticleSystem.SteamerExhaustSmallWispy          => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/CylCockSteam/CylCockSteam FR/SteamExhaust Small/GasWispy").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerExhaustSmallWave           => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/CylCockSteam/CylCockSteam FR/SteamExhaust Small/GasWave").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerExhaustSmallLeak           => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/CylCockSteam/CylCockSteam FR/SteamExhaust Small/GasLeak").GetComponent<ParticleSystem>(),


                VanillaParticleSystem.SteamerExhaustWispy               => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/SteamSafetyRelease/SteamExhaust/GasWispy").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerExhaustWave                => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/SteamSafetyRelease/SteamExhaust/GasWave").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerExhaustLeak                => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/SteamSafetyRelease/SteamExhaust/GasLeak").GetComponent<ParticleSystem>(),

                VanillaParticleSystem.SteamerExhaustLargeWispy          => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/Blowdown/SteamExhaust Large/GasWispy").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerExhaustLargeWave           => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/Blowdown/SteamExhaust Large/GasWave").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.SteamerExhaustLargeLeak           => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/Blowdown/SteamExhaust Large/GasLeak").GetComponent<ParticleSystem>(),

                VanillaParticleSystem.SteamerSteamLeaks                 => TrainCarType.LocoSteamHeavy.ToV2().prefab.transform.Find(
                    "LocoS282A_Particles/RandomLeaks/SteamLeaks/SteamLeaks").GetComponent<ParticleSystem>(),

                VanillaParticleSystem.FireboxFire                       => TrainCarType.LocoSteamHeavy.ToV2().interiorPrefab.transform.Find(
                    "Indicators/I_FireboxCoal/firebox_coal_pivot/fire").GetComponent<ParticleSystem>(),
                VanillaParticleSystem.FireboxSparks                     => TrainCarType.LocoSteamHeavy.ToV2().interiorPrefab.transform.Find(
                    "Indicators/I_FireboxCoal/sparks").GetComponent<ParticleSystem>(),

                #endregion

                _ => throw new System.NotImplementedException()
            };

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

        private static void ProcessAudioCopies(GameObject simAudioFab)
        {
            foreach (var item in simAudioFab.GetComponentsInChildren<CopyVanillaAudioSystem>())
            {
                var audio = Object.Instantiate(GetAudio(item.AudioSystem), item.transform);
                audio.transform.localPosition = Vector3.zero;
                audio.transform.localRotation = Quaternion.identity;

                item.InstancedObject = audio.gameObject;
                var readers = audio.GetComponents<LayeredAudioPortReader>().OrderBy(x => x.portId).ToArray();
                int length = Mathf.Min(readers.Length, item.Ports.Length);

                for (int i = 0; i < length; i++)
                {
                    readers[i].portId = item.Ports[i];
                }

                Object.Destroy(item);
            }
        }

        private static LayeredAudio GetAudio(VanillaAudioSystem audioSystem)
        {
            if (s_audioSystems.TryGetValue(audioSystem, out LayeredAudio audio))
            {
                return audio;
            }

            audio = audioSystem switch
            {
                #region Common

                VanillaAudioSystem.SandFlow => AudioS282.transform.Find("[sim] Engine/Sand/SandFlowLayers")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.TMOverspeed => AudioMicroshunter.transform.Find("[sim] Engine/TMOverspeed_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.CabFan => AudioDE2.transform.Find("[sim] Engine/CabFan_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region Diesel

                #region DE2

                VanillaAudioSystem.DE2Engine => AudioDE2.transform.Find("[sim] Engine/Engine_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2EnginePiston => AudioDE2.transform.Find("[sim] Engine/EnginePiston_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2EngineIgnition => AudioDE2.transform.Find("[sim] Engine/EngineIgnition_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2ElectricMotor => AudioDE2.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2Horn => AudioDE2.transform.Find("[sim] Engine/Horn_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2Compressor => AudioDE2.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region DE6

                VanillaAudioSystem.DE6EngineIdle => AudioDE6.transform.Find("[sim] Engine/Engine_Idle")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6EngineThrottling => AudioDE6.transform.Find("[sim] Engine/Engine_Throttling")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6EngineIgnition => AudioDE6.transform.Find("[sim] Engine/EngineIgnition_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6ElectricMotor => AudioDE6.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6DynamicBrakeBlower => AudioDE6.transform.Find("[sim] Engine/DBBlower_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6Horn => AudioDE6.transform.Find("[sim] Engine/LocoDiesel_Horn_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6Bell => AudioDE6.transform.Find("[sim] Engine/Bell_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6Compressor => AudioDE6.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region DH4

                VanillaAudioSystem.DH4Engine => AudioDH4.transform.Find("[sim] Engine/Engine_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4EnginePiston => AudioDH4.transform.Find("[sim] Engine/EnginePiston_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4EngineIgnition => AudioDH4.transform.Find("[sim] Engine/EngineIgnition_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4FluidCoupler => AudioDH4.transform.Find("[sim] Engine/FluidCoupler_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4HydroDynamicBrake => AudioDH4.transform.Find("[sim] Engine/HydroDynamicBrake_Layered(copy of FluidCoupler)")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4TransmissionEngaged => AudioDH4.transform.Find("[sim] Engine/TransmissionEngaged_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4ActiveCooler => AudioDH4.transform.Find("[sim] Engine/ActiveCooler_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4Horn => AudioDH4.transform.Find("[sim] Engine/Horn_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4Bell => AudioDH4.transform.Find("[sim] Engine/Bell_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DH4Compressor => AudioDH4.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region DM3

                VanillaAudioSystem.DM3Engine => AudioDM3.transform.Find("[sim] Engine/Engine_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DM3EnginePiston => AudioDM3.transform.Find("[sim] Engine/EnginePiston_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DM3EngineIgnition => AudioDM3.transform.Find("[sim] Engine/EngineIgnition_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DM3JakeBrake => AudioDM3.transform.Find("[sim] Engine/JakeBrake_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DM3TransmissionEngaged => AudioDM3.transform.Find("[sim] Engine/TransmissionEngaged_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DM3Horn => AudioDM3.transform.Find("[sim] Engine/Horn_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DM3Compressor => AudioDM3.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #endregion

                #region Steam

                VanillaAudioSystem.SteamerCoalDump => AudioS282.transform.Find("[sim] Engine/CoalDump/SandFlowLayers")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerFire => AudioS282.transform.Find("[sim] Engine/Fire/Fire_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerFireboxWind => AudioS282.transform.Find("[sim] Engine/FireboxWind/WindFirebox_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerSafetyRelease => AudioS282.transform.Find("[sim] Engine/SteamSafetyRelease/SteamRelease_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerBlowdown => AudioS282.transform.Find("[sim] Engine/Blowdown/SteamRelease_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerChestAdmission => AudioS282.transform.Find("[sim] Engine/SteamChestAdmission/SteamChestAdmission_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerCylinderCrack => AudioS282.transform.Find("[sim] Engine/CylinderCrack/SteamRelease_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerCylinderCock => AudioS282.transform.Find("[sim] Engine/CylinderCock/CylCockR")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerInjector => AudioS282.transform.Find("[sim] Engine/Injector/WaterInFlow_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerValveGear => AudioS282.transform.Find("[sim] Engine/ValveGearMechanism/Mechanism_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerValveGearDamaged => AudioS282.transform.Find("[sim] Engine/ValveGearMechanism/DamagedMechanism_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerCrownSheet => AudioS282.transform.Find("[sim] Engine/CrownSheetBoiling/CrownSheetBoiling_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerAirPump => AudioS282.transform.Find("[sim] Engine/AirPump/AirPump_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerDynamo => AudioS282.transform.Find("[sim] Engine/Dynamo/Dynamo_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerBellRing => AudioS282.transform.Find("[sim] Engine/Bell_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerBellPump => AudioS282.transform.Find("[sim] Engine/Bell_Pump")
                    .GetComponent<LayeredAudio>(),

                VanillaAudioSystem.S060Whistle => AudioS060.transform.Find("[sim] Engine/Whistle/Whistle_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.S282Whistle => AudioS282.transform.Find("[sim] Engine/Whistle/Whistle_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region Electric

                VanillaAudioSystem.BE2ElectricMotor => AudioMicroshunter.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.BE2TMController => AudioMicroshunter.transform.Find("[sim] Engine/TMController_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.BE2Compressor => AudioMicroshunter.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.BE2Horn => AudioMicroshunter.transform.Find("[sim] Engine/Horn_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                _ => throw new System.NotImplementedException(nameof(audio)),
            };

            s_audioSystems.Add(audioSystem, audio);
            return audio;
        }
    }
}
