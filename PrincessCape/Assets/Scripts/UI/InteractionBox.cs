using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBox : MonoBehaviour {

    Text textbox;
    Text interactionKey;
	// Use this for initialization
	void OnEnable () {
        EventManager.StartListening("SetInteraction", SetText);
		textbox = GetComponentInChildren<Text>();
		textbox.text = "";
        EventManager.StartListening("LevelLoaded", ()=> {
            IsHidden = Game.Instance.Map.Items == ItemLevel.None;
            textbox.text = "";
        });

        interactionKey = transform.parent.GetComponentInChildren<Text>();
        interactionKey.text = Controller.Instance.GetKey("Interact");

        EventManager.StartListening("ClearIntearction", ClearText);
	}

    private void OnDisable()
    {
        EventManager.StopListening("SetInteraction", SetText); 
    }

    void SetText() {
        if (IsHidden) {
            IsHidden = false;
        }
        if (InteractiveObject.Selected != null) {
            textbox.text = InteractiveObject.Selected.Interaction;
        } else {
            textbox.text = "";
        }
    }

    void ClearText() {
        textbox.text = "";
    }

    bool IsHidden {
        set {
            foreach(Image i in GetComponentsInParent<Image>()) {
                i.enabled = !value;
            }

            foreach(Text t in transform.parent.GetComponentsInChildren<Text>()) {
                t.enabled = !value;
            }
           
        }

        get {
            return !GetComponent<Image>().enabled;
        }
    }
}
