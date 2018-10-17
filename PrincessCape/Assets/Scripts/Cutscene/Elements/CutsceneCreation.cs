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

#if UNITY_EDITOR
public class CreationEditor : CutsceneElementEditor
{
    GameObject prefab;
    Vector3 position;

    public CreationEditor()
    {
        editorType = "Create Object";
        type = CutsceneElements.Creation;
    }
    public override void GenerateFromData(string[] data)
    {
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PCLParser.ParseLine(data[0]));
        position = PCLParser.ParseVector3(data[1]);
    }

    public override string GenerateSaveData(bool json)
    {
        string data = "";
        data += PCLParser.CreateAttribute("Prefab", AssetDatabase.GetAssetPath(prefab));
        data += PCLParser.CreateAttribute<Vector3>("Position", position);
        return data;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;
        position = EditorGUILayout.Vector3Field("Position", position);
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("create {0} {1} {2} {3}", prefab.name, position.x, position.y, position.z);
        }
    }
}

public class DestructionEditor : CutsceneElementEditor
{
    GameObject toBeDestroyed;
    public DestructionEditor()
    {
        editorType = "Destroy an object";
        type = CutsceneElements.Destroy;
    }
    public override void GenerateFromData(string[] data)
    {
        toBeDestroyed = GameObject.Find(PCLParser.ParseLine(data[0]));
    }

    public override string GenerateSaveData(bool json)
    {
        return PCLParser.CreateAttribute("Object Name", toBeDestroyed.name);
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        toBeDestroyed = EditorGUILayout.ObjectField("Object", toBeDestroyed, typeof(GameObject), true) as GameObject;
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("destroy {0}", toBeDestroyed.name);
        }
    }
}
#endif
