using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagneticField : MonoBehaviour {

    SpriteRenderer myRenderer;
	// Use this for initialization
	void Awake () {
        myRenderer = GetComponent<SpriteRenderer>();
	}
	
    public bool IsPull {
        get {
            return myRenderer.color == Color.blue;
        }

        set {
            myRenderer.color = value ? Color.blue : Color.red;
        }
    }
	// Update is called once per frame
	void Update () {
        Vector2 aim = Controller.Instance.Aim;
        transform.rotation = Quaternion.AngleAxis(aim.Angle().ToDegrees(), Vector3.forward);
	}


}
