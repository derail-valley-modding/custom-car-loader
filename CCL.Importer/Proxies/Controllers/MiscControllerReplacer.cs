using AutoMapper;
using CCL.Types.Proxies.Controllers;
using DV.Damage;
using DV.Simulation.Brake;
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

        public MiscControllerReplacer()
        {
            CreateMap<DamageControllerProxy, DamageController>().AutoCacheAndMap();
            CreateMap<CompressorSimControllerProxy, CompressorSimController>().AutoCacheAndMap();
            CreateMap<DeadTractionMotorsControllerProxy, DeadTractionMotorsController>().AutoCacheAndMap()
                .AfterMap(DeadTractionMotorsControllerAfter);
            CreateMap<ExplosionActivationOnSignalProxy, ExplosionActivationOnSignal>().AutoCacheAndMap()
                .AfterMap(ExplosionActivationOnSignalAfter);
        }

        private void DeadTractionMotorsControllerAfter(DeadTractionMotorsControllerProxy _, DeadTractionMotorsController controller)
        {
            controller.firePrefab = DE6DeadTM.firePrefab;
            controller.sparksPrefab = DE6DeadTM.sparksPrefab;
            controller.tmBlowPrefab = DE6DeadTM.tmBlowPrefab;
        }

        private void ExplosionActivationOnSignalAfter(ExplosionActivationOnSignalProxy proxy, ExplosionActivationOnSignal explosion)
        {
            explosion.explosionPrefab = proxy.explosion switch
            {
                ExplosionPrefab.ExplosionBoiler => BoilerExplosion,
                ExplosionPrefab.ExplosionElectric => ElectricExplosion,
                ExplosionPrefab.ExplosionHydraulic => HydraulicExplosion,
                ExplosionPrefab.ExplosionMechanical => MechanicalExplosion,
                ExplosionPrefab.ExplosionTMOverspeed => TMOverspeedExplosion,
                _ => null!
            };
        }
    }
}
