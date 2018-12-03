using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Cutscene:Manager
{
    List<CutsceneStep> steps = new List<CutsceneStep>();
	private List<CutsceneActor> characters;

	int currentIndex = 0;
	bool isBeingSkipped = false;

    UnityEvent onStart;
    UnityEvent onEnd;

    string cutsceneName = "Cutscene";
    int level = 0;
    List<CharacterInScene> charactersInScene;
    string[] sceneNames;
    Map map;

    GameObject gameObject;

    private static Cutscene instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Cutscene"/> class.
    /// </summary>
    public Cutscene() {
		characters = new List<CutsceneActor>();
        steps = new List<CutsceneStep>();
        onStart = new UnityEvent();
        onEnd = new UnityEvent();
        Game.Instance.AddManager(this);
		if (Map.Instance)
		{
			Game.Instance.Map.OnLevelLoaded.AddListener(EndCutscene);
		}

        if (gameObject == null)
        {
            gameObject = new GameObject("Cutscene");
            gameObject.AddComponent<EditorTimerManager>().Init();
            if (Application.isPlaying)
            {
                GameObject.DontDestroyOnLoad(gameObject);
            }
        }

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
        foreach (TextAsset ta in levels)
        {
            string name = PCLParser.ParseLine(ta.text.Split('\n')[1]);
            names.Add(name);
        }
        sceneNames = names.ToArray();
        LoadMap();

    }

    /// <summary>
    /// Loads a map object if ti exists.  If there isn't one, it waits for a scene change to look again.
    /// </summary>
    void LoadMap() {
        if (map == null)
        {
            GameObject mapGO = GameObject.Find("Map");
            if (mapGO)
            {
                map = mapGO.GetComponent<Map>();
            }
            else
            {
                SceneManager.sceneLoaded += LoadMap;
            }

        }
    }

    /// <summary>
    /// Loads the map if there is one.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="loadSceneMode">Load scene mode.</param>
    void LoadMap(Scene scene, LoadSceneMode loadSceneMode) {
        if (map == null)
        {
            GameObject mapGO = GameObject.Find("Map");
            if (mapGO)
            {
                map = mapGO.GetComponent<Map>();
                SceneManager.sceneLoaded -= LoadMap;
            }
        }
    }

    /// <summary>
    /// Loads a cutscene from the text file at the path.
    /// </summary>
    /// <param name="cutscenePath">Cutscene path.</param>
    public void LoadTextFileAtPath(string cutscenePath) {
        TextAsset text = Resources.Load<TextAsset>(cutscenePath);

        if (text) {
            LoadTextFile(text);
        }
    }


	/// <summary>
    /// Loads a cutscene element from the given text file.
    /// </summary>
    /// <param name="text">The text file from which the elements will be loaded.</param>
    /// <param name="autoStart">If set to <c>true</c> the cutscene start immediately.</param>
	public void LoadTextFile(TextAsset text, bool autoStart = false)
	{
        CutsceneFile file = CutsceneParser.ParseTextFile(text);
        Name = file.cutsceneName;
        Level = file.sceneName;
        Characters = file.characters;
        steps = file.steps;
        if (autoStart)
        {
            StartCutscene();
        }
	}

#if UNITY_EDITOR
    /// <summary>
    /// Gets the steps.
    /// </summary>
    /// <value>The steps.</value>
    public List<CutsceneStep> Steps {
        get {
            return steps;
        }
    }

    /// <summary>
    /// Sets the information about the cutscene such as name of the cutscene, scene it takes place it, characters in it and steps in it.
    /// </summary>
    /// <value>The info.</value>
    public CutsceneFile Info {
        set {
            cutsceneName = value.cutsceneName;
            Level = value.sceneName;
            Characters = value.characters;
            steps = value.steps;
        }
    }

    /// <summary>
    /// Gets the name of the cutscene.
    /// </summary>
    /// <value>The name.</value>
    public string Name {
        get {
            return cutsceneName;
        }

        private set {
            cutsceneName = value;
        }
    }

    /// <summary>
    /// Gets the level in which the scene takes place.
    /// </summary>
    /// <value>The level in which the scene takes place.</value>
    public string Level {

        get {
            return sceneNames[level];
        }

        private set
        {

            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i] == value)
                {
                    level = i;
                    map.Load(sceneNames[level].JoinCamelCase() + ".json");
                    return;
                }
            }
            level = 0;
        }
    }

    /// <summary>
    /// Gets or sets the characters in the cutscene.
    /// </summary>
    /// <value>The characters.</value>
    public string[] Characters
    {
        private set
        {
            foreach (string name in value)
            {
                CharacterInScene character = GetCharacter(name);
                if (character != null)
                {
                    character.isInScene = true;
                    AddActor(character.objectName);
                }
            }
        }

        get
        {
            List<string> names = new List<string>();
            foreach (CharacterInScene character in charactersInScene)
            {
                if (character.isInScene)
                {
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
    CharacterInScene GetCharacter(string name)
    {
        foreach (CharacterInScene character in charactersInScene)
        {
            if (character.objectName == name || character.characterName == name)
            {
                return character;
            }
        }

        return null;
    }

    /// <summary>
    /// Creates and handles the GUI for the Cutscene Info
    /// </summary>
    public void Render()
    {
        cutsceneName = EditorGUILayout.TextField("Cutscene Name", cutsceneName);

        int newLevel = EditorGUILayout.Popup("Level", level, sceneNames);

        if (newLevel != level)
        {
            level = newLevel;

            map.Load(sceneNames[level].JoinCamelCase() + ".json");
        }
        EditorGUILayout.LabelField("Characters In Scene");

        foreach (CharacterInScene character in charactersInScene)
        {
            character.isInScene = EditorGUILayout.Toggle(character.characterName, character.isInScene);

            /*
            if (character.isInScene)
            {
                CutsceneEditor.Instance.AddActor(character.objectName);
            }
            else
            {
                CutsceneEditor.Instance.RemoveActor(character.objectName);
            }*/
        }
    }

    /// <summary>
    /// Gets the game object.
    /// </summary>
    /// <value>The game object.</value>
    public GameObject GameObject
    {
        get
        {
            return gameObject;
        }
    }
#endif

    /// <summary>
    /// Starts the cutscene.
    /// </summary>
    public void StartCutscene()
	{
		if (!Map.Instance.IsLoaded)
		{
			Map.Instance.OnLevelLoaded.AddListener(EndCutscene);
		}
		OnStart.Invoke();
		currentIndex = -1;

		NextElement();

	}

	/// <summary>
	/// Advances to the next element if it exists.
	/// Otherwise, it ends the cutscene and removes everything from the screen.
	/// </summary>
	public void NextElement()
	{
		Controller.Instance.AnyKey.RemoveListener(NextElement);
        currentIndex++;
    
        if (currentIndex < steps.Count) {

            steps[currentIndex].Run();
            if (steps[currentIndex].NumTimers == 0)
            {
				NextElement();
            }
        } else {
            EndCutscene();
        }

	}

    /// <summary>
    /// Ends the cutscene.
    /// </summary>
	void EndCutscene()
	{
        OnEnd.Invoke();
        foreach (CutsceneActor ca in characters)
		{
			ca.DestroySelf();
		}

        characters.Clear();
	}


	

    public void Update(float dt)
    {
    }

    /*
	/// <summary>
	/// Skips the cutscene.
	/// </summary>
	public void SkipCutscene()
	{
		while (currentNode != null)
		{
			if (!currentNode.CanSkip)
			{
				currentNode.Run();
			}
			currentNode = currentNode.nextElement;
		}
		EndCutscene();
	}*/

    /// <summary>
    /// Gets a value indicating whether this instance is being skipped.
    /// </summary>
    /// <value><c>true</c> if this instance is being skipped; otherwise, <c>false</c>.</value>
    public bool IsBeingSkipped
	{
		get
		{
			return isBeingSkipped;
		}
	}

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static Cutscene Instance {
        get {
            if (instance == null) {
                instance = new Cutscene();
            }
            return instance;
        }
    }
    /// <summary>
    /// Gets the on start event.
    /// </summary>
    /// <value>The on start event.</value>
    public UnityEvent OnStart {
        get {
            return onStart;
        }
    }

    /// <summary>
    /// Gets the on end event.
    /// </summary>
    /// <value>The on end event.</value>
    public UnityEvent OnEnd {
        get {
            return onEnd;
        }
    }

    /// <summary>
    /// Finds the actor with the given name.
    /// </summary>
    /// <returns>The actor.</returns>
    /// <param name="actorName">The Actor's name.</param>

    public CutsceneActor FindActor(string actorName)
    {
        string name = actorName.Trim();
        foreach (CutsceneActor ca in characters)
        {
            if (ca.CharacterName.Trim() == name)
            {
                return ca;
            }
        }
        return null;
    }

    /// <summary>
    /// Finds the game object.
    /// </summary>
    /// <returns>The game object.</returns>
    /// <param name="goName">The name of the game object.</param>
    public GameObject FindGameObject(string goName)
    {
        CutsceneActor actor = FindActor(goName);
        if (goName == "Player")
        {
            return Game.Instance.Player.gameObject;
        }
        else if (actor)
        {
            return actor.gameObject;
        }
        else
        {
            return Map.Instance.GetChildByName(goName);
        }
    }

    /// <summary>
    /// Converts the cutscene to a JSON representation
    /// </summary>
    /// <value>To json representation of the cutscene.</value>
    public string ToJSON {
        get
        {
            string data = PCLParser.StructStart;
            data += PCLParser.CreateAttribute("Cutscene Name", cutsceneName);
            data += PCLParser.CreateAttribute("Cutscene Level", sceneNames[level]);
            string characterList = "";
            foreach (CharacterInScene character in charactersInScene)
            {
                if (character.isInScene)
                {
                    characterList += character.characterName + " ";
                }
            }

            data += PCLParser.CreateAttribute("Characters In Scene", characters);
            int i = 1;
            data += PCLParser.CreateArray("Steps");
            foreach (CutsceneStep step in steps)
            {
                data += PCLParser.StructStart;
                data += PCLParser.CreateAttribute("Step Number", i);
                data += step.ToJSON;
                data += PCLParser.StructEnd;
                i++;
            }
            data += PCLParser.ArrayEnding;
            data += PCLParser.StructEnd;
            return data;
        }
    }

    /// <summary>
    /// Converst the cutscene to a plain text representation
    /// </summary>
    /// <value>A plain text representation of the cutscene.</value>
    public string ToText {
        get {
            string info = "";
            info += string.Format("scene {0}\n", sceneNames[level]);
            info += "character ";
            foreach (string name in Characters)
            {
                info += name + " ";
            }
            info += "\n";

            foreach(CutsceneStep step in steps) {
                info += step.ToText;
            }
            return info;
        }
    }

    /// <summary>
    /// Determines whether or not the character is in the cutscene
    /// </summary>
    /// <returns><c>true</c>, if character is in the cutscene, <c>false</c> otherwise.</returns>
    /// <param name="name">The name of the character.</param>
    public bool HasCharacter(string name)
    {
        foreach (CutsceneActor actor in characters)
        {
            if (actor.name == name)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Adds the actor.
    /// </summary>
    /// <param name="name">The name of the actor.</param>
    public void AddActor(string name)
    {
        if (!HasCharacter(name))
        {
            GameObject baseActor = Resources.Load<GameObject>("Characters/" + name);

            CutsceneActor actor = GameObject.Instantiate(baseActor).GetComponent<CutsceneActor>();
            actor.transform.SetParent(gameObject.transform);
            actor.name = actor.name.Remove(actor.name.IndexOf('('));
            //actor.transform.parent = cutsceneGO.transform;
            characters.Add(actor);
        }
    }

    /// <summary>
    /// Gets the actor.
    /// </summary>
    /// <returns>The actor.</returns>
    /// <param name="actorName">The actor's name.</param>
    CutsceneActor GetActor(string actorName)
    {
        foreach (CutsceneActor actor in characters)
        {
            if (actor.CharacterName == actorName)
            {
                return actor;
            }
        }

        return null;
    }
    /// <summary>
    /// Removes the actor from the scene.
    /// </summary>
    /// <param name="name">The name of the actor.</param>
    public void RemoveActor(string name)
    {
        if (HasCharacter(name))
        {
            CutsceneActor actor = GetActor(name);
            characters.Remove(actor);
            GameObject.DestroyImmediate(actor);
        }
    }

    /// <summary>
    /// Previews the step.
    /// </summary>
    /// <param name="stepNumber">Step number.</param>
    public void PreviewStep(int stepNumber)
    {
        //onClearPreview.Invoke();
        UIManager.Instance.Clear();
        for (int i = 0; i < stepNumber - 1; i++)
        {
            steps[i].Skip();
        }

        steps[stepNumber - 1].Run();
    }
}

public class CutsceneStep
{
    public List<CutsceneElement> elements;
    List<Timer> timers;
    public bool textBeingRevealed;

    bool show = true;
    int stepNumber = -1;
    CutsceneDialog dialog;
#region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="T:CutsceneStep"/> class.
    /// </summary>
    /// <param name="number">The place of the step in the current cutscene.</param>
    public CutsceneStep(int number = 0)
    {
        elements = new List<CutsceneElement>();
        timers = new List<Timer>();
        textBeingRevealed = false;
        stepNumber = number;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CutsceneStep"/> class.
    /// </summary>
    /// <param name="lines">Lines of text to be converted to cutscene elements.</param>
    /// <param name="num">The place of the step in the current cutscen.</param>
    public CutsceneStep(List<string> lines, int num) : this(num)
    {
        elements = new List<CutsceneElement>();
        foreach (string line in lines)
        {
            elements.Add(CutsceneParser.ParseElement(line));
        }
    }

#endregion
    /// <summary>
    /// Gets the number of elements.
    /// </summary>
    /// <value>The number elements.</value>
    public int NumElements
    {
        get
        {
            return elements.Count;
        }
    }

    /// <summary>
    /// Gets the number timers currently running.
    /// </summary>
    /// <value>The number timers currently running.</value>
    public int NumTimers
    {
        get
        {
            return timers.Count;
        }
    }

    /// <summary>
    /// Adds the element to the step.
    /// </summary>
    /// <param name="element">Element.</param>
    public void AddElement(CutsceneElement element)
    {
        elements.Add(element);
    }

    /// <summary>
    /// Run all of the elements in the step.
    /// </summary>
    public void Run()
    {
        foreach (CutsceneElement ce in elements)
        {
            if (ce is CutsceneDialog)
            {
                textBeingRevealed = true;
            }
            Timer t = ce.Run();

            if (t != null)
            {
                t.OnComplete.AddListener(() =>
                {
                    timers.Remove(t);

                    if (timers.Count == 0)
                    {
                        if (ce.AutoAdvance && !textBeingRevealed)
                        {
                            Cutscene.Instance.NextElement();
                        }
                        else
                        {
                            Controller.Instance.AnyKey.AddListener(Cutscene.Instance.NextElement);
                        }
                    }
                });

                timers.Add(t);

                t.Start();
            }
        }

    }

#if UNITY_EDITOR
    /// <summary>
    /// Adds a new element to the step.
    /// </summary>
    /// <param name="type">The Cutscene Element type.</param>
    void AddElement(object type)
    {
        CutsceneElements eType = (CutsceneElements)System.Enum.Parse(typeof(CutsceneElements), (string)type);

        CutsceneElement ed = CutsceneParser.ParseElement(eType);
        if (ed != null)
        {
            elements.Add(ed);
            if (eType == CutsceneElements.Dialog)
            {
                if (dialog != null)
                {
                    return;
                }
                dialog = (CutsceneDialog)ed;
            }
            else if (dialog != null)
            {
                elements.Remove(dialog);
                elements.Add(dialog);
            }


        }

    }

    /// <summary>
    /// Draws the GUI for this step.
    /// </summary>
    public void DrawGUI()
    {
        EditorGUI.indentLevel = 0;

        show = EditorGUILayout.Foldout(show, string.Format("Step {0}", stepNumber), true);
        if (show)
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview"))
            {
                Cutscene.Instance.PreviewStep(stepNumber);
            }

            if (GUILayout.Button("Add Element"))
            {
                GenericMenu menu = new GenericMenu();

                string[] types = System.Enum.GetNames(typeof(CutsceneElements));

                foreach (string type in types)
                {
                    menu.AddItem(new GUIContent(type), false, AddElement, type);
                }

                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            foreach (CutsceneElement cee in elements)
            {
                cee.RenderUI();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.Separator();

    }
#endif

    /// <summary>
    /// Generates a JSON representation of the cutscene step.
    /// </summary>
    /// <value>A JSON representation of the cutscene step..</value>
    public string ToJSON
    {
        get
        {
            string data = "";
            data += PCLParser.CreateArray("Elements");
            foreach (CutsceneElement ed in elements)
            {
                data += ed.ToJSON;
            }
            data += PCLParser.ArrayEnding;
            return data;
        }
    }

    /// <summary>
    /// Generates a plain text representation of the cutscene step.
    /// </summary>
    /// <value>A plain text representation of the cutscene step.</value>
    public string ToText
    {
        get
        {
            string data = "";
            for (int i = 0; i < elements.Count; i++)
            {
                data += elements[i].ToText;
                if (i < elements.Count - 1)
                {
                    data += " and";
                }
                data += "\n";
            }
            return data;
        }
    }

    /// <summary>
    /// Skip this instance.
    /// </summary>
    public void Skip()
    {
        foreach (CutsceneElement cee in elements)
        {
            cee.Skip();
        }
    }
}

public class CharacterInScene
{
    public string objectName;
    public string characterName;
    public bool isInScene = false;
}