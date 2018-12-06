using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class InteractionBox : MonoBehaviour
{

    Text textbox;
    Text interactionKey;
    // Use this for initialization
    void OnEnable()
    {
        textbox = GetComponentInChildren<Text>();
        textbox.text = "";
        if (Game.IsBeingPlayed)
        {
            Game.Instance.OnReady.AddListener(() =>
            {
                IsHidden = Game.Instance.Map.Items == ItemLevel.None;
                textbox.text = "";

                Map.Instance.OnLevelLoaded.AddListener(() =>
                {
                    IsHidden = Game.Instance.Map.Items == ItemLevel.None;
                    textbox.text = "";
                });
            });
        }

        /*
        EventManager.StartListening("LevelLoaded", () =>
        {
            IsHidden = Game.Instance.Map.Items == ItemLevel.None;
            textbox.text = "";
        });*/

        interactionKey = transform.parent.GetComponentsInChildren<Text>()[1];
        //interactionKey.text = Controller.Instance.GetKey("Interact");

        EventManager.StartListening("ClearIntearction", ClearText);
    }

    void ClearText()
    {
        textbox.text = "";
    }

    public bool IsHidden
    {
        set
        {
            foreach (Image i in GetComponentsInParent<Image>())
            {
                i.enabled = !value;
            }

            foreach (Text t in transform.parent.GetComponentsInChildren<Text>())
            {
                t.enabled = !value;
            }

        }

        get
        {
            return !GetComponent<Image>().enabled;
        }
    }

    public string Text
    {
        get
        {
            return textbox.text;
        }

        set
        {
            if (textbox)
            {
                textbox.text = value;
            }
        }
    }

    public string KeyText {
        get {
            return interactionKey.text;
        }

        set {
            if (interactionKey)
            {
                interactionKey.text = value;
            }
        }
    }
}
