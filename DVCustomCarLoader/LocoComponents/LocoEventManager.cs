using System;
using System.Collections.Generic;
using System.Linq;
using CCL_GameScripts.CabControls;
using DV.Util.EventWrapper;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public interface ILocoEventProvider
    {
        bool TryBind(ILocoEventAcceptor listener);
    }

    public interface ILocoEventAcceptor
    {
        bool IsBound { get; set; }
        bool IsBoundToInterior { get; set; }
        SimEventType EventType { get; }
    }

    public interface ILocoEventAcceptor<TValue> : ILocoEventAcceptor
    {
        void HandleChange(TValue value);
    }

    public interface ILocoValueProvider
    {
        bool TryBind(ILocoValueWatcher watcher);
    }

    public interface ILocoValueWatcher
    {
        bool IsBound { get; set; }
        bool IsBoundToInterior { get; set; }
        CabIndicatorType ValueBinding { get; }
        Func<float> ValueFunction { get; set; }
    }

    public class LocoValueBinding : ILocoValueWatcher
    {
        public bool IsBound { get; set; } = false;
        public bool IsBoundToInterior { get; set; } = false;
        [field: SerializeField]
        public CabIndicatorType ValueBinding { get; protected set; }
        public Func<float> ValueFunction { get; set; }

        public float LatestValue { get; protected set; }

        public LocoValueBinding(CabIndicatorType valueType)
        {
            ValueBinding = valueType;
        }

        public void GetLatest()
        {
            if (IsBound) LatestValue = ValueFunction();
            else LatestValue = 0;
        }
    }

    public interface ILocoMultiValueWatcher
    {
        IEnumerable<ILocoValueWatcher> ValueBindings { get; }
    }

    public static class LocoEventExtensions
    {
        private static readonly Dictionary<ILocoEventAcceptor, (Type, Delegate)> UnbindActions =
            new Dictionary<ILocoEventAcceptor, (Type, Delegate)>();

        public static void Bind<TVal>(this ILocoEventAcceptor<TVal> acceptor, event_<TVal> e)
        {
            e.Register(acceptor.HandleChange);

            Action<Action<TVal>> unbinder = e.Unregister;
            UnbindActions.Add(acceptor, (typeof(Action<TVal>), unbinder));

            acceptor.IsBound = true;
        }

        public static void UnBind(this ILocoEventAcceptor acceptor)
        {
            try
            {
                if (UnbindActions.TryGetValue(acceptor, out var unbindInfo))
                {
                    var handler = AccessTools.Method(acceptor.GetType(), "HandleChange");
                    var handlerDelegate = handler.CreateDelegate(unbindInfo.Item1);
                    unbindInfo.Item2.DynamicInvoke(handlerDelegate);
                }
            }
            catch (Exception ex)
            {
                Main.Error($"When unbinding {(acceptor as Component)?.name}");
                Main.ModEntry.Logger.LogException(ex);
            }
        }

        public static bool TryBindGeneric<TVal>(this ILocoEventAcceptor acceptor, event_<TVal> e)
        {
            if (acceptor is ILocoEventAcceptor<TVal> castAcceptor)
            {
                e.Register(castAcceptor.HandleChange);
                acceptor.IsBound = true;
                return true;
            }
            return false;
        }

        public static void Bind(this ILocoValueWatcher watcher, Func<float> valueFunc)
        {
            watcher.ValueFunction = valueFunc;
            watcher.IsBound = true;
        }

        public static void UnBind(this ILocoValueWatcher watcher)
        {
#if DEBUG
            Main.Log($"Detached watcher {(watcher as Component)?.name} ({Enum.GetName(typeof(CabIndicatorType), watcher.ValueBinding)})");
#endif
            watcher.ValueFunction = null;
            watcher.IsBound = false;
        }
    }

    public class LocoEventManager : MonoBehaviour
    {
        public class EventCollection
        {
            public List<ILocoEventProvider> EventProviders = new List<ILocoEventProvider>();
            public List<ILocoValueProvider> ValueProviders = new List<ILocoValueProvider>();

            public List<ILocoEventAcceptor> EventAcceptors = new List<ILocoEventAcceptor>();
            public List<ILocoValueWatcher> ValueWatchers = new List<ILocoValueWatcher>();
            public List<ILocoMultiValueWatcher> MultiValueWatchers = new List<ILocoMultiValueWatcher>();

            public void Clear()
            {
                EventProviders.Clear();
                EventAcceptors.Clear();

                ValueProviders.Clear();
                ValueWatchers.Clear();
                MultiValueWatchers.Clear();
            }

            public void Initialize(GameObject prefab)
            {
                // Events
                EventProviders.AddRange(prefab.GetComponentsByInterface<ILocoEventProvider>());
                EventAcceptors.AddRange(prefab.GetComponentsInChildrenByInterface<ILocoEventAcceptor>());
#if DEBUG
                Main.Log($"EventManager Start: {EventProviders.Count} event providers, {EventAcceptors.Count} acceptors");
#endif

                // Value watchers
                ValueProviders.AddRange(prefab.GetComponentsByInterface<ILocoValueProvider>());
                ValueWatchers.AddRange(prefab.GetComponentsInChildrenByInterface<ILocoValueWatcher>());
#if DEBUG
                Main.Log($"EventManager Start: {ValueProviders.Count} value providers, {ValueWatchers.Count} value watchers");
#endif

                // Multi-Value Watchers
                MultiValueWatchers.AddRange(prefab.GetComponentsInChildrenByInterface<ILocoMultiValueWatcher>());
#if DEBUG
                Main.Log($"EventManager Start: {MultiValueWatchers.Count} multi value watchers");
#endif
            }

            public void BindSelf(bool bindToInterior)
            {
                BindOther(this, bindToInterior);
            }

            public void BindOther(EventCollection providers, bool bindToInterior)
            {
                foreach (var acceptor in EventAcceptors)
                {
                    // search for a loco component/event to bind to
                    BindFirst(providers.EventProviders, acceptor, bindToInterior);
                }

                foreach (var watcher in ValueWatchers)
                {
                    WatchFirst(providers.ValueProviders, watcher, bindToInterior);
                }

                foreach (var multiWatcher in MultiValueWatchers)
                {
                    foreach (var binding in multiWatcher.ValueBindings)
                    {
                        WatchFirst(providers.ValueProviders, binding, bindToInterior);
                    }
                }
            }

            public void Unbind(bool onlyInterior = false)
            {
                foreach (var acceptor in EventAcceptors)
                {
                    if (!onlyInterior || acceptor.IsBoundToInterior) acceptor.UnBind();
                }

                foreach (var watcher in ValueWatchers)
                {
                    if (!onlyInterior || watcher.IsBoundToInterior) watcher.UnBind();
                }

                foreach (var multiWatcher in MultiValueWatchers)
                {
                    foreach (var binding in multiWatcher.ValueBindings)
                    {
                        if (!onlyInterior || binding.IsBoundToInterior) binding.UnBind();
                    }
                }
            }
        }

        public EventCollection ExteriorEvents = new EventCollection();
        public EventCollection InteriorEvents = new EventCollection();

        /// <summary>
        /// Bind the acceptor to the first matching event provider in providers
        /// </summary>
        private static void BindFirst(IEnumerable<ILocoEventProvider> providers, ILocoEventAcceptor acceptor, bool interior)
        {
            foreach (var provider in providers)
            {
                if (provider.TryBind(acceptor))
                {
                    acceptor.IsBoundToInterior = interior;
                    return;
                }
            }
        }

        /// <summary>
        /// Bind the watcher to the first matching value provider in providers
        /// </summary>
        private static void WatchFirst(IEnumerable<ILocoValueProvider> providers, ILocoValueWatcher watcher, bool interior)
        {
            foreach (var provider in providers)
            {
                if (provider.TryBind(watcher))
                {
                    // value watchers should only have one provider each (many to one)
#if DEBUG
                    Main.Log($"Attached watcher {(watcher as Component)?.name} to {(provider as Component)?.name}.{Enum.GetName(typeof(CabIndicatorType), watcher.ValueBinding)}");
#endif
                    return;
                }
            }
        }

        public void Start()
        {
            ExteriorEvents.Initialize(gameObject);
            ExteriorEvents.BindSelf(false);
        }

        public void OnInteriorLoaded(GameObject interior)
        {
            InteriorEvents.Initialize(interior);
            InteriorEvents.BindSelf(true);

            InteriorEvents.BindOther(ExteriorEvents, false);
            ExteriorEvents.BindOther(InteriorEvents, true);
        }

        public void OnInteriorUnloaded()
        {
            InteriorEvents.Unbind();
            InteriorEvents.Clear();

            ExteriorEvents.Unbind(true);
        }
    }
}