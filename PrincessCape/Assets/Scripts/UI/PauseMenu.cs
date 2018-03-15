using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    // Use this for initialization
    private void Awake()
    {
        EventManager.StartListening("Pause", Toggle);
        gameObject.SetActive(false);
    }

    void Toggle()
    {
        if (!Game.Instance.IsInCutscene)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
