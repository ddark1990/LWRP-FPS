using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ObserverSystem
{
    private static ObserverSystem _Instance;
    public static ObserverSystem Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new ObserverSystem();

            return _Instance;
        }
    }

    delegate void EventListener(EventInfo eventInfo);
    Dictionary<Type, List<EventListener>> eventListeners;

    public void RegisterListener<T>(Action<T> listener) where T : EventInfo
    {
        var eventType = typeof(T);

        if (eventListeners == null)
            eventListeners = new Dictionary<Type, List<EventListener>>();

        if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null)
            eventListeners[eventType] = new List<EventListener>();

        EventListener wrapper = (eventInfo) => { listener((T)eventInfo); };

        eventListeners[eventType].Add(wrapper);
    }

    public void UnregisterListener<T>(Action<T> listener) where T : EventInfo
    {
        var eventType = typeof(T);

        EventListener wrapper = (eventInfo) => { listener((T)eventInfo); };

        eventListeners[eventType].Remove(wrapper);
    }

    public void FireEvent(EventInfo eventInfo)
    {
        var eventType = eventInfo.GetType();

        if (eventListeners == null || eventListeners[eventType] == null) return;

        for (int i = 0; i < eventListeners[eventType].Count; i++)
        {
            var listener = eventListeners[eventType][i];

            listener(eventInfo);
        }
    }
}
