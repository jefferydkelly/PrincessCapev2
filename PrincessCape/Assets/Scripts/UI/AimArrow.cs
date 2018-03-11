using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimArrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventManager.StartListening("ShowAim", Activate);
        EventManager.StartListening("HideAim", ()=> {
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        Rotate();
	}

    void Activate() {
        gameObject.SetActive(true);
        Rotate();
    }

    void Rotate() {
        Vector2 input = Controller.Instance.DirectionalInput;

        if (input.sqrMagnitude <= 0) {
            input = Game.Instance.Player.Forward;
        }

        transform.rotation = Quaternion.AngleAxis(input.Angle().ToDegrees(), Vector3.forward);
    }
}
