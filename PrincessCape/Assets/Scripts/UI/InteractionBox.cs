using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBox : MonoBehaviour {

    Text textbox;
	// Use this for initialization
	void OnEnable () {
        EventManager.StartListening("SetInteraction", SetText);
        textbox = GetComponentInChildren<Text>();
	}

    private void OnDisable()
    {
        EventManager.StopListening("SetInteraction", SetText); 
    }

    void SetText() {
        if (InteractiveObject.Selected != null) {
            textbox.text = InteractiveObject.Selected.Interaction;
        } else {
            textbox.text = "";
        }
    }
}
