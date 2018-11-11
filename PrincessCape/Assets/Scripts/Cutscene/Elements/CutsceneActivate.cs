using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CutsceneActivate : CutsceneElement
{
    [SerializeField]
    bool activate;
    [SerializeField]
    ActivatedObject activatedObject;
    string objectName;

    public CutsceneActivate() {
        autoAdvance = true;
        canSkip = false;
        type = CutsceneElements.Activate;
    }

    public override Timer Run()
    {

        if (activatedObject == null)
        {
            GameObject gameObject = Cutscene.Instance.FindGameObject(objectName);
            activatedObject = gameObject.GetComponent<ActivatedObject>();
        }
        if (activate)
        {
            activatedObject.Activate();
        }
        else
        {
            activatedObject.Deactivate();
        }

        return null;
    }

    public override void Skip()
    {
        Run();
    }

#if UNITY_EDITOR
    public override void RenderEditor() {
        activatedObject = EditorGUILayout.ObjectField("Activated Object", activatedObject, typeof(ActivatedObject), true) as ActivatedObject;
        activate = EditorGUILayout.Toggle("Is Activated", activate);
    }
#endif 

    public override void CreateFromText(string[] data)
    {
        GameObject gameObject = GameObject.Find(data[1]);
        if (!gameObject)
        {
            gameObject = FindTile(data[1]);
        }

        if (gameObject)
        {
            activatedObject = gameObject.GetComponent<ActivatedObject>();
        }
        activate = bool.Parse(data[2]);

       
    }

    public override void CreateFromJSON(string[] data)
    {
        activatedObject = GameObject.Find(PCLParser.ParseLine(data[0])).GetComponent<ActivatedObject>();
        activate = PCLParser.ParseBool(data[1]);
    }

    public override string ToText
    {
        get
        {
            return string.Format("activate {0} {1}", activatedObject.InstanceName, activate);
        }
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Activated Object", activatedObject.InstanceName);
            data += PCLParser.CreateAttribute("Is Activated", activate);
            return data;
        }
    }
}