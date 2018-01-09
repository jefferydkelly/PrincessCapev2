using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Manager
{
    static CameraManager instance;
    GameObject target;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CameraManager"/> class.
    /// </summary>
    public CameraManager() {
        Game.Instance.AddManager(this);
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
            Camera.main.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, Camera.main.transform.position.z);
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
}

public enum CameraState {
    Following,
    Frozen,
    Resetting,
    Panning,
    Zooming
}
