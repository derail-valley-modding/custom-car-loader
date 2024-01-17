using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV;
using DV.CabControls.Spec;
using DV.Interaction;
using DV.Logic.Job;
using DV.Simulation.Ports;
using DV.ThingTypes;
using LocoSim.Definitions;
using System;
using UnityEngine;

namespace CCL.Importer.Proxies.Controls
{
    [ProxyMap(typeof(LeverProxy), typeof(Lever), fieldsFromCache: new[] { nameof(LeverProxy.nonVrStaticInteractionArea) })]
    [ProxyMap(typeof(PullerProxy), typeof(Puller), fieldsFromCache: new[] { nameof(PullerProxy.nonVrStaticInteractionArea) })]
    [ProxyMap(typeof(RotaryProxy), typeof(Rotary), fieldsFromCache: new[] { nameof(RotaryProxy.nonVrStaticInteractionArea) })]
    [ProxyMap(typeof(ToggleSwitchProxy), typeof(ToggleSwitch), fieldsFromCache: new[] { nameof(ToggleSwitchProxy.nonVrStaticInteractionArea) })]
    [ProxyMap(typeof(WheelProxy), typeof(Wheel), fieldsFromCache: new[] { nameof(WheelProxy.nonVrStaticInteractionArea) })]
    [ProxyMap(typeof(ButtonProxy), typeof(Button), fieldsFromCache: new[] { nameof(ButtonProxy.nonVrStaticInteractionArea) })]
    [ProxyMap(typeof(ExternalControlDefinitionProxy), typeof(ExternalControlDefinition))]
    [ProxyMap(typeof(StaticInteractionAreaProxy), typeof(StaticInteractionArea))]
    [ProxyMap(typeof(InteractablePortFeederProxy), typeof(InteractablePortFeeder))]
    public class ControlSpecReplacer : ProxyReplacer
    {
        protected override MonoBehaviour? CreateAndCacheTargetScript(MonoBehaviour source, Type sourceType, Type destType)
        {
            if ((source is ControlSpecProxy controlProxy) && (controlProxy.CopiedControlIndex > 0))
            {
                var targetHolder = InstantiateCopiedControl(controlProxy);
                var target = (MonoBehaviour)targetHolder.GetComponent(destType);
                Mapper.CacheTargetComponent(source, target);
                return target;
            }
            return Mapper.CreateProxyTargetAndCache(source, sourceType, destType);
        }

        protected override MonoBehaviour? MapSingleProxyScript(MonoBehaviour source, Type sourceType, Type destType)
        {
            var target = base.MapSingleProxyScript(source, sourceType, destType);



            return target;
        }

        private static GameObject InstantiateCopiedControl(ControlSpecProxy proxy)
        {
            var controlData = proxy.CopiedControlData[proxy.CopiedControlIndex];
            var interior = Globals.G.Types.TrainCarType_to_v2[(TrainCarType)controlData.CarType].interiorPrefab;
            var toCopy = interior.transform.Find(controlData.TransformPath).gameObject;

            return UnityEngine.Object.Instantiate(toCopy, Vector3.zero, Quaternion.identity, proxy.transform);
        }
    }
}