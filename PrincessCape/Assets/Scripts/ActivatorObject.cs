using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorObject : ActivatedObject {
    [SerializeField]
    //List<ActivatedObject> connected = new List<ActivatedObject>();

    /// <summary>
    /// Activate this instance.
    /// </summary>
    public override void Activate()
    {
        foreach(ActivatedObject ao in connections) {
            if (ao)
            {
                ao.Activate();
            }
        }
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public override void Deactivate()
    {
        foreach (ActivatedObject ao in connections)
		{
            if (ao)
            {
                ao.Deactivate();
			}
		}
    }
}
