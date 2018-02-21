using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class InputSaveLoadMenu : MonoBehaviour {

    [SerializeField]
    Text title;

    [SerializeField]
    LoadInputBrowser loader;

    [SerializeField]
    SaveInputMenu saver;
   
	// Use this for initialization
	void Awake () {
        EventManager.StartListening("OpenBrowserSave", OpenSave);
        EventManager.StartListening("OpenBrowserLoad", OpenLoad);
        gameObject.SetActive(false);
	}

    void OpenSave() {
        gameObject.SetActive(true);
        title.text = "Save";

        saver.gameObject.SetActive(true);
        loader.gameObject.SetActive(false);
    }

    void OpenLoad()
    {
        saver.gameObject.SetActive(false);
        gameObject.SetActive(true);
        title.text = "Load";

    }


}
