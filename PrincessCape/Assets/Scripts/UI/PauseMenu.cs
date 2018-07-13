using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    // Use this for initialization
    private void Awake()
    {
        Controller.Instance.OnPause.AddListener(Toggle);
        gameObject.SetActive(false);
    }

    void Toggle()
    {
        if (!Game.Instance.IsInCutscene && !Game.Instance.IsInLevelEditor)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
