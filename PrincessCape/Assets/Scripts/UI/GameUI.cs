using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventManager.StartListening("StartCutscene", ()=> {
            gameObject.SetActive(false);
        });

        EventManager.StartListening("EndCutscene", ()=> {
            gameObject.SetActive(true);
        });
	}
}
