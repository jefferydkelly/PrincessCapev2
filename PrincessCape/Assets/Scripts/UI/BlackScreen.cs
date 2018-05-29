using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour {

    Image image;
	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
		Map.Instance.OnLevelLoaded.AddListener(() => {
            image.color = image.color.SetAlpha(0);
        });

        EventManager.StartListening("LevelOver",()=> {
            image.color = image.color.SetAlpha(1);
        });

        image.color = image.color.SetAlpha(0);
	}
}
