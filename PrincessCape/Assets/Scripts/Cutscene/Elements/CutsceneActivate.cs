using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneActivate : CutsceneElement
{
    bool activate;
    ActivatedObject ao;

    public CutsceneActivate(ActivatedObject aObj, bool activated)
    {
        ao = aObj;
        activate = activated;
        autoAdvance = true;
        canSkip = false;
    }

    public override Timer Run()
    {
        if (activate)
        {
            ao.Activate();
        }
        else
        {
            ao.Deactivate();
        }

        return null;
    }
}
