using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ControlsMenu : MainMenu {
    Dictionary<string, KeyCode> keyCodes;
    [SerializeField]
    ControlMenuButton[] buttons;
    string inputToAssign = "";
	// Use this for initialization
	void Start () {
        //buttons = FindObjectsOfType<ControlMenuButton>();
        keyCodes = new Dictionary<string, KeyCode>();
        keyCodes.Add("Forward", KeyCode.D);
        keyCodes.Add("Backward", KeyCode.A);
        keyCodes.Add("Up", KeyCode.W);
        keyCodes.Add("Down", KeyCode.S);
        keyCodes.Add("Jump", KeyCode.Space);
        keyCodes.Add("Interact", KeyCode.F);
        keyCodes.Add("First Item", KeyCode.Mouse0);
        keyCodes.Add("Second Item", KeyCode.Mouse1);
        keyCodes.Add("Pause", KeyCode.P);
        EventManager.StartListening("UpdateKeys", UpdateKeys);
        UpdateKeys();

	}

    private void Update()
    {
        if (keyCodes.ContainsKey(inputToAssign) && Input.anyKeyDown) {
            KeyCode code = KeyCode.None;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                code = KeyCode.LeftArrow;
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                code = KeyCode.RightArrow;
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                code = KeyCode.UpArrow;
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                code = KeyCode.DownArrow;
            } else {
                code = (KeyCode)System.Enum.Parse(typeof(KeyCode), Input.inputString.ToUpper());
            }

            //keyCodes[inputToAssign] = (KeyCode)System.Enum.Parse(typeof(KeyCode), Input.inputString);

            foreach(ControlMenuButton cmb in buttons) {
                if (cmb.InputName == inputToAssign) {
                    keyCodes[inputToAssign] = code;
                    cmb.ButtonText = code.ToString();
                } else if (cmb.ButtonText == code.ToString()) {
                    keyCodes[cmb.InputName] = KeyCode.None;
                    cmb.ButtonText = KeyCode.None.ToString();
                }
            }
            inputToAssign = "";
        }
    }

    public void SaveControls() {
        Controller.Instance.SetKeys(keyCodes);
        EventManager.TriggerEvent("OpenBrowserSave");
    }

    public void LoadControls() {
        EventManager.TriggerEvent("OpenBrowserLoad");
    }

    public void RestoreDefaults() {
		keyCodes["Forward"] = KeyCode.D;
		keyCodes["Backward"] = KeyCode.A;
		keyCodes["Up"] = KeyCode.W;
		keyCodes["Down"] = KeyCode.S;
		keyCodes["Jump"] = KeyCode.Space;
		keyCodes["Interact"] = KeyCode.F;
		keyCodes["First Item"] = KeyCode.Mouse0;
		keyCodes["Second Item"] = KeyCode.Mouse1;
		keyCodes["Pause"] = KeyCode.P;
        UpdateKeys();
    }

    void UpdateKeys() {
		int i = 0;

        foreach (KeyValuePair<string, KeyCode> kp in Controller.Instance.KeyDict)
		{
			EventManager.StartListening("Assign " + kp.Key, () => { inputToAssign = kp.Key; });
			buttons[i].SetText(kp.Key, kp.Value.ToString());
			i++;
		}
    }
}