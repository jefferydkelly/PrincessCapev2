using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        objectName = go.name;
        enable = en;
        autoAdvance = true;
        canSkip = false;
    }

    public CutsceneEnable(GameObject go, float x, float y) : this(go, true)
    {
        move = true;
        pos = new Vector2(x, y);
        objectName = go.name;
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

#if UNITY_EDITOR
public class EnableEditor : CutsceneElementEditor
{
    bool useObject = true;
    bool move = false;
    GameObject hideObject;
    bool enable = true;
    Vector2 pos;
    string objectName = "";
    public EnableEditor()
    {
        editorType = "Enable Object";
        type = CutsceneElements.Enable;
    }

    public override void GenerateFromData(string[] data)
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

    public override string GenerateSaveData()
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

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
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

    public override string HumanReadable
    {
        get
        {
            if (move) {
                return string.Format("{0} {1} {2} {3}", enable ? "enable" : "disable", objectName, pos.x, pos.y);
            }
            
            return string.Format("{0} {1}", enable ? "enable" : "disable", objectName);
        }
    }

    public override void GenerateFromText(string[] data)
    {
        enable = data[0] == "enable";
        objectName = data[1];
        GameObject found = GameObject.Find(objectName);
        if (found) {
            hideObject = found;
            useObject = true;
        }
        if (data.Length > 2 && data[data.Length - 1] != "and") {
            pos = new Vector2(float.Parse(data[2]), float.Parse(data[3]));

        }
    }
}
#endif