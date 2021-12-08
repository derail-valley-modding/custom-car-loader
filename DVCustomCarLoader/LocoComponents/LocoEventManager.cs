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
    }

    public class LocoEventWrapper<T>
    {
        public readonly ILocoEventProvider Provider;
        public readonly SimEventType EventType;

        private LocoEventWrapper(ILocoEventProvider parent, SimEventType eventType)
        {
            Provider = parent;
            EventType = eventType;
        }

        public void OnChange(T newVal)
        {
            Provider.EventManager?.Dispatch(Provider, EventType, newVal);
        }

        public static LocoEventWrapper<T> Create(ref event_<T> e, ILocoEventProvider parent, SimEventType eventType)
        {
            var wrapper = new LocoEventWrapper<T>(parent, eventType);
            e.Register(wrapper.OnChange);
            return wrapper;
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
            }

            public void Initialize(GameObject prefab, LocoEventManager dispatcher)
            {
                // Events
                var acceptors = prefab.GetComponentsInChildrenByInterface<ILocoEventAcceptor>();
                int aCount = 0;
                foreach (var acceptor in acceptors)
                {
                    aCount++;
                    foreach (SimEventType eventType in acceptor.EventTypes)
                    {
                        Add(eventType, acceptor);
                    }
                }

                var providers = prefab.GetComponentsInChildrenByInterface<ILocoEventProvider>();
                int pCount = 0;
                foreach (var provider in providers)
                {
                    pCount++;
                    provider.EventManager = dispatcher;
                }
                
#if DEBUG
                Main.Log($"EventManager Start: {pCount} providers, {aCount} acceptors");
#endif
            }
        }

        public EventCollection ExteriorEvents = new EventCollection();
        public EventCollection InteriorEvents = new EventCollection();

        public void Dispatch(ILocoEventProvider provider, SimEventType eventType, object newVal)
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
        }

        public void UpdateValueDispatchOnChange<TVal>(ILocoEventProvider provider, ref TVal value, TVal newVal, SimEventType eventType)
        {
            if (!newVal.Equals(value))
            {
                value = newVal;
                Dispatch(provider, eventType, newVal);
            }
        }

        public void Start()
        {
            ExteriorEvents.Initialize(gameObject, this);
        }

        public void OnInteriorLoaded(GameObject interior)
        {
            InteriorEvents.Initialize(interior, this);
        }

        public void OnInteriorUnloaded()
        {
            InteriorEvents.Clear();
        }
    }
}