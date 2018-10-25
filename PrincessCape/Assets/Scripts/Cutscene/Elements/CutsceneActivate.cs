using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
public class ActivateEditor : CutsceneElementEditor
{
    ActivatedObject activated;
    bool isActivated = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ActivateEditor"/> class.
    /// </summary>
    public ActivateEditor()
    {
        editorType = "Activation";
        type = CutsceneElements.Activate;
    }
    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {

        activated = EditorGUILayout.ObjectField("Activated Object", activated, typeof(ActivatedObject), true) as ActivatedObject;
        isActivated = EditorGUILayout.Toggle("Is Activated", isActivated);
    }

    /// <summary>
    /// Populates the properties of the class from the given data
    /// </summary>
    /// <param name="data">Data.</param>
    public override void GenerateFromData(string[] data)
    {
        activated = GameObject.Find(PCLParser.ParseLine(data[0])).GetComponent<ActivatedObject>();
        isActivated = PCLParser.ParseBool(data[1]);
    }

    /// <summary>
    /// Generates the save data.
    /// </summary>
    /// <returns>The save data.</returns>
    /// <param name="json">If set to <c>true</c> json.  Human readable otherwise</param>
    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute<string>("Object", activated.InstanceName);
        data += PCLParser.CreateAttribute<bool>("Is Activated", isActivated);
        return data;
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("activate {0} {1}", activated.InstanceName, isActivated);
        }
    }

    public override void GenerateFromText(string[] data)
    {
        GameObject gameObject = GameObject.Find(data[1]);
        if (!gameObject) {
            gameObject = FindTile(data[1]);
        }

        if (gameObject)
        {
            activated = gameObject.GetComponent<ActivatedObject>();
        } 
        isActivated = bool.Parse(data[2]);
    }
}
#endif