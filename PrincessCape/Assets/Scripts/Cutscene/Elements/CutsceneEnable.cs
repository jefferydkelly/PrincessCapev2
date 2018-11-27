using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneEnable : CutsceneElement
{
    bool useObject = false;
    GameObject hideObject;
    bool enable = true;
    bool move = false;
    Vector2 pos;
    string objectName = "";

    public CutsceneEnable() {
        autoAdvance = true;
        canSkip = false;
        type = CutsceneElements.Enable;
    }
    public override string SaveData
    {
        get
        {
            string data = PCLParser.CreateAttribute("Use Game Object?", useObject);
            if (useObject)
            {
                data += PCLParser.CreateAttribute("Object Name", hideObject.name);
            }
            else
            {
                data += PCLParser.CreateAttribute("Object Name", objectName);
            }

            data += PCLParser.CreateAttribute("Enable Object", enable);
            data += PCLParser.CreateAttribute("Move Object?", move);
            if (move)
            {
                data += PCLParser.CreateAttribute("Move To", pos);
            }

            return data;
        }
    }

    public override string ToText {
        get {
             if (move) {
                return string.Format("{0} {1} {2} {3}", enable ? "enable" : "disable", objectName, pos.x, pos.y);
            }
            
            return string.Format("{0} {1}", enable ? "enable" : "disable", objectName);
        }
    }

    public override Timer Run()
    {
        if (hideObject == null || !hideObject.activeInHierarchy)
        {
            hideObject = Cutscene.Instance.FindGameObject(objectName);
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

    public override void Skip()
    {
        Run();
    }

#if UNITY_EDITOR
    public override void RenderEditor()
    {
        useObject = EditorGUILayout.Toggle("Use GameObject?", useObject);
        if (useObject)
        {
            hideObject = EditorGUILayout.ObjectField("Object", hideObject, typeof(GameObject), true) as GameObject;
        }
        else
        {
            objectName = EditorGUILayout.TextField("Name", objectName);
        }
        enable = EditorGUILayout.Toggle("Enable", enable);
        move = EditorGUILayout.Toggle("Move Object", move);
        if (move)
        {
            pos = EditorGUILayout.Vector2Field("Move To", pos);
        }
    }
#endif

    public override void CreateFromJSON(string[] data)
    {
        useObject = PCLParser.ParseBool(data[0]);
        if (useObject)
        {
            string objName = PCLParser.ParseLine(data[1]);
            hideObject = GameObject.Find(objName);
        }
        else
        {
            objectName = PCLParser.ParseLine(data[1]);
        }
        enable = PCLParser.ParseBool(data[2]);
        move = PCLParser.ParseBool(data[3]);
        if (move)
        {
            pos = PCLParser.ParseVector2(data[2]);
        }
    }

    public override void CreateFromText(string[] data)
    {
        enable = data[0] == "enable";
        objectName = data[1];
        GameObject found = Cutscene.Instance.FindGameObject(objectName);
        if (found)
        {
            hideObject = found;
            useObject = true;
        }
        if (data.Length > 2 && data[data.Length - 1] != "and")
        {
            pos = new Vector2(float.Parse(data[2]), float.Parse(data[3]));

        }
    }
}