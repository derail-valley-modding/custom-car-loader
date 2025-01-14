using AutoMapper;
using CCL.Types.Proxies.Controllers;
using DV.Damage;
using DV.Simulation.Brake;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using LocoSim.Definitions;
using System;
using UnityEngine;

namespace CCL.Importer.Proxies.Controllers
{
    internal class MiscControllerReplacer : Profile
    {
        private static DeadTractionMotorsController? s_de6deadTM;
        private static GameObject? s_boilerExplosion;
        private static GameObject? s_electricExplosion;
        private static GameObject? s_hydraulicExplosion;
        private static GameObject? s_mechanicalExplosion;
        private static GameObject? s_tmOverspeedExplosion;
        private static GameObject? s_fireExplosion;

        private static DeadTractionMotorsController DE6DeadTM => Extensions.GetCached(ref s_de6deadTM,
            () => TrainCarType.LocoDiesel.ToV2().prefab.GetComponentInChildren<DeadTractionMotorsController>());
        private static GameObject BoilerExplosion => Extensions.GetCached(ref s_boilerExplosion,
            () => TrainCarType.LocoSteamHeavy.ToV2().prefab.GetComponentInChildren<BoilerDefinition>()
                .GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
        private static GameObject ElectricExplosion => Extensions.GetCached(ref s_electricExplosion,
            () => DE6DeadTM.tmBlowPrefab);
        private static GameObject HydraulicExplosion => Extensions.GetCached(ref s_hydraulicExplosion,
            () => TrainCarType.LocoDH4.ToV2().prefab.GetComponentInChildren<HydraulicTransmissionDefinition>()
                .GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
        private static GameObject MechanicalExplosion => Extensions.GetCached(ref s_mechanicalExplosion,
            () => TrainCarType.LocoDM3.ToV2().prefab.GetComponentInChildren<DieselEngineDirectDefinition>()
                .GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
        private static GameObject TMOverspeedExplosion => Extensions.GetCached(ref s_tmOverspeedExplosion,
            () => DE6DeadTM.GetComponent<ExplosionActivationOnSignal>().explosionPrefab);
        private static GameObject FireExplosion => Extensions.GetCached(ref s_fireExplosion,
            () => TrainCarType.LocoSteamHeavy.ToV2().prefab.GetComponentInChildren<BlowbackParticlePortReader>().blowbackParticlesPrefab);

        public MiscControllerReplacer()
        {
            CreateMap<DamageControllerProxy, DamageController>().AutoCacheAndMap();
            CreateMap<CompressorSimControllerProxy, CompressorSimController>().AutoCacheAndMap();
            CreateMap<DeadTractionMotorsControllerProxy, DeadTractionMotorsController>().AutoCacheAndMap()
                .AfterMap(DeadTractionMotorsControllerAfter);
            CreateMap<ExplosionActivationOnSignalProxy, ExplosionActivationOnSignal>().AutoCacheAndMap()
                .AfterMap(ExplosionActivationOnSignalAfter);
            CreateMap<EngineOnReaderProxy, EngineOnReader>().AutoCacheAndMap();
            CreateMap<EnvironmentDamagerProxy, EnvironmentDamager>().AutoCacheAndMap();

            CreateMap<MagicShovellingProxy, MagicShoveling>().AutoCacheAndMap();
            CreateMap<CoalPileSimControllerProxy, CoalPileSimController>().AutoCacheAndMap();
            CreateMap<FireboxSimControllerProxy, FireboxSimController>().AutoCacheAndMap();
            CreateMap<BoilerSimControllerProxy, BoilerSimController>().AutoCacheAndMap();

            CreateMap<WindowsBreakingControllerProxy, WindowsBreakingController>().AutoCacheAndMap();
            CreateMap<BlowbackParticlePortReaderProxy, BlowbackParticlePortReader>().AutoCacheAndMap()
                .AfterMap(BlowbackParticlePortReaderAfter);

            CreateMap<ClapperControllerProxy, ClapperController>().AutoCacheAndMap();
        }

        private void DeadTractionMotorsControllerAfter(DeadTractionMotorsControllerProxy _, DeadTractionMotorsController controller)
        {
            controller.firePrefab = DE6DeadTM.firePrefab;
            controller.sparksPrefab = DE6DeadTM.sparksPrefab;
            controller.tmBlowPrefab = DE6DeadTM.tmBlowPrefab;
        }

        private void ExplosionActivationOnSignalAfter(ExplosionActivationOnSignalProxy proxy, ExplosionActivationOnSignal explosion)
        {
            explosion.explosionPrefab = GetExplosionPrefab(proxy.explosion);
        }

        private void BlowbackParticlePortReaderAfter(BlowbackParticlePortReaderProxy proxy, BlowbackParticlePortReader reader)
        {
            reader.blowbackParticlesPrefab = GetExplosionPrefab(proxy.BlowbackPrefab);
        }

        private static GameObject GetExplosionPrefab(ExplosionPrefab explosionPrefab) => explosionPrefab switch
        {
            ExplosionPrefab.Boiler => BoilerExplosion,
            ExplosionPrefab.Electric => ElectricExplosion,
            ExplosionPrefab.Hydraulic => HydraulicExplosion,
            ExplosionPrefab.Mechanical => MechanicalExplosion,
            ExplosionPrefab.TMOverspeed => TMOverspeedExplosion,
            ExplosionPrefab.Fire => FireExplosion,
            _ => null!
        };
    }
}
