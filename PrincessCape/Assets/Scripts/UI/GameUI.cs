using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
      
		Cutscene.Instance.OnStart.AddListener(() => {
			gameObject.SetActive(false);
		});

        Cutscene.Instance.OnEnd.AddListener(() => {
            gameObject.SetActive(true);
		});
	}
}
