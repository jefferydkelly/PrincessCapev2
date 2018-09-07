using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour {

    bool isSelected = false;
    MovingPlatform platform;
	// Use this for initialization
	void Awake () {
        platform = GetComponentInParent<MovingPlatform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDrag()
    {
        transform.position = Controller.Instance.MousePosition;
        platform.EndPoint = transform.position;
    }


}
