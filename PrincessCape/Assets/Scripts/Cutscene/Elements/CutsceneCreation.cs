using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCreation : CutsceneElement
{
    GameObject prefab;
    Vector3 position;
    string objectName;
    bool destroy = false;
    public CutsceneCreation(string name, string dx, string dy, string dz)
    {
        prefab = Resources.Load<GameObject>("Prefabs/" + name);
        position = new Vector3(float.Parse(dx), float.Parse(dy), float.Parse(dz));
        autoAdvance = true;
        canSkip = false;
    }

    public CutsceneCreation(string name)
    {
        objectName = name;
        destroy = true;
        autoAdvance = true;
        canSkip = false;
    }

    public override Timer Run()
    {
        if (!destroy)
        {
            GameObject go = Object.Instantiate(prefab);
            go.name = prefab.name;
            go.transform.position = position;
        }
        else
        {
            GameObject go = GameObject.Find(objectName);
            if (go)
            {
                Object.Destroy(go);
            }
        }

        return null;
    }
}

