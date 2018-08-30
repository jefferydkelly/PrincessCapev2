using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimArrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UIManager.Instance.OnAimStatusChange.AddListener((bool show) =>
        {
            gameObject.SetActive(show);

            if (show) {
                Rotate();
            }
        });
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        Rotate();
	}

    void Rotate() {
		Vector2 input = Controller.Instance.Aim;

        if (input.sqrMagnitude <= 0) {
            input = Game.Instance.Player.Forward;
        }

        transform.rotation = Quaternion.AngleAxis(input.Angle().ToDegrees(), Vector3.forward);
    }
}
