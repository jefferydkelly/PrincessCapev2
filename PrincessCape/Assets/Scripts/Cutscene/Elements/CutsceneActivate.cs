using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneActivate : CutsceneElement
{
    bool activate;
    ActivatedObject ao;
    string objectName;

    public CutsceneActivate(ActivatedObject aObj, bool activated)
    {
        ao = aObj;
        activate = activated;
        autoAdvance = true;
        canSkip = false;
    }

    public CutsceneActivate(string objName, bool activated)
    {
        objectName = objName;
        activate = activated;
        autoAdvance = true;
        canSkip = false;
    }

    public override Timer Run()
    {

        if (ao == null)
        {
            GameObject gameObject = Cutscene.Instance.FindGameObject(objectName);
            ao = gameObject.GetComponent<ActivatedObject>();
        }
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