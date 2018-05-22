using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A container for cutscene dialog
/// </summary>
public class CutsceneDialog : CutsceneElement
{
    string speaker = "Character";
    string dialog = "Hi, I'm a character";

    /// <summary>
    /// Initializes a new instance of the <see cref="CutsceneDialog"/> class with a speaker and a line.
    /// </summary>
    /// <param name="spk">Spk.</param>
    /// <param name="dia">Dia.</param>
    public CutsceneDialog(string spk, string dia)
    {
        speaker = spk;
        dialog = dia.Replace("\\n", "\n").Trim();
        canSkip = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CutsceneDialog"/> class for duration.
    /// </summary>
    /// <param name="dia">Dia.</param>
    public CutsceneDialog(string dia)
    {
        speaker = null;
        dialog = dia.Replace("\\n", "\n").Trim();
    }

    /// <summary>
    /// Displays the dialog of message
    /// </summary>
    public override Timer Run()
    {
        return UIManager.Instance.ShowMessage(dialog, speaker);
    }
}