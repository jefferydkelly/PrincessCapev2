using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager {
    private Dictionary<string, UnityEvent> eventDictionary;
    private static EventManager instance = null;

    public static EventManager Instance {
        get {
            if (instance == null) {
                instance = new EventManager();
            }
            return instance;
        }
    }

    EventManager()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();

        }
    }

    public static void StartListening(string eventName, UnityAction listener) {
        UnityEvent thisEvent = null;

        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener) {
        if (instance == null) {
            return;
        }

        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName) {
        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.Invoke();
        }
    }

}
