using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void TimerDelegate();
public class TimerManager: Manager
{
    static TimerManager instance;
    List<Timer> timers;
    List<Timer> toAdd;
    List<Timer> toRemove;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TimerManager"/> class.
    /// </summary>
    public TimerManager() {
        timers = new List<Timer>();
        toAdd = new List<Timer>();
        toRemove = new List<Timer>();
    }

    /// <summary>
    /// Adds the timer to the array to be added.
    /// </summary>
    /// <param name="t">T.</param>
    public void AddTimer(Timer t) {
        toAdd.Add(t);
    }

    /// <summary>
    /// Adds the timer to the list of timers to be removed.
    /// </summary>
    /// <param name="t">T.</param>
    public void RemoveTimer(Timer t) {
        toRemove.Add(t);
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static TimerManager Instance {
        get {
            if (instance == null) {
                instance = new TimerManager();
                Game.Instance.AddManager(instance);
            }
            return instance;
        }
    }

    /// <summary>
    /// Update all the currently active timers.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="dt">Dt.</param>
    public void Update(float dt) {
        foreach(Timer t in toAdd) {
            timers.Add(t);
        }
        toAdd.Clear();
        foreach(Timer t in timers) {
            t.Update(dt);
        }

        foreach(Timer t in toRemove) {
            timers.Remove(t);
        }

        toRemove.Clear();
    }
}

public class Timer
{
	float runTime = 0;
	float curTime = 0;
	int numRepeats = 0;
	int timesRun = 0;
	bool infinite = false;
	TimerState state;
	public UnityEvent OnTick;
	public UnityEvent OnComplete;
	/// <summary>
	/// Initializes a new instance of the <see cref="T:Timer"/> class.
	/// </summary>
	/// <param name="td">The function to be run when the timer ticks.</param>
	/// <param name="time">The time between ticks.</param>
	/// <param name="reps">The number of times the timer will tick beyond the first.</param>
	public Timer(float time, int reps)
	{
		runTime = time;
		numRepeats = reps;
		infinite = false;
		OnTick = new UnityEvent();
		OnComplete = new UnityEvent();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Timer"/> class.
	/// </summary>
	/// <param name="td">The function to be run when the timer ticks.</param>
	/// <param name="time">The time between ticks.</param>
	/// <param name="inf">If set to <c>true</c> the timer will run until it is stopped.</param>
	public Timer(float time, bool inf = false)
	{
		runTime = time;
		numRepeats = 0;
		infinite = inf;
		OnTick = new UnityEvent();
		OnComplete = new UnityEvent();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start()
	{
		if (state == TimerState.NotStarted || state == TimerState.Done || state == TimerState.Stopped)
		{
			state = TimerState.Running;
			curTime = 0;
            timesRun = 0;
			TimerManager.Instance.AddTimer(this);
		}

	}

	/// <summary>
	/// Pause this instance.
	/// </summary>
	public void Pause()
	{
		if (state == TimerState.Running)
		{
			state = TimerState.Paused;
		}
	}

	/// <summary>
	/// Unpause this instance.
	/// </summary>
	public void Unpause()
	{
		if (state == TimerState.Paused)
		{
			state = TimerState.Running;
		}
	}

	/// <summary>
	/// Stop this instance.
	/// </summary>
	public void Stop()
	{
		state = TimerState.Stopped;
		TimerManager.Instance.RemoveTimer(this);
	}

	/// <summary>
	/// Update the timer.
	/// </summary>
	/// <returns>The update.</returns>
	/// <param name="dt">Dt.</param>
	public void Update(float dt)
	{
		if (state == TimerState.Running)
		{
			curTime += dt;
			if (curTime >= runTime)
			{
				curTime -= runTime;
				OnTick.Invoke();
				timesRun++;

				if (timesRun > numRepeats || infinite)
				{
					OnComplete.Invoke();
					state = TimerState.Done;
					TimerManager.Instance.RemoveTimer(this);
				}

			}
		}
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="T:Timer"/> is running.
	/// </summary>
	/// <value><c>true</c> if is running; otherwise, <c>false</c>.</value>
	public bool IsRunning
	{
		get
		{
			return state == TimerState.Running;
		}
	}

    public int TicksCompleted {
        get {
            return timesRun;
        }
    }
}

/// <summary>
/// Timer states.
/// </summary>
public enum TimerState {
    NotStarted,
    Running,
    Stopped,
    Paused,
    Done
}
