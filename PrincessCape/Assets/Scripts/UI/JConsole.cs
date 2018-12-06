using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JConsole {
    
    Text logText;
    static JConsole instance;

    JConsole() {
        logText = GameObject.Find("JConsole").GetComponent<Text>();
    }

    public void Log(string log) {
        logText.text += log + '\n';
    }

    public void Toggle() {
        if (logText)
        {
            logText.enabled = !logText.enabled;
        }

    }
    public static JConsole Instance {
        get {
            if (instance == null) {
                instance = new JConsole();
            }
            return instance;
        }
    }


}
