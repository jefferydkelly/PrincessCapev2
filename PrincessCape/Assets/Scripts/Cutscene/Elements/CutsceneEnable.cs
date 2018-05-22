using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEnable : CutsceneElement
{
    GameObject hideObject;
    bool enable = true;
    bool move = false;
    Vector2 pos;
    string objectName = "";

    public CutsceneEnable(GameObject go, bool en)
    {
        hideObject = go;
        enable = en;
        autoAdvance = true;
        canSkip = false;
    }

    public CutsceneEnable(GameObject go, float x, float y) : this(go, true)
    {
        move = true;
        pos = new Vector2(x, y);
    }

    public CutsceneEnable(string oName, bool en)
    {
        hideObject = null;
        objectName = oName;
        enable = en;
    }

    public override Timer Run()
    {
        if (hideObject == null)
        {
            hideObject = GameObject.Find(objectName);
            Debug.Log(string.Format("{0}:{1}", objectName, hideObject == null));
        }
        if (hideObject)
        {
            hideObject.SetActive(enable);

            if (move)
            {
                hideObject.transform.position = pos;

            }

        }

        return null;
    }
}

