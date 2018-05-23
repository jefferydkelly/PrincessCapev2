using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneAlign : CutsceneElement
{
    bool left;

    public CutsceneAlign(bool l)
    {
        left = l;
        autoAdvance = true;
        canSkip = true;
    }

    public override Timer Run()
    {
        if (left)
		{
			UIManager.Instance.Alignment = TextAnchor.UpperLeft;
		}
        else
        {
			UIManager.Instance.Alignment = TextAnchor.UpperRight;
        }

        return null;
    }
}
