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
            } else if (state == CameraState.Resetting || state == CameraState.Panning) {
                Position = Vector3.Lerp(Position, targetPos, Time.deltaTime);

                if (Vector3.Distance(Position, targetPos) < 0.05f) {
                    Position = targetPos;
                    if (state == CameraState.Panning) {
                        Cutscene.Instance.NextElement();
                    }
                    state = CameraState.Following;
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
            if (instance == null) {
                instance = new CameraManager();
            }
            return instance;
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

    public void Pan(GameObject tar) {
        targetPos = tar.transform.position.SetZ(Position.z);
        state = CameraState.Panning;
    }

    public void Pan(Vector2 tar) {
        targetPos = Position + (Vector3)tar;
        state = CameraState.Panning;
    }

    public void PanTo(Vector2 tar) {
        targetPos = new Vector3(tar.x, tar.y, Position.z);
        state = CameraState.Panning;

    }
}

public enum CameraState {
    Following,
    Frozen,
    Resetting,
    Panning,
    Zooming
}
