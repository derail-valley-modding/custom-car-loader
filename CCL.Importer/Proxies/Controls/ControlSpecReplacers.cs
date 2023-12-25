using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.CabControls.Spec;
using DV.Interaction;
using DV.Simulation.Ports;
using LocoSim.Definitions;
using System.ComponentModel.Composition;

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
    }
}