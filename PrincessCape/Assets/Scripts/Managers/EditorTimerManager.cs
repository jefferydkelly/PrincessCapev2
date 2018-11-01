using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorTimerManager : MonoBehaviour {

    static EditorTimerManager instance;
    List<Timer> timers;
    List<Timer> toAdd;
    List<Timer> toRemove;

	// Use this for initialization
    public void Init () {
        timers = new List<Timer>();
        toAdd = new List<Timer>();
        toRemove = new List<Timer>();
        instance = this;
	}
	
	// Update is called once per frame
    /// <summary>
    /// Adds the timer to the array to be added.
    /// </summary>
    /// <param name="t">T.</param>
    public void AddTimer(Timer t)
    {
        toAdd.Add(t);
    }

    /// <summary>
    /// Adds the timer to the list of timers to be removed.
    /// </summary>
    /// <param name="t">T.</param>
    public void RemoveTimer(Timer t)
    {
        toRemove.Add(t);
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static EditorTimerManager Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Update all the currently active timers.
    /// </summary>
    /// <returns>The update.</returns>
    public void Update()
    {
        if (toAdd == null) {
            timers = new List<Timer>();
            toAdd = new List<Timer>();
            toRemove = new List<Timer>();
        }
        foreach (Timer t in toAdd)
        {
            timers.Add(t);
        }
        toAdd.Clear();
        foreach (Timer t in timers)
        {
            if (t != null)
            {
                t.Update(Time.deltaTime);
            }
            else
            {
                toRemove.Add(t);
            }
        }

        foreach (Timer t in toRemove)
        {
            timers.Remove(t);
        }

        toRemove.Clear();
    }
}
