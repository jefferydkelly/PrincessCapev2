using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// Cutscene element.
/// </summary>
public class CutsceneElement
{
	public CutsceneElement nextElement = null;
	public CutsceneElement prevElement = null;
	protected bool canSkip = false;

	protected bool autoAdvance = false;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:CutsceneElement"/> advances automatically.
    /// </summary>
    /// <value><c>true</c> if auto advance; otherwise, <c>false</c>.</value>
	public bool AutoAdvance
	{
		get
		{
			return autoAdvance;
		}
	}

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:CutsceneElement"/> can be skipped.
    /// </summary>
    /// <value><c>true</c> if can be skipped; otherwise, <c>false</c>.</value>
	public bool CanSkip
	{
		get
		{
			return canSkip;
		}
	}

    /// <summary>
    /// Run this instance.
    /// </summary>
	public virtual void Run()
	{
	}

    /// <summary>
    /// Skip this instance.
    /// </summary>
    public virtual void Skip() {
        
    }
}

/// <summary>
/// A container for cutscene dialog
/// </summary>
public class CutsceneDialog : CutsceneElement
{
	string speaker = "Character";
	string dialog = "Hi, I'm a character";

	/// <summary>
	/// Initializes a new instance of the <see cref="CutsceneDialog"/> class with a speaker and a line.
	/// </summary>
	/// <param name="spk">Spk.</param>
	/// <param name="dia">Dia.</param>
	public CutsceneDialog(string spk, string dia)
	{
		speaker = spk;
		dialog = dia.Replace("\\n", "\n").Trim();
		canSkip = true;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CutsceneDialog"/> class for duration.
	/// </summary>
	/// <param name="dia">Dia.</param>
	public CutsceneDialog(string dia)
	{
		speaker = null;
		dialog = dia.Replace("\\n", "\n").Trim();
		//dialog = dialog.Replace("[[PauseKey]]", GameManager.Instance.Player.Controller.PauseKey);
		//dialog = dialog.Replace("[[LeftItemKey]]", GameManager.Instance.Player.Controller.LeftItemKey);
		//dialog = dialog.Replace("[[RightItemKey]]", GameManager.Instance.Player.Controller.RightItemKey);
		//dialog = dialog.Replace("[[AimKeys]]", GameManager.Instance.Player.Controller.AimKeys);
	}

    /// <summary>
    /// Displays the dialog of message
    /// </summary>
	public override void Run()
	{
        MessageBox.SetLine(dialog);
		if (speaker != null)
		{
            SpeakerBox.SetSpeaker(speaker);
            EventManager.TriggerEvent("ShowDialog");
		}
		else
		{
            EventManager.TriggerEvent("ShowMessage");
		}

        EventManager.StartListening("EndOfMessage", Proceed);
	}

    void Proceed() {
        EventManager.StopListening("EndOfMessage", Proceed);
        Cutscene.Instance.NextElement();
    }
}

/// <summary>
/// Camera pan.
/// </summary>
public class CameraPan : CutsceneElement
{
	Vector2 panDistance = Vector2.zero;
	Vector3 panEnding;
	bool panTo;
	string panToName = "";

	/// <summary>
	/// Initializes a new <see cref="CameraPan"/>.
	/// </summary>
	/// <param name="pd">The distance which the Camera will be panned.</param>
	/// <param name="t">The duration of the pan.</param>
	public CameraPan(Vector2 pd)
	{
		panDistance = pd;
		panTo = false;
		canSkip = true;
	}

	/// <summary>
	/// Initializes a new <see cref="CameraPan"/> to the given location.
	/// </summary>
	/// <param name="pd">The ending of the pan</param>
	/// <param name="t">The duration of the pan</param>
	public CameraPan(Vector3 pd)
	{
		panEnding = pd;
		panTo = true;
		canSkip = true;
	}

    public CameraPan(string name)
	{
		panToName = name;

		panTo = true;
		canSkip = true;
	}

	public override void Run()
	{
        if (panTo)
        {
            if (panToName.Length > 0)
            {
                CameraManager.Instance.Pan(Cutscene.Instance.FindActor(panToName).gameObject);
            }
            else
            {
                CameraManager.Instance.PanTo(panEnding);
            }
        }
        else
        {
            CameraManager.Instance.Pan(panDistance);
        }
	}
}

public class CameraFollow : CutsceneElement
{
	public string targetName;

	public CameraFollow(string name)
	{
		targetName = name;
		canSkip = true;
		autoAdvance = true;
	}

	public override void Run()
	{
		CameraManager.Instance.Target = GameObject.Find(targetName);
        CameraManager.Instance.IsFollowing = true;
	}
}

/// <summary>
/// Cutscene wait.
/// </summary>
public class CutsceneWait : CutsceneElement
{
    Timer waitTimer;
	/// <summary>
	/// Initializes a new instance of the <see cref="CutsceneWait"/> class.
	/// </summary>
	/// <param name="dt">The duration of the wait.</param>
	public CutsceneWait(float dt)
	{
		canSkip = true;
        waitTimer = new Timer(dt);
        waitTimer.OnComplete.AddListener(Cutscene.Instance.NextElement);
	}
	public override void Run()
	{
        waitTimer.Start();
	}
}

public enum MoveTypes
{
	X, Y, XY, Rotate
}

public class CutsceneMovement : CutsceneElement
{
	string mover = "Character";
	MoveTypes moveType = MoveTypes.XY;
	float x = 0;
	float y = 0;
	float ang = 0;
	float time = 0;

	public CutsceneMovement(string target, MoveTypes mt, float dx, float dy, float dt)
	{
		mover = target;
		moveType = mt;
		x = dx;
		y = dy;
		time = dt;
		canSkip = true;
	}

	public CutsceneMovement(string target, MoveTypes mt, float angle, float dt)
	{
		mover = target;
		moveType = mt;
		if (mt == MoveTypes.Rotate)
		{
			ang = angle;
		}
		else if (mt == MoveTypes.X)
		{
			x = angle;
		}
		else if (mt == MoveTypes.Y)
		{
			y = angle;
		}

		time = dt;
	}

	public override void Run()
	{
        CutsceneActor myActor = Cutscene.Instance.FindActor(mover);

		if (myActor != null)
		{
			if (moveType == MoveTypes.Rotate)
			{
                myActor.Rotate(ang, time);
			}
			else
			{
				if (moveType == MoveTypes.X)
				{
					y = myActor.transform.position.y;
				}
				else if (moveType == MoveTypes.Y)
				{
					x = myActor.transform.position.x;
				}
				myActor.MoveTo(new Vector2(x, y), time);
			}
		}
	}
}

public class CutsceneEffect : CutsceneElement
{
	EffectType type = EffectType.Show;
	string affected = "Character";
	float x = 0.0f;
	float y = 0.0f;

	public CutsceneEffect(string target, EffectType et)
	{
		affected = target;
		type = et;
		autoAdvance = true;
		canSkip = true;
	}

	public CutsceneEffect(string target, EffectType et, float dx, float dy)
	{
		affected = target;
		type = et;
		x = dx;
		y = dy;
		autoAdvance = true;
		canSkip = true;
	}

	public override void Run()
	{
        CutsceneActor myActor = Cutscene.Instance.FindActor(affected);

		if (type == EffectType.Show)
		{
            if (myActor)
			{
                
				Vector3 aPosition = new Vector3(x, y);

				myActor.Position = aPosition;
				myActor.IsHidden = false;

			}
		}
		else if (type == EffectType.Hide)
		{
			if (myActor && !myActor.IsHidden)
			{
				myActor.IsHidden = true;
			}
			//auto advance
		}
		else if (type == EffectType.FlipHorizontal)
		{
			myActor.FlipX();
			
		}
		else if (type == EffectType.FlipVertical)
		{
			myActor.FlipY();
		}
	}
}

public class CutsceneScale : CutsceneElement
{
	ScaleType type;
	float scale = 1.0f;
	float scale2 = 1.0f;
	float time = 0;
	string actorName;
	public CutsceneScale(ScaleType st, string actor, float sc, float dt)
	{
		actorName = actor;
		type = st;
		scale = sc;
		time = dt;
		canSkip = true;
	}

	public CutsceneScale(string actor, float sc1, float sc2, float dt)
	{
		actorName = actor;
		type = ScaleType.Ind;
		scale = sc1;
		scale2 = sc2;
		time = dt;
		canSkip = true;
	}

	public override void Run()
	{
        CutsceneActor actor = Cutscene.Instance.FindActor(actorName);
		if (type == ScaleType.All)
		{
			actor.Scale(scale, time);
		}
		else if (type == ScaleType.X)
		{
			actor.ScaleX(scale, time);
		}
		else if (type == ScaleType.Y)
		{
			actor.ScaleY(scale, time);
		}
		else if (type == ScaleType.Ind)
		{
			actor.ScaleXY(new Vector3(scale, scale2, 1), time);
		}
	}
}

public class CutsceneFade : CutsceneElement
{
	string actorName;
	float alpha;
	float time;
	public CutsceneFade(string actor, float toAlpha, float dt)
	{
		actorName = actor;
		alpha = toAlpha;
		time = dt;
		canSkip = true;
	}

	public override void Run()
	{
        CutsceneActor actor = Cutscene.Instance.FindActor(actorName);
		if (actor)
		{
			actor.Fade(alpha, time);
		}
	}
}

public class CutsceneCreation : CutsceneElement
{
	GameObject prefab;
	Vector3 position;
	string objectName;
	bool destroy = false;
	public CutsceneCreation(string name, string dx, string dy, string dz)
	{
		prefab = Resources.Load<GameObject>(name);
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

	public override void Run()
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
	}
}

public class CutsceneAdd : CutsceneElement
{
    string itemName;

	public CutsceneAdd(string item)
	{
        itemName = item;
		autoAdvance = true;
		canSkip = false;
	}

	public override void Run()
	{
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemName) as MagicItem, false);
	}
}

public class CutsceneEnable : CutsceneElement
{
	GameObject hideObject;
	bool enable = true;
	bool move = false;
	Vector2 pos;
	public CutsceneEnable(GameObject go, bool en)
	{
		hideObject = go;
		enable = en;
		autoAdvance = true;
		canSkip = false;
	}

	public CutsceneEnable(GameObject go, float x, float y) : this(go, true)
	{
		move = true;
		pos = new Vector2(x, y);
	}

	public override void Run()
	{
		if (hideObject)
		{
			hideObject.SetActive(enable);

			if (move)
			{
				hideObject.transform.position = pos;
			}

		}
	}
}

public class CutsceneActivate : CutsceneElement
{
	bool activate;
	ActivatedObject ao;

	public CutsceneActivate(ActivatedObject aObj, bool activated)
	{
		ao = aObj;
		activate = activated;
        autoAdvance = true;
		canSkip = false;
	}

	public override void Run()
	{
		if (activate)
		{
			ao.Activate();
		}
		else
		{
			ao.Deactivate();
		}
	}
}

public class CutsceneAlign : CutsceneElement
{
	bool left;

	public CutsceneAlign(bool l)
	{
		left = l;
		autoAdvance = true;
		canSkip = true;
	}

	public override void Run()
	{
        if (left) {
            EventManager.TriggerEvent("AlignLeft");
        } else {
            EventManager.TriggerEvent("AlignRight");
        }
	}
}

/*
public class CutscenePlay : CutsceneElement
{
	AudioClip soundEffect;
	public CutscenePlay(string s)
	{
		soundEffect = Resources.Load<AudioClip>("Sounds/" + s);
		autoAdvance = true;
		canSkip = true;
	}

	public override void Run()
	{
		AudioManager.Instance.PlaySound(soundEffect);
	}
}*/
public enum EffectType
{
	Show, Hide, FlipHorizontal, FlipVertical
}

public enum ScaleType
{
	All, X, Y, Ind
}

public enum CutsceneElements
{
	Dialog, Move, Effect, SpriteChange
}

[System.Serializable]
public class CutsceneSpriteChange : CutsceneElement
{
	string affected = "Character";
	int newSprite = -1;

	public CutsceneSpriteChange(string target, int spriteIndex)
	{
		affected = target;
        newSprite = spriteIndex;
		autoAdvance = true;
	}

	public override void Run()
	{
        CutsceneActor ca = Cutscene.Instance.FindActor(affected);

		if (ca)
		{
            //ca.SpriteIndex = newSprite;
		}
	}
}

public class CutsceneFontEffect : CutsceneElement
{
	bool enact;
	FontEffects effect;

	public CutsceneFontEffect(FontEffects fe, bool act)
	{
		effect = fe;
		enact = act;
		autoAdvance = true;
	}

	public override void Run()
	{
		if (effect == FontEffects.Bold)
		{
			//UIManager.Instance.Bolded = enact;
		}
		else if (effect == FontEffects.Italics)
		{
			//UIManager.Instance.Italicized = enact;
		}
	}
}

public enum FontEffects
{
	Bold,
	Italics
}

public class SceneChange : CutsceneElement
{
	string newScene;

	public SceneChange(string scene)
	{
		newScene = scene;
		canSkip = false;
		autoAdvance = true;
	}

	public override void Run()
	{
        Game.Instance.LoadScene(newScene);
		//CameraManager.Instance.FadeOutToNewScene(newScene);
	}
}