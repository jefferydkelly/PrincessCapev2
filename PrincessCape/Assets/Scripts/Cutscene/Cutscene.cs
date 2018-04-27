using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System;

public class Cutscene:Manager
{
	List<List<CutsceneElement>> elements = new List<List<CutsceneElement>>();
	private List<CutsceneActor> characters;

    int currentIndex = 0;
    int currentCompleted = 0;
	bool isBeingSkipped = false;

    UnityEvent onStart;
    UnityEvent onEnd;
   
    private static Cutscene instance;

    public Cutscene() {
		characters = new List<CutsceneActor>();
        elements = new List<List<CutsceneElement>>();
        onStart = new UnityEvent();
        onEnd = new UnityEvent();
        Game.Instance.AddManager(this);
        Game.Instance.Map.OnLevelLoaded.AddListener(EndCutscene);
        EventManager.StartListening("ElementCompleted", ElementCompleted);
    }

    public void Load(string cutscenePath) {
        TextAsset text = Resources.Load<TextAsset>(cutscenePath);
        if (text) {
            Load(text);
        }
    }

    CutsceneElement Parse(string line) {
        string[] parts = line.Split(' ');
		string p = parts[0].ToLower();
      
		if (p == "show")
		{
			return new CutsceneEffect(parts[1], EffectType.Show, float.Parse(parts[2]), float.Parse(parts[3]));
		}
		else if (p == "hide")
		{
			return new CutsceneEffect(parts[1], EffectType.Hide);
		}
		else if (p == "fade-in")
		{
			return new CutsceneFade(parts[1], 1.0f, float.Parse(parts[2]));
		}
		else if (p == "fade-out")
		{
			return new CutsceneFade(parts[1], 0.0f, float.Parse(parts[2]));
		}
		else if (p == "fade")
		{
			return new CutsceneFade(parts[1], float.Parse(parts[2]), float.Parse(parts[3]));
		}
		else if (p == "alpha")
		{
			return new CutsceneFade(parts[1], float.Parse(parts[2]), 0);
		}
		else if (p == "flip-x")
		{
			return new CutsceneEffect(parts[1], EffectType.FlipHorizontal);
		}
		else if (p == "flip-y")
		{
			return new CutsceneEffect(parts[1], EffectType.FlipVertical);
		}
		else if (p == "scale")
		{
			return new CutsceneScale(ScaleType.All, parts[1], float.Parse(parts[2]), parts.Length == 4 ? float.Parse(parts[3]) : 0);
		}
		else if (p == "scalex")
		{
			return new CutsceneScale(ScaleType.X, parts[1], float.Parse(parts[2]), float.Parse(parts[3]));
		}
		else if (p == "scaley")
		{
			return new CutsceneScale(ScaleType.Y, parts[1], float.Parse(parts[2]), float.Parse(parts[3]));
		}
		else if (p == "scalexy")
		{
			return new CutsceneScale(parts[1], float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
		}
		else if (p == "rotate")
		{
			return new CutsceneMovement(parts[1], MoveTypes.Rotate, float.Parse(parts[2]), parts.Length == 4 ? float.Parse(parts[3]) : 0);
		}
		else if (p == "move")
		{
			return new CutsceneMovement(parts[1], MoveTypes.XY, float.Parse(parts[2]), float.Parse(parts[3]), parts.Length == 5 ? float.Parse(parts[4]) : 0);
		}
		else if (p == "move-x")
		{
			return new CutsceneMovement(parts[1], MoveTypes.X, float.Parse(parts[2]), parts.Length == 4 ? float.Parse(parts[3]) : 0);
		}
		else if (p == "move-y")
		{
            return new CutsceneMovement(parts[1], MoveTypes.Y, float.Parse(parts[2]), parts.Length == 4 ? float.Parse(parts[3]) : 0);
		}
		else if (p == "pan")
		{
			if (parts[1] == "to")
			{
				if (parts.Length == 4)
				{
					GameObject go = GameObject.Find(parts[2]);
                    float panTime = float.Parse(parts[3]);

					if (go)
					{
                        return new CameraPan(go.transform.position, panTime);
					}
					else
					{
                        return new CameraPan(parts[2], panTime);
					}
				}
				else
				{
                    float panTime = float.Parse(parts[4]);
					
                    return new CameraPan(new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), Camera.main.transform.position.z), panTime);
				}
			}
			else
			{
				return new CameraPan(new Vector2(float.Parse(parts[1]), float.Parse(parts[2])));
			}
		}
		else if (p == "wait")
		{
            return new CutsceneWait(float.Parse(parts[1]));
		}
		else if (p == "create")
		{
			return new CutsceneCreation(parts[1], parts[2], parts[3], parts[4]);
		}
		else if (p == "destroy")
		{
            return new CutsceneCreation(parts[1].Trim());
		}
		else if (p == "add")
		{
			return new CutsceneAdd(parts[1].Trim());
		}
		else if (p == "disable")
		{
			GameObject go;
			CutsceneActor ca = FindActor(parts[1].Trim());
			if (ca == null)
			{
				go = GameObject.Find(parts[1].Trim());
			}
			else
			{
				go = ca.gameObject;
			}
			return new CutsceneEnable(go, false);
		}
		else if (p == "enable")
		{
			GameObject go;
			CutsceneActor ca = FindActor(parts[1].Trim());
			if (ca == null)
			{
				go = GameObject.Find(parts[1].Trim());
			}
			else
			{
				go = ca.gameObject;
			}
			if (parts.Length == 4)
			{
				return new CutsceneEnable(go, float.Parse(parts[2]), float.Parse(parts[3]));
			}
			else
			{
				return new CutsceneEnable(go, true);
			}
		}
		else if (p == "activate")
		{
			GameObject go = GameObject.Find(parts[1].Trim());
			if (go != null)
			{
				ActivatedObject ao = go.GetComponent<ActivatedObject>();
				if (ao != null)
				{
					return new CutsceneActivate(ao, parts[2].Trim() == "true");
				}
			}
		}
		else if (p == "align")
		{
			return new CutsceneAlign(parts[1].Trim() == "left");
		}
		else if (p == "play")
		{
			//c = new CutscenePlay(parts[1].Trim());
		}
		else if (p == "follow")
		{
			return new CameraFollow(parts[1].Trim());
		}
		else if (p == "goto")
		{
			return new SceneChange(parts[1].Trim());
        } else if (p == "animate") {
            return new CutsceneAnimation(FindActor(parts[1].Trim()), parts[2].Trim());
        }
		else if (p == "character")
		{
			if (parts.Length == 2)
			{
				CreateCharacter(parts[1].Trim());
			}
			else
			{
				CreateCharacter(parts[1].Trim(), parts[2]);
			}
        } else {
            parts = line.Split(':');
       
			if (parts.Length == 2)
			{
				return new CutsceneDialog(parts[0], parts[1].Trim());
			}
			else
			{
				return new CutsceneDialog(parts[0].Trim());
			}
        }

        return null;

	}
	// Use this for initialization
	public void Load(TextAsset text)
	{
		elements = new List<List<CutsceneElement>>();
        string[] lines = text.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            

            List<CutsceneElement> elems = new List<CutsceneElement>();
            bool seq = false;
            do
            {
                string line = lines[i].Trim();

                seq = line.Substring(line.Length - 3) == "and";

                if (seq)
                {
                    line = line.Substring(0, line.Length - 4);
                }
               
                CutsceneElement e = Parse(line);

                if (e != null)
                {
                    elems.Add(e);
                    if (seq) {
                        i++;
                    }
                } 
               
            } while (seq);

            if (elems.Count > 0)
            {
                elements.Add(elems);
            }
        }
	}

	public void StartCutscene()
	{
        OnStart.Invoke();
        currentIndex = -1;

        NextElement();
       

	}

    void ElementCompleted() {
        currentCompleted++;
    }
	/// <summary>
	/// Advances to the next element if it exists.
	/// Otherwise, it ends the cutscene and removes everything from the screen.
	/// </summary>
	public void NextElement()
	{
        currentIndex++;
        currentCompleted = 0;
    
        if (currentIndex < elements.Count) {
            foreach(CutsceneElement ce in elements[currentIndex]) {
                ce.Run();
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
	/// <summary>
	/// Creates a character dynamically from the sprite in Resources with the same name.
	/// </summary>
	/// <param name="charName">The name of the character and the sprite.</param>
	public void CreateCharacter(string charName)
	{
        GameObject character = Resources.Load<GameObject>("Characters/" + charName);
        Debug.Log(string.Format("{0}: {1}", charName, character != null));
        if (character)
		{
            Debug.Log("I found " + charName);
			CutsceneActor actor = GameObject.Instantiate(character).GetComponent<CutsceneActor>();
			actor.Init();
            actor.CharacterName = charName;
            Debug.Log(actor.name + " " + charName);
			characters.Add(actor);
		}
	}

	/// <summary>
	/// Creates the character.
	/// </summary>
	/// <param name="charName">Char name.</param>
	/// <param name="spriteName">Sprite name.</param>
	public void CreateCharacter(string charName, string spriteName)
	{
        GameObject character = Resources.Load<GameObject>("Characters/" + charName);
		if (character)
		{
            CutsceneActor actor = GameObject.Instantiate(character).GetComponent<CutsceneActor>();
            actor.Init();
            actor.CharacterName = spriteName;
            characters.Add(actor);
		}
	}

	/// <summary>
	/// Finds the actor with the given name.
	/// </summary>
	/// <returns>The actor.</returns>
	/// <param name="actorName">The Actor's name.</param>

	public CutsceneActor FindActor(string actorName)
	{
		foreach (CutsceneActor ca in characters)
		{
			if (ca.CharacterName.Trim() == actorName.Trim())
			{
				return ca;
			}
		}
		return null;
	}

    public void Update(float dt)
    {
        if (Game.Instance.IsInCutscene)
        {
            if (currentCompleted >= elements[currentIndex].Count)
            {
                NextElement();
            }
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
}

