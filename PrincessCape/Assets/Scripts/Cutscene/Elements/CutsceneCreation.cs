using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneCreation : CutsceneElement
{
    GameObject prefab;
    Vector3 position;
    bool destroy = false;
    bool useObject = false;
    string prefabName;

    public override string SaveData
    {
        get
        {
            if (destroy) {
                return PCLParser.CreateAttribute("Object Name", useObject? prefab.name : prefabName);
            } else {
                string data = "";
                data += PCLParser.CreateAttribute("Prefab", AssetDatabase.GetAssetPath(prefab));
                data += PCLParser.CreateAttribute<Vector3>("Position", position);
                return data;
            }
        }
    }

    public override string ToText {
        get {
            if (destroy) {
                return string.Format("destroy {0}", prefab.name);
            } else {
                return string.Format("create {0} {1} {2} {3}", prefab.name, position.x, position.y, position.z);
            }
        }
    }

    public CutsceneCreation(bool dest) {
        destroy = dest;
        autoAdvance = true;
        canSkip = false;

        type = dest ? CutsceneElements.Destroy : CutsceneElements.Creation;
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
            if (!prefab) {
                prefab = GameObject.Find(prefabName);
            }
            if (prefab)
            {
                Object.Destroy(prefab);
            }
        }

        return null;
    }

    public override void Skip()
    {
        Run();
    }
#if UNITY_EDITOR
    public override void RenderEditor()
    {
        destroy = EditorGUILayout.Toggle("Destroy Object", destroy);
        useObject = EditorGUILayout.Toggle("Use Object?", useObject);
        if (useObject)
        {
            prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;
        } else {
            prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
        }
        position = EditorGUILayout.Vector3Field("Position", position);
    }
#endif

    public override void CreateFromJSON(string[] data)
    {
        if (destroy)
        {
            prefab = GameObject.Find(PCLParser.ParseLine(data[0]));
        } else {
           
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PCLParser.ParseLine(data[0]));
            position = PCLParser.ParseVector3(data[1]); 
        }
    }

    public override void CreateFromText(string[] data)
    {
        if (destroy)
        {
            prefabName = data[1];
            prefab = GameObject.Find(prefabName);
            useObject = prefab != null;
        }
        else
        {
            prefab = Resources.Load<GameObject>("Prefabs/" + data[1]);
            position = new Vector3(float.Parse(data[2]), float.Parse(data[3]), float.Parse(data[4]));
        }
    }
}