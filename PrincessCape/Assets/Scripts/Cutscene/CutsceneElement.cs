using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// Cutscene element.
/// </summary>
public class CutsceneElement
{
	public CutsceneElement nextElement = null;
	public CutsceneElement prevElement = null;
	protected bool canSkip = false;

	protected bool autoAdvance = false;
	protected Timer runTimer;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:CutsceneElement"/> advances automatically.
    /// </summary>
    /// <value><c>true</c> if auto advance; otherwise, <c>false</c>.</value>
	public bool AutoAdvance
	{
		get
		{
			return autoAdvance;
		}
	}

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:CutsceneElement"/> can be skipped.
    /// </summary>
    /// <value><c>true</c> if can be skipped; otherwise, <c>false</c>.</value>
	public bool CanSkip
	{
		get
		{
			return canSkip;
		}
	}

    /// <summary>
    /// Run this instance.
    /// </summary>
	public virtual Timer Run()
	{
		return null;
	}

    /// <summary>
    /// Skip this instance.
    /// </summary>
    public virtual void Skip() {
        
    }
}

/*
public class CutscenePlay : CutsceneElement
{
	AudioClip soundEffect;
	public CutscenePlay(string s)
	{
		soundEffect = Resources.Load<AudioClip>("Sounds/" + s);
		autoAdvance = true;
		canSkip = true;
	}

	public override void Run()
	{
		AudioManager.Instance.PlaySound(soundEffect);
	}
}*/

public enum CutsceneElements
{
	Dialog, Move, Effect, SpriteChange
}