using CCL.Types.Proxies.Controls;
using DV.CabControls.Spec;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies.Controls
{
    [Export(typeof(IProxyReplacer))]
    public class ButtonReplacer : ProxyReplacer<ButtonProxy, Button>
    {
        protected override List<(Func<ButtonProxy, MonoBehaviour> SourceProvider, Expression<Func<Button, MonoBehaviour>> DestProvider)> FieldsToGetFromCache()
        {
            return new List<(Func<ButtonProxy, MonoBehaviour> SourceProvider, Expression<Func<Button, MonoBehaviour>> DestProvider)> {
                (src => src.nonVrStaticInteractionArea, dest => dest.nonVrStaticInteractionArea)
            };
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class LeverReplacer : ProxyReplacer<LeverProxy, Lever>
    {
        protected override List<(Func<LeverProxy, MonoBehaviour> SourceProvider, Expression<Func<Lever, MonoBehaviour>> DestProvider)> FieldsToGetFromCache()
        {
            return new List<(Func<LeverProxy, MonoBehaviour> SourceProvider, Expression<Func<Lever, MonoBehaviour>> DestProvider)> {
                (src => src.nonVrStaticInteractionArea, dest => dest.nonVrStaticInteractionArea)
            };
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class PullerReplacer : ProxyReplacer<PullerProxy, Puller>
    {
        protected override List<(Func<PullerProxy, MonoBehaviour> SourceProvider, Expression<Func<Puller, MonoBehaviour>> DestProvider)> FieldsToGetFromCache()
        {
            return new List<(Func<PullerProxy, MonoBehaviour> SourceProvider, Expression<Func<Puller, MonoBehaviour>> DestProvider)> {
                (src => src.nonVrStaticInteractionArea, dest => dest.nonVrStaticInteractionArea)
            };
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class RotaryReplacer : ProxyReplacer<RotaryProxy, Rotary>
    {
        protected override List<(Func<RotaryProxy, MonoBehaviour> SourceProvider, Expression<Func<Rotary, MonoBehaviour>> DestProvider)> FieldsToGetFromCache()
        {
            return new List<(Func<RotaryProxy, MonoBehaviour> SourceProvider, Expression<Func<Rotary, MonoBehaviour>> DestProvider)> {
                (src => src.nonVrStaticInteractionArea, dest => dest.nonVrStaticInteractionArea)
            };
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class ToggleSwitchReplacer : ProxyReplacer<ToggleSwitchProxy, ToggleSwitch>
    {
        protected override List<(Func<ToggleSwitchProxy, MonoBehaviour> SourceProvider, Expression<Func<ToggleSwitch, MonoBehaviour>> DestProvider)> FieldsToGetFromCache()
        {
            return new List<(Func<ToggleSwitchProxy, MonoBehaviour> SourceProvider, Expression<Func<ToggleSwitch, MonoBehaviour>> DestProvider)> {
                (src => src.nonVrStaticInteractionArea, dest => dest.nonVrStaticInteractionArea)
            };
        }
    }


    [Export(typeof(IProxyReplacer))]
    public class WheelReplacer : ProxyReplacer<WheelProxy, Wheel>
    {
        protected override List<(Func<WheelProxy, MonoBehaviour> SourceProvider, Expression<Func<Wheel, MonoBehaviour>> DestProvider)> FieldsToGetFromCache()
        {
            return new List<(Func<WheelProxy, MonoBehaviour> SourceProvider, Expression<Func<Wheel, MonoBehaviour>> DestProvider)> {
                (src => src.nonVrStaticInteractionArea, dest => dest.nonVrStaticInteractionArea)
            };
        }
    }
}