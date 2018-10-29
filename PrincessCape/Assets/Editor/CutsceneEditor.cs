using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
public class CutsceneEditor : EditorWindow {

    private static CutsceneEditor instance;
    List<CutsceneStepEditor> steps;
    CutsceneInfo info;
    GameObject cutsceneGO;
    List<CutsceneActor> actors;
    Vector2 scrollPos;

    [MenuItem("My Game/Cutscene Editor")]
    public static void ShowWindow() {
        GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
       
    }

    private void OnEnable()
    {
        if (EditorSceneManager.GetActiveScene().name != "Test")
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Test.unity");
        }
        instance = this;//GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
        steps = new List<CutsceneStepEditor>();
        steps.Add(new CutsceneStepEditor());
        info = new CutsceneInfo();
        GameObject cutscene = GameObject.Find("Cutscene");
        if (cutscene == null) {
            cutscene = new GameObject("Cutscene");
        }
        cutsceneGO = cutscene;
        actors = new List<CutsceneActor>();
        scrollPos = Vector2.zero;
       



    }
    private void OnDestroy()
    {
        if (instance)
        {
            DestroyImmediate(instance.cutsceneGO);
        }
    }

    private void OnGUI()
    {
        if (instance != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save as JSON")) {
                //Save the cutscene
                string path = EditorUtility.SaveFilePanel("Save Cutscene To File", "Assets/Resources/Cutscenes", instance.info.CutsceneName.JoinCamelCase(), "json");

                if (path.Length > 0)
                {
                    string data = PCLParser.StructStart;
                    data += instance.info.SaveData;
                    int i = 1;
                    data += PCLParser.CreateArray("Steps");
                    foreach (CutsceneStepEditor step in instance.steps)
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
            } else if (GUILayout.Button("Save as Text"))
            {
                //Save the cutscene
                string path = EditorUtility.SaveFilePanel("Save Cutscene To File", "Assets/Resources/Cutscenes", instance.info.CutsceneName.JoinCamelCase(), "txt");

                if (path.Length > 0)
                {
                    string data = instance.info.HumanReadable;

                    foreach (CutsceneStepEditor step in instance.steps)
                    {
                        data += step.HumanReadable;
                    }
                  
                    File.WriteAllText(path, data);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load JSON"))
            {
                string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Cutscenes", "json");
                if (path.Length > 0)
                {

                    CutsceneFile file = PCLParser.ParseCutsceneFile(File.ReadAllText(path));
                    instance.info.CutsceneName = file.cutsceneName;
                    instance.info.Characters = file.characters;

                    instance.steps.Clear();
                    foreach (CutsceneStepStruct step in file.steps)
                    {
                        instance.steps.Add(new CutsceneStepEditor(step.elements));
                    }

                }
            } else if (GUILayout.Button("Load Text"))
            {
                string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Cutscenes", "txt");
                if (path.Length > 0) {
                    instance.steps.Clear();
                    string[] lines = File.ReadAllLines(path);

                    string[] pathParts = path.Split('/');
                    string sceneName = pathParts[pathParts.Length - 1].Split('.')[0].SplitCamelCase();

                    instance.info.CutsceneName = sceneName;
                    sceneName = lines[0].Substring(lines[0].IndexOf(' ') + 1).Trim();

                    instance.info.Scene = sceneName;

                    string[] chars = lines[1].Substring(lines[1].IndexOf(' ') + 1).Trim().Split(' ');
                    instance.info.Characters = chars;

                    List<string> stepText = new List<string>();
                    int i = 3;
                    do
                    {
                        stepText.Clear();
                        string[] parts = { };
                        do
                        {
                            parts = lines[i].Trim().Split(' ');
                            stepText.Add(lines[i]);
                            i++;
                        } while (parts[parts.Length - 1] == "and");
                        steps.Add(new CutsceneStepEditor(stepText));
                    } while (i < lines.Length);
                }
            }


            EditorGUILayout.EndHorizontal();
            instance.info.Render();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < instance.steps.Count; i++)
            {
                EditorGUI.indentLevel = 0;
                CutsceneStepEditor step = instance.steps[i];
                step.Show = EditorGUILayout.Foldout(step.Show, string.Format("Step {0}", i + 1), true);
                if (step.Show)
                {
                    EditorGUI.indentLevel = 1;
                    instance.steps[i].DrawGUI();
                }
                EditorGUILayout.Separator();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Step"))
            {
                instance.steps.Add(new CutsceneStepEditor());
            }
        }
    }

    public static CutsceneEditor Instance {
        get {
            return instance;
        }
    }

    public bool HasCharacter(string name) {
        foreach(CutsceneActor actor in actors) {
            if (actor.name == name) {
                return true;
            }
        }

       return false;
    }

    public void AddActor(string name) {
        if (!HasCharacter(name)) {
            GameObject baseActor = Resources.Load<GameObject>("Characters/" + name);
            CutsceneActor actor = Instantiate(baseActor).GetComponent<CutsceneActor>();
            actor.name = actor.name.Remove(actor.name.IndexOf('('));
            actor.transform.parent = cutsceneGO.transform;
            actors.Add(actor);
        }
    }

    CutsceneActor GetActor(string actorName) {
        foreach(CutsceneActor actor in actors) {
            if (actor.CharacterName == actorName) {
                return actor;
            }
        }

        return null;
    }

    public void RemoveActor(string name) {
        if (HasCharacter(name)) {
            CutsceneActor actor = GetActor(name);
            actors.Remove(actor);
            DestroyImmediate(actor);
        }
    }

    public CutsceneInfo Info {
        get {
            return info;
        }
    }

}

/// <summary>
/// Cutscene info.
/// </summary>
public class CutsceneInfo
{
    string cutsceneName = "Cutscene";
    int level = 0;
    List<CharacterInScene> charactersInScene;
    string[] sceneNames;
    Map map;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CutsceneInfo"/> class.
    /// </summary>
    public CutsceneInfo()
    {
        charactersInScene = new List<CharacterInScene>();
        foreach (CutsceneActor actor in Resources.LoadAll<CutsceneActor>("Characters"))
        {
            CharacterInScene character = new CharacterInScene();
            character.characterName = actor.CharacterName;
            character.objectName = actor.name;
            character.isInScene = false;
            charactersInScene.Add(character);
        }

        List<string> names = new List<string>();
        TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");
        foreach(TextAsset ta in levels) {
            string name = PCLParser.ParseLine(ta.text.Split('\n')[1]);
            names.Add(name);
        }
        sceneNames = names.ToArray();
        map = GameObject.Find("Map").GetComponent<Map>();
    }

    /// <summary>
    /// Creates and handles the GUI for the Cutscene Info
    /// </summary>
    public void Render()
    {
        cutsceneName = EditorGUILayout.TextField("Cutscene Name", cutsceneName);

        int newLevel = EditorGUILayout.Popup("Level", level, sceneNames);

        if (newLevel != level) {
            level = newLevel;
         
            map.Load(sceneNames[level].JoinCamelCase() + ".json");
        }
        EditorGUILayout.LabelField("Characters In Scene");
      
        foreach (CharacterInScene character in charactersInScene)
        {
            character.isInScene = EditorGUILayout.Toggle(character.characterName, character.isInScene);

            if (character.isInScene) {
                CutsceneEditor.Instance.AddActor(character.objectName);
            } else {
                CutsceneEditor.Instance.RemoveActor(character.objectName);
            }
        }
    }

    /// <summary>
    /// Gets or sets the name of the cutscene.
    /// </summary>
    /// <value>The name of the cutscene.</value>
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

    /// <summary>
    /// Gets or sets the characters in the cutscene.
    /// </summary>
    /// <value>The characters.</value>
    public string[] Characters {
        set {
            foreach(string name in value) {
                CharacterInScene character = GetCharacter(name);
                if (character != null) {
                    character.isInScene = true;
                    CutsceneEditor.Instance.AddActor(character.objectName);
                }
            }
        }

        get {
            List<string> names = new List<string>();
            foreach(CharacterInScene character in charactersInScene) {
                if (character.isInScene) {
                    names.Add(character.characterName);
                }
            }
            return names.ToArray();
        }
    }

    /// <summary>
    /// Gets the character with the given name.
    /// </summary>
    /// <returns>The character with that name.</returns>
    /// <param name="name">The name of the character.</param>
    CharacterInScene GetCharacter(string name) {
        foreach(CharacterInScene character in charactersInScene) {
            if (character.objectName == name || character.characterName == name) {
                return character;
            }
        }

        return null;
    }
    /// <summary>
    /// Gets the save data.
    /// </summary>
    /// <value>The save data.</value>
    public string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Cutscene Name", cutsceneName);

            string characters = "";
            foreach(CharacterInScene character in charactersInScene) {
                if (character.isInScene) {
                    characters += character.characterName + " ";
                }
            }
          
            data += PCLParser.CreateAttribute("Characters In Scene", characters);
            return data;
        }
    }

    public string HumanReadable {
        get {
            string info = string.Format("name {0}\n", cutsceneName);
            info += string.Format("scene {0}\n", sceneNames[level]);
            info += "character ";
            foreach(string name in Characters) {
                info += name + " ";
            }
            info += "\n";
            return info;
        }
    }

    public string Scene {
        set {
         
            for (int i = 0; i < sceneNames.Length; i++) {
                if (sceneNames[i] == value) {
                    level = i;
                    map.Load(sceneNames[level].JoinCamelCase() + ".json");
                    return;
                }
            }
            level = 0;
        }
    }

}

/// <summary>
/// A representation of one step of the cutscene which can have multiple elements
/// </summary>
public class CutsceneStepEditor {
    bool show = true;
    List<CutsceneElementEditor> elements = new List<CutsceneElementEditor>();
    DialogEditor dialog;

    public CutsceneStepEditor() {
        
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CutsceneStep"/> class.
    /// </summary>
    /// <param name="els">The elements already in the step.</param>
    public CutsceneStepEditor(List<CutsceneElementStruct> els) {
        foreach(CutsceneElementStruct ces in els) {
            CutsceneElementEditor cee = CutsceneParser.ParseElement(ces.type);
            cee.GenerateFromData(ces.info.ToArray());
            elements.Add(cee);
        }
    }

    public CutsceneStepEditor(List<string> lines) {
        elements = new List<CutsceneElementEditor>();
        foreach(string line in lines) {
            elements.Add(CutsceneParser.ParseElement(line));
        }
    }

    /// <summary>
    /// Draws the GUI for this step.
    /// </summary>
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

    /// <summary>
    /// Adds a new element to the step.
    /// </summary>
    /// <param name="type">The Cutscene Element type.</param>
    void AddElement(object type)
    {
        CutsceneElements eType = (CutsceneElements)System.Enum.Parse(typeof(CutsceneElements), (string)type);
       
        CutsceneElementEditor ed = CutsceneParser.ParseElement(eType);
        if (ed != null) {
            elements.Add(ed);
            if (eType == CutsceneElements.Dialog)
            {
                if (dialog != null)
                {
                    return;
                }
                dialog = (DialogEditor)ed;
            } else if (dialog != null) {
                elements.Remove(dialog);
                elements.Add(dialog);
            }
          

        }
       
    }


    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:CutsceneStep"/> is shown.
    /// </summary>
    /// <value><c>true</c> if shown; otherwise, <c>false</c>.</value>
    public bool Show {
        get {
            return show;
        }

        set {
            show = value;
        }
    }

    /// <summary>
    /// Gets the save data for the step and all of its elements.
    /// </summary>
    /// <value>The save data.</value>
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

    public string HumanReadable {
        get {
            string data = "";
            for (int i = 0; i < elements.Count; i++) {
                data += elements[i].HumanReadable;
                if (i < elements.Count - 1) {
                    data += " and";
                }
                data += "\n";
            }
            return data;
        }
    }
}

public class CharacterInScene {
    public string objectName;
    public string characterName;
    public bool isInScene = false;
}