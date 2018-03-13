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

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CameraManager"/> class.
    /// </summary>
    public CameraManager() {
        Game.Instance.AddManager(this);
        EventManager.StartListening("PlayerDied", ResetCamera);
        EventManager.StartListening("PlayerRespawned", OnPlayerRespawn);

    }

    void ResetCamera() {
        target = Checkpoint.Active;
        state = CameraState.Resetting;
        targetPos = target.transform.position.SetZ(Position.z);
    }

    void OnPlayerRespawn() {
        state = CameraState.Following;
        target = Game.Instance.Player.gameObject;
    }
    /// <summary>
    /// Update the specified dt.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="dt">Dt.</param>
    public void Update(float dt)
    {
		if (target)
        {
            if (state == CameraState.Following)
            {
                Position = target.transform.position.SetZ(Position.z);//new Vector3(target.transform.position.x, target.transform.position.y, Camera.main.transform.position.z);
            } else if (state == CameraState.Resetting) {

                if (Follow()) {
                    Position = targetPos;
                    state = CameraState.Following;
                }
            }
        }
    }

    bool Follow() {
        Position = Vector3.Lerp(Position, targetPos, Time.deltaTime);
        return Vector3.Distance(Position, targetPos) < 0.05f;
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

    public void Pan(Vector2 tar, float time) {
        state = CameraState.Panning;
        Vector3 startPos = Position;
		int ticks = Mathf.FloorToInt(time / 0.03f);
		Timer panTimer = new Timer(0.03f, ticks);
		panTimer.OnTick.AddListener(() => {
            Position = startPos + (Vector3)tar * panTimer.RunPercent;
		});

		panTimer.OnComplete.AddListener(() => {
            Position = startPos + (Vector3)tar;
			state = CameraState.Frozen;
			EventManager.TriggerEvent("ElementCompleted");
		});

		panTimer.Start();

	}

    public void PanTo(Vector2 tar, float time) {
        targetPos = new Vector3(tar.x, tar.y, Position.z);
        state = CameraState.Panning;
		Vector3 startPos = Position;
        Vector3 dif = targetPos - Position;


		int ticks = Mathf.FloorToInt(time / 0.03f);
		Timer panTimer = new Timer(0.03f, ticks);
		panTimer.OnTick.AddListener(() => {
			Position = startPos + dif * panTimer.RunPercent;
		});

		panTimer.OnComplete.AddListener(() => {
			Position = startPos + dif;
			state = CameraState.Frozen;
			EventManager.TriggerEvent("ElementCompleted");
		});

		panTimer.Start();

	}

    public void Pan(GameObject go, float time) {
        state = CameraState.Panning;

        Vector3 startPos = Position;
        Vector3 dif = (go.transform.position - Position).SetZ(0);
       
        int ticks = Mathf.FloorToInt(time / 0.03f);
        Timer panTimer = new Timer(0.03f, ticks);
        panTimer.OnTick.AddListener(()=> {
            Position = startPos + dif * panTimer.RunPercent;
        });

        panTimer.OnComplete.AddListener(()=> {
            Position = startPos + dif;
            state = CameraState.Frozen;
            EventManager.TriggerEvent("ElementCompleted");
        });

        panTimer.Start();
    }

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
