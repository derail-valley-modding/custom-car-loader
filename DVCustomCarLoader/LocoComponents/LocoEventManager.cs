using System;
using System.Collections.Generic;
using System.Linq;
using CCL_GameScripts.CabControls;
using DV.Util.EventWrapper;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public struct LocoEventInfo
    {
        public readonly ILocoEventProvider Provider;
        public readonly SimEventType EventType;
        public readonly object NewValue;

        public LocoEventInfo(ILocoEventProvider provider, SimEventType type, object val)
        {
            Provider = provider;
            EventType = type;
            NewValue = val;
        }
    }

    public interface ILocoEventProvider
    {
        LocoEventManager EventManager { get; set; }
        IEnumerable<WatchableValue> Watchables { get; }
        void ForceDispatchAll();
    }

    public class LocoEventWrapper<T> where T : struct
    {
        public readonly ILocoEventProvider Provider;
        public readonly SimEventType EventType;

        public T? LastValue = null;

        private LocoEventWrapper(ILocoEventProvider parent, SimEventType eventType)
        {
            Provider = parent;
            EventType = eventType;
        }

        public void OnChange(T newVal)
        {
            LastValue = newVal;
            Provider.EventManager?.Dispatch(Provider, EventType, newVal);
        }

        public void ForceDispatch()
        {
            if (LastValue.HasValue)
            {
                Provider.EventManager?.Dispatch(Provider, EventType, LastValue.Value);
            }
        }

        public static LocoEventWrapper<T> Create(ref event_<T> e, ILocoEventProvider parent, SimEventType eventType)
        {
            var wrapper = new LocoEventWrapper<T>(parent, eventType);
            e.Register(wrapper.OnChange);
            return wrapper;
        }
    }

    public abstract class WatchableValue
    {
        public readonly ILocoEventProvider Provider;
        public readonly SimEventType EventType;

        public LocoEventManager EventManager { get; set; }

        protected WatchableValue(ILocoEventProvider parent, SimEventType eventType)
        {
            Provider = parent;
            EventType = eventType;
        }

        public abstract void UpdateAndDispatchChanges();
        public abstract void ForceDispatch();
    }

    public class WatchableValue<TVal> : WatchableValue
    {
        private readonly Func<TVal> getterFunc;
        public TVal LastValue { get; private set; }

        public WatchableValue(ILocoEventProvider parent, SimEventType eventType, Func<TVal> getter) :
            base(parent, eventType)
        {
            getterFunc = getter;
        }

        public override void UpdateAndDispatchChanges()
        {
            TVal newVal = getterFunc();
            if (!newVal.Equals(LastValue))
            {
                LastValue = newVal;
                EventManager.Dispatch(Provider, EventType, newVal);
            }
        }

        public override void ForceDispatch()
        {
            EventManager.Dispatch(Provider, EventType, LastValue);
        }
    }

    public class WatchableSimValue : WatchableValue
    {
        private readonly SimComponent component;
        public float LastValue { get; private set; }

        public WatchableSimValue(ILocoEventProvider parent, SimEventType eventType, SimComponent toWatch) :
            base(parent, eventType)
        {
            component = toWatch;
        }

        public override void UpdateAndDispatchChanges()
        {
            if (component.value != LastValue)
            {
                LastValue = component.value;
                EventManager.Dispatch(Provider, EventType, LastValue);
            }
        }

        public override void ForceDispatch()
        {
            EventManager.Dispatch(Provider, EventType, LastValue);
        }
    }

    internal static class WatchableExtensions
    {
        public static void AddNew<TVal>(this IList<WatchableValue> list, ILocoEventProvider parent, SimEventType eventType, Func<TVal> getter)
        {
            list.Add(new WatchableValue<TVal>(parent, eventType, getter));
        }

        public static void AddNew(this IList<WatchableValue> list, ILocoEventProvider parent, SimEventType eventType, SimComponent toWatch)
        {
            list.Add(new WatchableSimValue(parent, eventType, toWatch));
        }
    }

    public interface ILocoEventAcceptor
    {
        SimEventType[] EventTypes { get; }
        void HandleEvent(LocoEventInfo eventInfo);
    }

    public class LocoEventManager : MonoBehaviour
    {
        public class EventCollection
        {
            protected Dictionary<SimEventType, LinkedList<ILocoEventAcceptor>> EventAcceptors = 
                new Dictionary<SimEventType, LinkedList<ILocoEventAcceptor>>();

            protected List<ILocoEventProvider> Providers = new List<ILocoEventProvider>();

            protected void Add(SimEventType eventType, ILocoEventAcceptor acceptor)
            {
                if (!EventAcceptors.TryGetValue(eventType, out var acceptList))
                {
                    acceptList = new LinkedList<ILocoEventAcceptor>();
                    EventAcceptors.Add(eventType, acceptList);
                }

                acceptList.AddLast(acceptor);
            }

            public IEnumerable<ILocoEventAcceptor> GetAcceptors(SimEventType eventType)
            {
                if (EventAcceptors.TryGetValue(eventType, out var acceptList))
                {
                    return acceptList;
                }
                return Enumerable.Empty<ILocoEventAcceptor>();
            }

            public void Clear()
            {
                EventAcceptors.Clear();
                Providers.Clear();
            }

            public void Initialize(GameObject prefab, LocoEventManager dispatcher)
            {
                // Events
                var acceptors = prefab.GetComponentsInChildrenByInterface<ILocoEventAcceptor>();
                int aCount = 0;
                foreach (var acceptor in acceptors)
                {
                    aCount++;
                    if (!acceptor.EventTypes.SafeAny()) continue;

                    foreach (SimEventType eventType in acceptor.EventTypes)
                    {
                        Add(eventType, acceptor);
                    }
                }

                var providers = prefab.GetComponentsInChildrenByInterface<ILocoEventProvider>();
                Providers.AddRange(providers);
                foreach (var provider in providers)
                {
                    provider.EventManager = dispatcher;

                    if (provider.Watchables.SafeAny())
                    {
                        foreach (var watchable in provider.Watchables)
                        {
                            watchable.EventManager = dispatcher;
                        }
                    }
                }
                
                Main.LogVerbose($"EventManager Start: {Providers.Count} providers, {aCount} acceptors");
            }

            public void UpdateWatchables()
            {
                foreach (var provider in Providers)
                {
                    if (provider.Watchables.SafeAny())
                    {
                        foreach (var watchable in provider.Watchables)
                        {
                            watchable.UpdateAndDispatchChanges();
                        }
                    }
                }
            }

            public void ForceDispatchAll()
            {
                foreach (var provider in Providers)
                {
                    provider.ForceDispatchAll();

                    if (provider.Watchables.SafeAny())
                    {
                        foreach (var watchable in provider.Watchables)
                        {
                            watchable.ForceDispatch();
                        }
                    }
                }
            }
        }

        public EventCollection ExteriorEvents = new EventCollection();
        public EventCollection InteriorEvents = new EventCollection();

        public event Action<ILocoEventProvider, SimEventType, object> OnDispatch;

        public virtual void Dispatch(ILocoEventProvider provider, SimEventType eventType, object newVal)
        {
            var acceptors = ExteriorEvents.GetAcceptors(eventType);
            foreach (var acceptor in acceptors)
            {
                acceptor.HandleEvent(new LocoEventInfo(provider, eventType, newVal));
            }

            acceptors = InteriorEvents.GetAcceptors(eventType);
            foreach (var acceptor in acceptors)
            {
                acceptor.HandleEvent(new LocoEventInfo(provider, eventType, newVal));
            }

            OnDispatch?.Invoke(provider, eventType, newVal);
        }

        public void UpdateValueDispatchOnChange<TVal>(ILocoEventProvider provider, ref TVal value, TVal newVal, SimEventType eventType)
        {
            if (!newVal.Equals(value))
            {
                value = newVal;
                Dispatch(provider, eventType, newVal);
            }
        }

        public virtual void Start()
        {
            ExteriorEvents.Initialize(gameObject, this);

            ExteriorEvents.ForceDispatchAll();
        }

        public void OnInteriorLoaded(GameObject interior)
        {
            InteriorEvents.Initialize(interior, this);

            ExteriorEvents.ForceDispatchAll();
            InteriorEvents.ForceDispatchAll();
        }

        public void OnInteriorUnloaded()
        {
            InteriorEvents.Clear();
        }

        public void Update()
        {
            InteriorEvents.UpdateWatchables();
            ExteriorEvents.UpdateWatchables();
        }
    }
}