using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CutsceneEditor : EditorWindow {

    private static CutsceneEditor instance;
    List<CutsceneStep> steps;

    [MenuItem("My Game/Cutscene Editor")]
    public static void ShowWindow() {
        instance = GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
        instance.steps = new List<CutsceneStep>();
        instance.steps.Add(new CutsceneStep());
    }

    private void OnGUI()
    {
        if (instance != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) {
                //Save the cutscene
            } else if (GUILayout.Button("Load")) {
                //Open a file browser to load a cutscene
            }
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < instance.steps.Count; i++)
            {
                EditorGUI.indentLevel = 0;
                CutsceneStep step = instance.steps[i];
                step.Show = EditorGUILayout.Foldout(step.Show, string.Format("Step {0}", i + 1), true);
                if (step.Show)
                {
                    EditorGUI.indentLevel = 1;
                    instance.steps[i].DrawGUI();
                }
                EditorGUILayout.Separator();
            }

            if (GUILayout.Button("Add Step"))
            {
                instance.steps.Add(new CutsceneStep());
            }
        }
    }

}

public class CutsceneStep {
    bool show = true;
    List<CutsceneElementEditor> elements = new List<CutsceneElementEditor>();
    public void DrawGUI() {
        EditorGUILayout.BeginVertical();

        foreach (CutsceneElementEditor cee in elements) {
            cee.Render();
            EditorGUILayout.Separator();
        }
        if (GUILayout.Button("Add Element"))
        {
            GenericMenu menu = new GenericMenu();

            string[] types = System.Enum.GetNames(typeof(CutsceneElements));

            foreach(string type in types) {
                menu.AddItem(new GUIContent(type), false, AddElement, type);
            }
            /*
            foreach (string s in items)
            {
                menu.AddItem(new GUIContent(s), false, AddElement, s);
            }*/
            menu.ShowAsContext();
        }
        EditorGUILayout.EndVertical();
    }

    void AddElement(object type)
    {
        CutsceneElements eType = (CutsceneElements)System.Enum.Parse(typeof(CutsceneElements), (string)type);
        switch(eType) {
            case CutsceneElements.Dialog:
                elements.Add(new DialogEditor());
                break;
            case CutsceneElements.Activate:
                elements.Add(new ActivateEditor());
                break;
            case CutsceneElements.Add:
                elements.Add(new AddEditor());
                break;
            case CutsceneElements.Animation:
                elements.Add(new AnimationEditor());
                break;
            default:
                break;
                
        }
    }

    public bool Show {
        get {
            return show;
        }

        set {
            show = value;
        }
    }
}

public abstract class CutsceneElementEditor {
    bool show = true;
    protected string editorType = "Element";
    public void Render() {
        EditorGUILayout.BeginVertical();
        show = EditorGUILayout.Foldout(show, editorType, true);
        if (show)
        {
            EditorGUI.indentLevel = 2;
            DrawGUI();

        }
        EditorGUILayout.EndVertical();
    }

    protected abstract void DrawGUI();
    public abstract string GenerateSaveData();
    public abstract void GenerateFromData(string[] data);
}

public class DialogEditor : CutsceneElementEditor {
    string speaker = "";
    string line = "";

    public DialogEditor() {
        editorType = "Dialog";
    }
    protected override void DrawGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Speaker");
        speaker = EditorGUILayout.TextArea(speaker);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Line");
        line = EditorGUILayout.TextArea(line);
        EditorGUILayout.EndHorizontal();
    }

    public override string GenerateSaveData()
    {
        string data = PCLParser.CreateAttribute<string>("Speaker", speaker);
        data += PCLParser.CreateAttribute<string>("Line", line);
        return data;
    }

    public override void GenerateFromData(string[] data)
    {
        speaker = PCLParser.ParseLine(data[0]);
        line = PCLParser.ParseLine(data[1]);
    }
}

public class ActivateEditor : CutsceneElementEditor
{
    ActivatedObject activated;
    bool isActivated = false;

    public ActivateEditor() {
        editorType = "Activation";
    }
    protected override void DrawGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Activated Object");
        activated = EditorGUILayout.ObjectField(activated, typeof(ActivatedObject), true) as ActivatedObject;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Activated");
        isActivated = EditorGUILayout.Toggle(isActivated);
        EditorGUILayout.EndHorizontal();
    }

    public override void GenerateFromData(string[] data)
    {
        activated = GameObject.Find(PCLParser.ParseLine(data[0])).GetComponent<ActivatedObject>();
        isActivated = PCLParser.ParseBool(data[1]);
    }

    public override string GenerateSaveData()
    {
        string data = PCLParser.CreateAttribute<string>("Object",activated.InstanceName);
        data += PCLParser.CreateAttribute<bool>("Is Activated", isActivated);
        return data;
    }
}

public class AddEditor : CutsceneElementEditor
{
    ItemLevel item;
    public AddEditor() {
        editorType = "Add Magic Item";
    }
    protected override void DrawGUI()
    {
        item = (ItemLevel)EditorGUILayout.EnumPopup("Magic Item", item);
    }

    public override void GenerateFromData(string[] data)
    {
        item = PCLParser.ParseEnum<ItemLevel>(data[0]);
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("Magic Item", item);
    }
}

public class AnimationEditor : CutsceneElementEditor
{
    GameObject gameObject;
    Animator animator;
    string animation;

    public AnimationEditor() {
        editorType = "Play Animation";
    }

    protected override void DrawGUI()
    {
        GameObject oldObject = gameObject;
        gameObject = EditorGUILayout.ObjectField("Game Object", gameObject, typeof(GameObject), true) as GameObject;
        if (oldObject != gameObject) {
            animator = gameObject.GetComponent<Animator>();
        }


        if (animator) {
            //Make a list of the animations and list them to be selected
        } else {
            EditorGUILayout.LabelField("This does not have an animator");
        }

    }

    public override void GenerateFromData(string[] data)
    {
        throw new System.NotImplementedException();
    }

    public override string GenerateSaveData()
    {
        throw new System.NotImplementedException();
    }


}

public enum CutsceneElements {
    Activate,
    Add,
    Align,
    Animation,
    Creation,
    Dialog,
    Effect,
    Enable,
    Fade,
    Follow,
    Movement,
    Pan,
    Play,
    Scale,
    Wait,
    Change
}