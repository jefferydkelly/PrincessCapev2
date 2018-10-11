using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CutsceneEditor : EditorWindow {

    private static CutsceneEditor instance;
    List<CutsceneStep> steps;
    CutsceneInfo info;

    [MenuItem("My Game/Cutscene Editor")]
    public static void ShowWindow() {
        instance = GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
        instance.steps = new List<CutsceneStep>();
        instance.steps.Add(new CutsceneStep());
        instance.info = new CutsceneInfo();
    }

    private void OnGUI()
    {
        if (instance != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) {
                //Save the cutscene
                string path = EditorUtility.SaveFilePanel("Save Cutscene To File", "Assets/Resources/Cutscenes", instance.info.CutsceneName, "json");

                if (path.Length > 0)
                {
                    string data = PCLParser.StructStart;
                    data += instance.info.SaveData;
                    int i = 1;
                    data += PCLParser.CreateArray("Steps");
                    foreach (CutsceneStep step in instance.steps)
                    {
                        data += PCLParser.StructStart;
                        data += PCLParser.CreateAttribute("Step Number", i);
                        data += step.SaveData;
                        data += PCLParser.StructEnd;
                        i++;
                    }
                    data += PCLParser.ArrayEnding;
                    data += PCLParser.StructEnd;
                    File.WriteAllText(path, data);
                }
            } else if (GUILayout.Button("Load")) {
                string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Cutscenes", "json");
                if (path.Length > 0)
                {

                    CutsceneFile file = PCLParser.ParseCutsceneFile(File.ReadAllText(path));
                    instance.info.CutsceneName = file.cutsceneName;
                    instance.info.Characters = file.characters;

                    instance.steps.Clear();
                    foreach(CutsceneStepStruct step in file.steps) {
                        instance.steps.Add(new CutsceneStep(step.elements));
                    }

                }
            }
            EditorGUILayout.EndHorizontal();
            instance.info.Render();
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

public class CutsceneInfo
{
    string cutsceneName = "Cutscene";
    Dictionary<string, bool> charactersInScene;

    public CutsceneInfo()
    {
        charactersInScene = new Dictionary<string, bool>();
        foreach (CutsceneActor actor in Resources.LoadAll<CutsceneActor>("Characters"))
        {
            charactersInScene.Add(actor.CharacterName, false);
        }

    }
    public void Render()
    {
        cutsceneName = EditorGUILayout.TextField("Scene Name", cutsceneName);
        EditorGUILayout.LabelField("Characters In Scene");
        Dictionary<string, bool> copy = new Dictionary<string, bool>();
        charactersInScene.Copy(copy);
        foreach (string name in charactersInScene.Keys)
        {
            copy[name] = EditorGUILayout.Toggle(name, charactersInScene[name]);
        }
        charactersInScene = copy;
    }

    public string CutsceneName {
        get {
            return cutsceneName;
        }

        set {
            if (value.Length > 0) {
                cutsceneName = value;
            }
        }
    }

    public string[] Characters {
        set {
            foreach(string character in value) {
                if (charactersInScene.ContainsKey(character)) {
                    charactersInScene[character] = true;
                }
            }
        }
    }

    public string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Cutscene Name", cutsceneName);

            string characters = "";
            foreach(KeyValuePair<string, bool> kvp in charactersInScene) {
                if (kvp.Value) {
                    characters += kvp.Key + " ";
                }
            }
            data += PCLParser.CreateAttribute("Characters In Scene", characters);
            return data;
        }
    }

}
public class CutsceneStep {
    bool show = true;
    List<CutsceneElementEditor> elements = new List<CutsceneElementEditor>();

    public CutsceneStep() {
        
    }

    public CutsceneStep(List<CutsceneElementStruct> els) {
        foreach(CutsceneElementStruct ces in els) {
            CutsceneElementEditor cee = ParseElement(ces.type);
            cee.GenerateFromData(ces.info.ToArray());
            elements.Add(cee);
        }
    }
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

            menu.ShowAsContext();
        }
        EditorGUILayout.EndVertical();
    }

    void AddElement(object type)
    {
        CutsceneElements eType = (CutsceneElements)System.Enum.Parse(typeof(CutsceneElements), (string)type);
        CutsceneElementEditor ed = ParseElement(eType);
        if (ed != null) {
            elements.Add(ed);
        }
       
    }

    public CutsceneElementEditor ParseElement(CutsceneElements ce) {
        switch (ce)
        {
            
                
            case CutsceneElements.Activate:
                return new ActivateEditor();
            case CutsceneElements.Add:
                return new AddEditor();
            case CutsceneElements.Align:
                return new AlignmentEditor();
            case CutsceneElements.Animation:
                return new AnimationEditor();
            case CutsceneElements.Creation:
                return new CreationEditor();
            case CutsceneElements.Destroy:
                return new DestructionEditor();
            case CutsceneElements.Dialog:
                return new DialogEditor();
            case CutsceneElements.Fade:
                return new FadeEditor();
            case CutsceneElements.Flip:
                return new FlipEditor();
            case CutsceneElements.Follow:
                return new FollowEditor();
            case CutsceneElements.Hide:
                return new HideEditor();
            case CutsceneElements.Movement:
                return new MovementEditor();
            case CutsceneElements.Pan:
                return new PanEditor();
            case CutsceneElements.Play:
                return new PlayEditor();
            case CutsceneElements.Rotate:
                return new RotationEditor();
            case CutsceneElements.Scale:
                return new ScaleEditor();
            case CutsceneElements.Show:
                return new ScaleEditor();
            case CutsceneElements.Wait:
                return new WaitEditor();
            default:
                break;

        }

        return null;
    }

    public bool Show {
        get {
            return show;
        }

        set {
            show = value;
        }
    }

    public string SaveData {
        get {
            string data = "";
            data += PCLParser.CreateArray("Elements");
            foreach(CutsceneElementEditor ed in elements) {
                data += ed.SaveData;
            }
            data += PCLParser.ArrayEnding;
            return data;
        }
    }
}

public abstract class CutsceneElementEditor {
  
    bool show = true;
    protected string editorType = "Element";
    protected CutsceneElements type;
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

    public string SaveData {
        get {
            string data = PCLParser.StructStart;
            data += PCLParser.CreateAttribute("Element Type", type);
            data += GenerateSaveData();
            data += PCLParser.StructEnd;
            return data;
        }
    }

    protected abstract void DrawGUI();
    public abstract string GenerateSaveData();
    public abstract void GenerateFromData(string[] data);
}


public class ActivateEditor : CutsceneElementEditor
{
    ActivatedObject activated;
    bool isActivated = false;

    public ActivateEditor()
    {
        editorType = "Activation";
        type = CutsceneElements.Activate;
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
        string data = "";
        data += PCLParser.CreateAttribute<string>("Object", activated.InstanceName);
        data += PCLParser.CreateAttribute<bool>("Is Activated", isActivated);
        return data;
    }
}

public class AddEditor : CutsceneElementEditor
{
    ItemLevel item;
    public AddEditor() {
        editorType = "Add Magic Item";
        type = CutsceneElements.Add;
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

public class AlignmentEditor : CutsceneElementEditor
{
    bool left = true;

    public AlignmentEditor() {
        editorType = "Set Text Alignment";
        type = CutsceneElements.Align;
    }

    public override void GenerateFromData(string[] data)
    {
        left = PCLParser.ParseBool(data[0]);
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("Is Left?", left);
    }

    protected override void DrawGUI()
    {
        left = EditorGUILayout.Toggle("Is Left Algined?", left);
    }
}

public class AnimationEditor : CutsceneElementEditor
{
    GameObject gameObject;
    Animator animator;
    string animation;

    public AnimationEditor() {
        editorType = "Play Animation";
        type = CutsceneElements.Animation;
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

public class CreationEditor : CutsceneElementEditor
{
    GameObject prefab;
    Vector3 position;

    public CreationEditor() {
        editorType = "Create Object";
        type = CutsceneElements.Creation;
    }
    public override void GenerateFromData(string[] data)
    {
        throw new System.NotImplementedException();
    }

    public override string GenerateSaveData()
    {
        throw new System.NotImplementedException();
    }

    protected override void DrawGUI()
    {
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;
        position = EditorGUILayout.Vector3Field("Position", position);
    }
}

public class DestructionEditor : CutsceneElementEditor
{
    GameObject toBeDestoryed;
    public DestructionEditor() {
        editorType = "Destroy an object";
        type = CutsceneElements.Destroy;
    }
    public override void GenerateFromData(string[] data)
    {
        throw new System.NotImplementedException();
    }

    public override string GenerateSaveData()
    {
        throw new System.NotImplementedException();
    }

    protected override void DrawGUI()
    {
        toBeDestoryed = EditorGUILayout.ObjectField("Object", toBeDestoryed, typeof(GameObject),true) as GameObject;
    }
}
public class DialogEditor : CutsceneElementEditor
{
    string speaker = "";
    string line = "";

    public DialogEditor()
    {
        editorType = "Dialog";
        type = CutsceneElements.Dialog;
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

public class EnableEditor : CutsceneElementEditor
{
    bool useObject = true;
    bool move = false;
    GameObject hideObject;
    bool enable = true;
    Vector2 pos;
    string objectName = "";
    public EnableEditor() {
        editorType = "Enable Object";
        type = CutsceneElements.Enable;
    }
        
    public override void GenerateFromData(string[] data)
    {
        useObject = PCLParser.ParseBool(data[0]);
        if (useObject) {
            string objName = PCLParser.ParseLine(data[1]);
            hideObject = GameObject.Find(objName);
        } else {
            objectName = PCLParser.ParseLine(data[1]);
        }
        enable = PCLParser.ParseBool(data[2]);
        move = PCLParser.ParseBool(data[3]);
        if (move) {
            pos = PCLParser.ParseVector2(data[2]);
        }
    }

    public override string GenerateSaveData()
    {
        string data = PCLParser.CreateAttribute("Use Game Object?", useObject);
        if (useObject) {
            data += PCLParser.CreateAttribute("Object Name", hideObject.name);
        } else {
            data += PCLParser.CreateAttribute("Object Name", objectName);
        }

        data += PCLParser.CreateAttribute("Enable Object", enable);
        data += PCLParser.CreateAttribute("Move Object?", move);
        if (move) {
            data += PCLParser.CreateAttribute("Move To", pos);
        }

        return data;
    }

    protected override void DrawGUI()
    {
        useObject = EditorGUILayout.Toggle("Use GameObject?", useObject);
        if (useObject) {
            hideObject = EditorGUILayout.ObjectField("Object", hideObject, typeof(GameObject), true) as GameObject;
        } else {
            objectName = EditorGUILayout.TextField("Name", objectName);
        }
        enable = EditorGUILayout.Toggle("Enable", enable);
        move = EditorGUILayout.Toggle("Move Object", move);
        if (move) {
            pos = EditorGUILayout.Vector2Field("Move To", pos);
        }

    }
}

public class FadeEditor : CutsceneElementEditor
{
    string actorName;
    float alpha;
    float time;

    public FadeEditor() {
        editorType = "Fade In/Out";
        type = CutsceneElements.Fade;
    }
    public override void GenerateFromData(string[] data)
    {
        actorName = PCLParser.ParseLine(data[0]);
        alpha = PCLParser.ParseFloat(data[1]);
        time = PCLParser.ParseFloat(data[2]);
    }

    public override string GenerateSaveData()
    {
        string data = PCLParser.CreateAttribute("Actor", actorName);
        data += PCLParser.CreateAttribute("Fade To", alpha);
        data += PCLParser.CreateAttribute("Over", time);
        return data;
    }

    protected override void DrawGUI()
    {
        actorName = EditorGUILayout.TextField("Actor", actorName);
        alpha = EditorGUILayout.FloatField("Fade To Alpha", alpha);
        alpha = Mathf.Clamp01(alpha);
        float newTime = EditorGUILayout.FloatField("Time (in seconds)", time);
        if (newTime > 0) {
            time = newTime;
        }
    }
}

public class FlipEditor : CutsceneElementEditor
{
    bool horizontal = true;

    public FlipEditor() {
        editorType = "Flip Sprite";
        type = CutsceneElements.Flip;
    }
    public override void GenerateFromData(string[] data)
    {
        horizontal = PCLParser.ParseBool(data[0]);
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("Horizontal", horizontal);
    }

    protected override void DrawGUI()
    {
        horizontal = EditorGUILayout.Toggle("Flip Horizontal", horizontal);
    }
}

public class FollowEditor : CutsceneElementEditor
{
    string followedName;
    public FollowEditor() {
        editorType = "Follow Character";
        type = CutsceneElements.Follow;
    }
    public override void GenerateFromData(string[] data)
    {
        followedName = PCLParser.ParseLine(data[0]);
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("Follow", followedName);
    }

    protected override void DrawGUI()
    {
        followedName = EditorGUILayout.TextField("Follow", followedName);
    }
}

public class HideEditor : CutsceneElementEditor
{
    string hideName;

    public HideEditor() {
        editorType = "Hide Object";
        type = CutsceneElements.Hide;
    }
    public override void GenerateFromData(string[] data)
    {
        hideName = PCLParser.ParseLine(data[0]);
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("To Be Hidden", hideName);
    }

    protected override void DrawGUI()
    {
        hideName = EditorGUILayout.TextArea("To Be Hidden", hideName);
    }
}

public class MovementEditor : CutsceneElementEditor
{
    bool useObject = true;
    GameObject gameObject;
    string name;
    Vector3 moveTo;
    float time;
    public MovementEditor()
    {
        editorType = "Move Object";
        type = CutsceneElements.Movement;
    }

    public override void GenerateFromData(string[] data)
    {
        useObject = PCLParser.ParseBool(data[0]);
        if (useObject) {
            gameObject = GameObject.Find(PCLParser.ParseLine(data[1]));
        } else {
            name = PCLParser.ParseLine(data[1]);
        }
        moveTo = PCLParser.ParseVector3(data[2]);
        time = PCLParser.ParseFloat(data[3]);
    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Use Object", useObject);
        data += PCLParser.CreateAttribute("Object Name", useObject ? gameObject.name : name);
        data += PCLParser.CreateAttribute("Move To", moveTo);
        data += PCLParser.CreateAttribute("Over", time);
        return data;
    }

    protected override void DrawGUI()
    {
        useObject = EditorGUILayout.Toggle("Use GameObject?", useObject);
        if (useObject)
        {
            gameObject = EditorGUILayout.ObjectField("Object", gameObject, typeof(GameObject), true) as GameObject;
        }
        else
        {
            name = EditorGUILayout.TextField("Name", name);
        }
        moveTo = EditorGUILayout.Vector3Field("Move To", moveTo);
        float newTime = EditorGUILayout.FloatField("Over", time);
        if (newTime > 0) {
            time = newTime;
        }
    }
}

public class PanEditor: CutsceneElementEditor {
    PanType pType = PanType.ToPosition;
    Vector2 panDistance = Vector2.zero;
    Vector3 panEnding;
    string panToName = "";
    float panTime = 1.0f;

    public PanEditor() {
        editorType = "Camera Pan";
        type = CutsceneElements.Pan;
    }

    public override void GenerateFromData(string[] data)
    {
        pType = PCLParser.ParseEnum<PanType>(data[0]);
        if (pType == PanType.ByAmount) {
            panDistance = PCLParser.ParseVector2(data[1]);
        } else if (pType == PanType.ToPosition) {
            panEnding = PCLParser.ParseVector3(data[1]);
        } else {
            panToName = PCLParser.ParseLine(data[1]);
        }

        panTime = PCLParser.ParseFloat(data[2]);

    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Pan Type", pType);
        if (pType == PanType.ByAmount) {
            data += PCLParser.CreateAttribute("Distance", panDistance);
        } else if(pType == PanType.ToPosition) {
            data += PCLParser.CreateAttribute("To", panEnding);
        } else {
            data += PCLParser.CreateAttribute("To", panToName);
        }
        data += PCLParser.CreateAttribute("Over", panTime);
        return data;
    }

    protected override void DrawGUI()
    {
        pType = (PanType)EditorGUILayout.EnumPopup("Pan Type", pType);

        if (pType == PanType.ByAmount) {
            panDistance = EditorGUILayout.Vector2Field("By Amount", panDistance);
        } else if (pType == PanType.ToPosition) {
            panEnding = EditorGUILayout.Vector3Field("To", panEnding);
        } else {
            panToName = EditorGUILayout.TextField("To Character", panToName);
        }
        float time = EditorGUILayout.FloatField("Time", panTime);
        if (time > 0) {
            panTime = time;
        }
    }
}

public class PlayEditor : CutsceneElementEditor {
    static string[] soundEffects;
    int selectedFX;
    public PlayEditor() {
        editorType = "Play Sound Effect";
        type = CutsceneElements.Play;
        if (soundEffects == null) {
            List<string> fx = new List<string>();
            foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Sounds")) {
                fx.Add(clip.name);
            }
            soundEffects = fx.ToArray();
        }
    }

    public override void GenerateFromData(string[] data)
    {
        selectedFX = ArrayUtility.IndexOf(soundEffects, PCLParser.ParseLine(data[0]));
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("Sound Effect", soundEffects[selectedFX]);
    }

    protected override void DrawGUI()
    {
        selectedFX = EditorGUILayout.Popup("Sound Effect", selectedFX, soundEffects);
    }
}

public class RotationEditor : CutsceneElementEditor
{
    string mover = "";
    float ang = 0;
    float time = 0;

    public RotationEditor() {
        editorType = "Rotate Object";
        type = CutsceneElements.Rotate;
    }
    protected override void DrawGUI()
    {
        mover = EditorGUILayout.TextField("Character", mover);
        ang = EditorGUILayout.FloatField("Angle", ang);
        float t = EditorGUILayout.FloatField("Time", time);
        if (t > 0) {
            time = t;
        }
    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Character", mover);
        data += PCLParser.CreateAttribute("Angle", ang);
        data += PCLParser.CreateAttribute("Time", time);
        return data;
    }

    public override void GenerateFromData(string[] data)
    {
        mover = PCLParser.ParseLine(data[0]);
        ang = PCLParser.ParseFloat(data[1]);
        time = PCLParser.ParseFloat(data[2]);
    }
}

public class ScaleEditor: CutsceneElementEditor {

    float xScale = 1.0f;
    float yScale = 1.0f;
    float time = 0;
    string name;

    public ScaleEditor() {
        editorType = "Scale Object";
        type = CutsceneElements.Scale;
    }

    public override void GenerateFromData(string[] data)
    {
        name = PCLParser.ParseLine(data[0]);
        xScale = PCLParser.ParseFloat(data[1]);
        yScale = PCLParser.ParseFloat(data[2]);
        time = PCLParser.ParseFloat(data[3]);
    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Name", name);
        data += PCLParser.CreateAttribute("X", xScale);
        data += PCLParser.CreateAttribute("Y", yScale);
        data += PCLParser.CreateAttribute("Time", time);
        return data;
    }

    protected override void DrawGUI()
    {
        name = EditorGUILayout.TextArea("Name", name);
        float scale = EditorGUILayout.FloatField("X Scale", xScale);
        if (scale > 0) {
            xScale = scale;
        }
        scale = EditorGUILayout.FloatField("Y Scale", yScale);
        if (scale > 0)
        {
            yScale = scale;
        }

        scale = EditorGUILayout.FloatField("Time", time);
        if (scale > 0)
        {
            time = scale;
        }

    }
}

public class ShowEditor:CutsceneElementEditor {
    string name;
    Vector2 pos;

    public ShowEditor() {
        editorType = "Show Object";
        type = CutsceneElements.Show;
    }

    public override void GenerateFromData(string[] data)
    {
        name = PCLParser.ParseLine(data[0]);
        pos = PCLParser.ParseVector2(data[1]);
    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Name", name);
        data += PCLParser.CreateAttribute("Location", pos);
        return data;
    }

    protected override void DrawGUI()
    {
        name = EditorGUILayout.TextField("Name", name);
        pos = EditorGUILayout.Vector2Field("Location", pos);
    }
}

public class WaitEditor: CutsceneElementEditor {
    float time;
    public WaitEditor() {
        editorType = "Wait For";
        type = CutsceneElements.Wait;
    }

    public override void GenerateFromData(string[] data)
    {
        time = PCLParser.ParseFloat(data[0]);
    }

    public override string GenerateSaveData()
    {
        return PCLParser.CreateAttribute("Time", time);
    }

    protected override void DrawGUI()
    {
        float t = EditorGUILayout.FloatField("Time", time);
        if (t > 0) {
            time = t;
        }
    }
}