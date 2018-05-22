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
	protected Timer runTimer;

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
	public virtual Timer Run()
	{
		return null;
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
	}

    /// <summary>
    /// Displays the dialog of message
    /// </summary>
	public override Timer Run()
	{
		return UIManager.Instance.ShowMessage(dialog, speaker);
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
    float panTime = 1.0f;

	/// <summary>
	/// Initializes a new <see cref="CameraPan"/>.
	/// </summary>
	/// <param name="pd">The distance which the Camera will be panned.</param>
	/// <param name="t">The duration of the pan.</param>
	public CameraPan(Vector2 pd, float time = 1.0f)
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
	public CameraPan(Vector3 pd, float time = 1.0f)
	{
		panEnding = pd;
		panTo = true;
		canSkip = true;
	}

    public CameraPan(string name, float time = 1.0f)
	{
		panToName = name;

		panTo = true;
		canSkip = true;
        panTime = time;
	}

	public override Timer Run()
	{
        if (panTo)
        {
            if (panToName.Length > 0)
            {
                CameraManager.Instance.Pan(Cutscene.Instance.FindActor(panToName).gameObject, panTime);
            }
            else
            {
                CameraManager.Instance.PanTo(panEnding, panTime);
            }
        }
        else
        {
            CameraManager.Instance.Pan(panDistance, panTime);
        }

		return null;
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

	public override Timer Run()
	{
		CameraManager.Instance.Target = GameObject.Find(targetName);
        CameraManager.Instance.IsFollowing = true;
		return null;
	}
}

/// <summary>
/// Cutscene wait.
/// </summary>
public class CutsceneWait : CutsceneElement
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CutsceneWait"/> class.
	/// </summary>
	/// <param name="dt">The duration of the wait.</param>
	public CutsceneWait(float dt)
	{
		canSkip = true;
        runTimer = new Timer(dt);
	}
	public override Timer Run()
	{
		return null;
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

	public override Timer Run()
	{
		CutsceneActor actor = Cutscene.Instance.FindActor(mover);
		GameObject gameObject = actor ? actor.gameObject : GameObject.Find(mover);
		if (gameObject)
		{
			if (time > 0)
			{
				runTimer = new Timer(1.0f / 30.0f, (int)(time * 30));
				runTimer.name = "Move Timer";

				Vector3 startPosition = gameObject.transform.position;
				if (moveType == MoveTypes.X)
				{
					float xDist = x - gameObject.transform.position.x;
					runTimer.OnTick.AddListener(() =>
					{
						gameObject.transform.position = gameObject.transform.position.SetX(startPosition.x + xDist * runTimer.RunPercent);
					});

					runTimer.OnComplete.AddListener(() =>
					{
						gameObject.transform.position = startPosition.SetX(x);
					});
				}
				else if (moveType == MoveTypes.Y)
				{
					float yDist = y - gameObject.transform.position.y;
					runTimer.OnTick.AddListener(() =>
					{
						gameObject.transform.position = gameObject.transform.position.SetY(startPosition.y + yDist * runTimer.RunPercent);
					});

					runTimer.OnComplete.AddListener(() =>
                    {
                        gameObject.transform.position = startPosition.SetY(y);
                    });
				}
				else if (moveType == MoveTypes.XY)
				{
					Vector3 dist = new Vector3(x, y) - gameObject.transform.position;

					runTimer.OnTick.AddListener(() =>
					{
						gameObject.transform.position = startPosition + dist * runTimer.RunPercent;
					});

					runTimer.OnComplete.AddListener(() =>
                    {
						gameObject.transform.position = new Vector3(x,y, gameObject.transform.position.z);
                    });
				}
				else if (moveType == MoveTypes.Rotate)
				{
					runTimer.name = "Rotate Timer";
					float curRotation = 0;
					runTimer.OnTick.AddListener(() => {
                        gameObject.transform.Rotate(Vector3.forward, -curRotation);
						curRotation = ang * runTimer.RunPercent;
                        gameObject.transform.Rotate(Vector3.forward, curRotation);
                    });

                    runTimer.OnComplete.AddListener(() => {
                        gameObject.transform.Rotate(Vector3.forward, -curRotation);
                        gameObject.transform.Rotate(Vector3.forward, ang);
                    });
				}

				return runTimer;

			}
			else
			{
				Debug.Log(string.Format("{0}: {1} to {2}, {3}", moveType.ToString(), gameObject.name, x, y));
				if (moveType == MoveTypes.X) {
					gameObject.transform.position = gameObject.transform.position.SetX(x);
				} else if (moveType == MoveTypes.Y) {
					gameObject.transform.position = gameObject.transform.position.SetY(y);
				} else if (moveType == MoveTypes.XY) {
					gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);
				} else if (moveType == MoveTypes.Rotate) {
					gameObject.transform.Rotate(Vector3.forward, ang);
				}

				Debug.Log(gameObject.transform.position);
			}
		}
		return null;
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

	public override Timer Run()
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

		return null;
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

	public override Timer Run()
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

		return null;
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

	public override Timer Run()
	{
        CutsceneActor actor = Cutscene.Instance.FindActor(actorName);
		if (actor)
		{
			actor.Fade(alpha, time);
		}

		return null;
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
		prefab = Resources.Load<GameObject>("Prefabs/"+name);
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

public class CutsceneAdd : CutsceneElement
{
    string itemName;

	public CutsceneAdd(string item)
	{
        itemName = item;
		autoAdvance = true;
		canSkip = false;
	}

	public override Timer Run()
	{
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemName) as MagicItem, false);
		return null;
	}
}

public class CutsceneEnable : CutsceneElement
{
	GameObject hideObject;
	bool enable = true;
	bool move = false;
	Vector2 pos;
	string objectName = "";

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

	public CutsceneEnable(string oName, bool en) {
		hideObject = null;
		objectName = oName;
		enable = en;
	}

	public override Timer Run()
	{
		if (hideObject == null)
		{
			hideObject = GameObject.Find(objectName);
			Debug.Log(string.Format("{0}:{1}", objectName, hideObject == null));
		}
		if (hideObject)
        {
			hideObject.SetActive(enable);

			if (move)
			{
				hideObject.transform.position = pos;

			}

        }

		return null;
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

	public override Timer Run()
	{
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

public class CutsceneAlign : CutsceneElement
{
	bool left;

	public CutsceneAlign(bool l)
	{
		left = l;
		autoAdvance = true;
		canSkip = true;
	}

	public override Timer Run()
	{
        if (left) {
            EventManager.TriggerEvent("AlignLeft");
        } else {
            EventManager.TriggerEvent("AlignRight");
        }

		return null;
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

public class CutsceneAnimation: CutsceneElement {
    string triggerName;
    string actorName;

    public CutsceneAnimation(string aName, string tName) {
        triggerName = tName;
		actorName = aName;
    }

	public override Timer Run()
    {
		GameObject gameObject = GameObject.Find(actorName);
		if (gameObject) {
			Animator animator = gameObject.GetComponent<Animator>();
			if (animator) {
				animator.SetTrigger(triggerName);
				runTimer = new Timer(0.5f);
				return runTimer;
			}
		} else {
			CutsceneActor cutsceneActor = Cutscene.Instance.FindActor(actorName);
			if (cutsceneActor) {
				cutsceneActor.Animate(triggerName);
				runTimer = new Timer(0.5f);
                return runTimer;
			}
		}
		return null;
    }
}

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

public class SceneChange : CutsceneElement
{
	string newScene;

	public SceneChange(string scene)
	{
		newScene = scene;
		canSkip = false;
		autoAdvance = true;
	}

	public override Timer Run()
	{
        Game.Instance.LoadScene(newScene);
		return null;
	}
}