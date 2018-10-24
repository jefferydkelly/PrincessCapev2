using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Manager
{
    static CameraManager instance;
    CameraState state = CameraState.Following;
    GameObject target;
    Vector3 targetPos;
    Vector3 offset = Vector3.up * 2;
    Timer panTimer;
    /// <summary>
    /// Initializes a new instance of the <see cref="T:CameraManager"/> class.
    /// </summary>
    public CameraManager() {
        Game.Instance.AddManager(this);
		Game.Instance.OnReady.AddListener(() =>
		{
			Game.Instance.Player.OnDie.AddListener(ResetCamera);
			Game.Instance.Player.OnRespawn.AddListener(OnPlayerRespawn);
			Game.Instance.Player.OnLand.AddListener(PanToPlayer);
			Cutscene.Instance.OnStart.AddListener(() =>
			{
				state = CameraState.Frozen;
			});

			Cutscene.Instance.OnEnd.AddListener(() =>
			{
				state = CameraState.Following;
			});
		});

             
        Game.Instance.Map.OnLevelLoaded.AddListener(PanToPlayer);
        //Game.Instance.OnEditorStop.AddListener(CenterOnPlayer);
        //EventManager.StartListening("LevelLoaded", PanToPlayer);

    }

    /// <summary>
    /// Pans to player.
    /// </summary>
    void PanToPlayer() {
        if (!Game.Instance.Player.IsDead)
        {
            Vector3 panPos = Game.Instance.Player.transform.position.SetZ(Position.z) + offset;
			PanTo(panPos, 0.25f).Start();
        }
    }

    public void CenterOnPlayer() {
        Position = Game.Instance.Player.transform.position.SetZ(Position.z) + offset;
    }
    /// <summary>
    /// Pans to the respawn point when the player dies
    /// </summary>
    void ResetCamera() {
        Vector3 panPos = Checkpoint.ResetPosition.SetZ(Position.z) + offset;
		PanTo(panPos, 1.0f).Start();
    }

    /// <summary>
    /// Starts following the player again when they respawn
    /// </summary>
    void OnPlayerRespawn() {
        state = CameraState.Following;
        target = Game.Instance.Player.gameObject;
        Position = target.transform.position.SetZ(Position.z) + offset;
    }
    /// <summary>
    /// Update the specified dt.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="dt">Dt.</param>
    public void Update(float dt)
    {
        if (target && Game.Instance.IsPlaying)
        {
            if (state == CameraState.Following)
            {
                if (!Game.Instance.Player.IsOnScreen) {
					Position = Game.Instance.Player.transform.position.SetZ(Position.z);
                } else {
                    if (Game.Instance.Player.IsOnLadder || (Game.Instance.Player.IsFloating) || (Game.Instance.Player.IsUsingMagneticGloves && Mathf.Abs(Game.Instance.Player.Velocity.y) > 0))
                    {
                        Position = Game.Instance.Player.transform.position.SetZ(Position.z);
                    } else
                    {
                        Position = Position.SetX(target.transform.position.x);
                    } 
                }
            }
        }
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static CameraManager Instance {
        get {
            if (!Game.isClosing)
            {
                if (instance == null)
                {
                    instance = new CameraManager();
                }
                return instance;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    /// <value>The target.</value>
    public GameObject Target {
        get {
            return target;
        }

        set {
            if (value) {
                target = value;
            }
        }
    }

    /// <summary>
    /// Gets the position of the main camera.
    /// </summary>
    /// <value>The position of the main camera.</value>
    private Vector3 Position {
        get {
            return Camera.main.transform.position;
        }

        set {
            Camera.main.transform.position = value;
        }
    }

    void SetPanTimer(float time) {
        int ticks = Mathf.FloorToInt(time / 0.03f);
        if (panTimer != null && panTimer.IsRunning)
        {
            panTimer.OnComplete.Invoke();
            panTimer.Stop();
        }
        panTimer = new Timer(0.03f, ticks);
    }
    /// <summary>
    /// Pans the camera by the specified amount over the given time
    /// </summary>
    /// <returns>The pan.</returns>
    /// <param name="tar">The distance the camera will pan.</param>
    /// <param name="time">The time it will take for the camera to pan.</param>
    public Timer Pan(Vector2 tar, float time) {
        state = CameraState.Panning;
        Vector3 startPos = Position;
        SetPanTimer(time);
		panTimer.OnTick.AddListener(() => {
            Position = startPos + (Vector3)tar * panTimer.RunPercent;
		});

		panTimer.OnComplete.AddListener(() => {
            Position = startPos + (Vector3)tar;
			state = Game.Instance.IsInCutscene ? CameraState.Frozen : CameraState.Following;
		});

		//panTimer.Start();
		return panTimer;

	}

    /// <summary>
    /// Pans the camera to the given position
    /// </summary>
    /// <param name="tar">The position the camera will pan to.</param>
    /// <param name="time">The length og the pan in seconds.</param>
	public Timer PanTo(Vector2 tar, float time) {
        targetPos = new Vector3(tar.x, tar.y, Position.z);
        state = CameraState.Panning;
		Vector3 startPos = Position;
        Vector3 dif = targetPos - Position;

        SetPanTimer(time);
		panTimer.OnTick.AddListener(() => {
			Position = startPos + dif * panTimer.RunPercent;
		});

		panTimer.OnComplete.AddListener(() => {
			Position = targetPos;
			state = Game.Instance.IsInCutscene ? CameraState.Frozen : CameraState.Following;
		});

		//panTimer.Start();
		return panTimer;
	}

    /// <summary>
    /// Pans the camera to the specified GameObject
    /// </summary>
    /// <param name="go">The GameObject the camera will pan to.</param>
    /// <param name="time">The length of the pan in seconds.</param>
    public Timer Pan(GameObject go, float time) {
        state = CameraState.Panning;
        
        Vector3 startPos = Position;
        Vector3 dif = (go.transform.position - Position).SetZ(0);

        SetPanTimer(time);
        panTimer.OnTick.AddListener(()=> {
            Position = startPos + dif * panTimer.RunPercent;
        });

        panTimer.OnComplete.AddListener(()=> {
			Position = go.transform.position.SetZ(Position.z);
            state = Game.Instance.IsInCutscene ? CameraState.Frozen : CameraState.Following;
            //EventManager.TriggerEvent("ElementCompleted");
        });

		//panTimer.Start();
		return panTimer;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:CameraManager"/> is following.
    /// </summary>
    /// <value><c>true</c> if is following; otherwise, <c>false</c>.</value>
    public bool IsFollowing {
        get {
            return state == CameraState.Following;
        }

        set {
            if (value) {
                state = CameraState.Following;
            } else {
                state = CameraState.Frozen;
            }
        }
    }
}

public enum CameraState {
    Following,
    Frozen,
    Resetting,
    Panning,
    Zooming
}
