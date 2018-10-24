using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System;

public class Cutscene:Manager
{
    List<CutsceneStep> steps = new List<CutsceneStep>();
	private List<CutsceneActor> characters;

	int currentIndex = 0;
	bool isBeingSkipped = false;

    UnityEvent onStart;
    UnityEvent onEnd;

    private static Cutscene instance;

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
    }

    public void Load(string cutscenePath) {
        TextAsset text = Resources.Load<TextAsset>(cutscenePath);

        if (text) {
            Load(text);
        }
    }


	// Use this for initialization
	public void Load(TextAsset text, bool autoStart = false)
	{
        steps = CutsceneParser.ParseTextFile(text.text);
        if (autoStart)
        {
            StartCutscene();
        }
	}

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
        if (Game.Instance.IsInCutscene)
        {
           
        }
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
    /// Creates a character dynamically from the sprite in Resources with the same name.
    /// </summary>
    /// <param name="charName">The name of the character and the sprite.</param>
    public void CreateCharacter(string charName)
    {
        GameObject character = Resources.Load<GameObject>("Characters/" + charName);
        if (character)
        {
            CutsceneActor actor = GameObject.Instantiate(character).GetComponent<CutsceneActor>();
            actor.name = character.name;
            actor.Init();
            characters.Add(actor);
        }
    }
}

public class CutsceneStep {
    public List<CutsceneElement> elements;
    List<Timer> timers;
    public bool textBeingRevealed;

    public CutsceneStep() {
        elements = new List<CutsceneElement>();
        timers = new List<Timer>();
        textBeingRevealed = false;
    }
    public int NumElements {
        get {
            return elements.Count;
        }
    }

    public int NumTimers {
        get {
            return timers.Count;
        }
    }

    public void AddElement(CutsceneElement element) {
        elements.Add(element);
    }

    public void Run() {
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
}