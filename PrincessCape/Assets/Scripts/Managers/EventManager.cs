using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager
{
	private Dictionary<string, UnityEvent> eventDictionary;
	private static EventManager instance = null;

    /// <summary>
    /// Gets the instance of EventManager and creates it if there is none.
    /// </summary>
    /// <value>The instance of EventManager.</value>
	public static EventManager Instance
	{
		get
		{
            if (!Game.isClosing)
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }

            return null;
		}
	}

    /// <summary>
    /// Initializes a new instance of the <see cref="T:EventManager"/> class.
    /// </summary>
	EventManager()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();

		}
	}

    public void Clear() {
        eventDictionary.Clear();
        instance = null;
    }

    /// <summary>
    /// Starts adds a listener to the event if there is one or creates an event if there is none.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="listener">The action to be performed on the event.</param>
	public static void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;

        if (Game.isClosing) {
            return;
        }
		if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			instance.eventDictionary.Add(eventName, thisEvent);
		}
	}

    /// <summary>
    /// Removes the listener from the event if there is one.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="listener">The action to be removed.</param>
	public static void StopListening(string eventName, UnityAction listener)
	{
        if (Game.isClosing)
		{
			return;
		}

		UnityEvent thisEvent = null;

		if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
	}

    /// <summary>
    /// Triggers the event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
	public static void TriggerEvent(string eventName)
	{
		UnityEvent thisEvent = null;

		if (Game.isClosing)
		{
			return;
		}
		if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke();
		}
	}

}