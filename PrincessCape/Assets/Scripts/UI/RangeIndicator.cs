using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour {

    MovingPlatform platform;
	// Use this for initialization
	void Awake () {
        platform = GetComponentInParent<MovingPlatform>();
	}

    /// <summary>
    /// Change the direction of the moving platform when the indicator is dragged around.
    /// </summary>
    private void OnMouseDrag()
    {
        transform.position = Controller.Instance.MousePosition;
        platform.EndPoint = transform.position;
    }


}
