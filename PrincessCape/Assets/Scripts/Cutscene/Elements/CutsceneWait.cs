using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cutscene wait.
/// </summary>
public class CutsceneWait : CutsceneElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CutsceneWait"/> class.
    /// </summary>
    /// <param name="dt">The duration of the wait.</param>
    public CutsceneWait(float dt)
    {
        canSkip = true;
        runTimer = new Timer(dt);
		autoAdvance = true;

    }
    public override Timer Run()
    {
		return runTimer;
    }
}
