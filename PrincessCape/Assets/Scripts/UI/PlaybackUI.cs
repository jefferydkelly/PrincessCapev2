using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlaybackUI : MonoBehaviour {
    Button[] buttons;
	// Use this for initialization
	void Start () {
        buttons = GetComponentsInChildren<Button>();
        if (Game.Instance.IsInLevelEditor) {
         
            Game.Instance.OnEditorPlay.AddListener(() =>
            {
                NavActive = false;
            
            });



            Game.Instance.OnEditorPause.AddListener(() =>
            {
                NavActive = true;

            });

            Game.Instance.OnEditorStop.AddListener(() =>
            {
                NavActive = true;

            });
        }
	}

    bool NavActive {
        set {
            foreach(Button b in buttons) {
                Navigation navigation = b.navigation;
                navigation.mode = Navigation.Mode.None;
                b.navigation = value ? Navigation.defaultNavigation : navigation;
            }
        }
    }

    public void PlayInEditor() {
        Game.Instance.PlayInEditor();
    }

    public void PauseInEditor() {
        Game.Instance.PauseInEditor();
    }

}
