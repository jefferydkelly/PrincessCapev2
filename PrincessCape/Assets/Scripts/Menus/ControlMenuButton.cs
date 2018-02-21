using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ControlMenuButton : MonoBehaviour {

    Text inputName;
    Button button;
    Text buttonText;
 
	// Use this for initialization
	void Awake () {
        inputName = GetComponentInChildren<Text>();
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(()=>{EventManager.TriggerEvent("Assign " + inputName.text);});
        buttonText = button.GetComponentInChildren<Text>();
	}
	
    public void SetText(string input, string keyCode) {
        inputName.text = input;
        buttonText.text = keyCode;
    }

    public string InputName {
        get {
            return inputName.text;
        }
    }

    public string ButtonText {
        get {
            return buttonText.text;
        }

        set {
            buttonText.text = value;
        }
    }

}
