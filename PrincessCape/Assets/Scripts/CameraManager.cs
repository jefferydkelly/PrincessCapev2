using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Manager
{
    static CameraManager instance;
    CameraState state = CameraState.Following;
    GameObject target;

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
                Position = Vector3.Lerp(Position, target.transform.position.SetZ(Position.z), Time.deltaTime);
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
}

public enum CameraState {
    Following,
    Frozen,
    Resetting,
    Panning,
    Zooming
}
