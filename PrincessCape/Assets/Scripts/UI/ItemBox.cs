using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour {
    Image background;
    Image itemImage;
    [SerializeField]
    bool isFirstItem = true;
	// Use this for initialization
	void Start () {
        background = GetComponent<Image>();
        itemImage = GetComponentsInChildren<Image>()[1];
	}

    private void OnEnable()
    {
        string itemName = "Item" + (isFirstItem ? "One" : "Two");
        EventManager.StartListening(itemName + "ActivatedSuccessfully", BlueOut);
        EventManager.StartListening(itemName + "DeactivatedSuccessfully", GrayOut);
        EventManager.StartListening(itemName + "Cooldown", WhiteOut);
    }

    private void OnDisable()
    {
        string itemName = "Item" + (isFirstItem ? "One" : "Two");
        EventManager.StopListening(itemName + "ActivatedSuccessfully", BlueOut);
        EventManager.StopListening(itemName + "DeactivatedSuccessfully", GrayOut);
        EventManager.StopListening(itemName + "Cooldown", WhiteOut);
    }

    void GrayOut() {
        background.color = Color.gray;
    }

    void WhiteOut() {
        background.color = Color.white;
    }

    void BlueOut() {
        background.color = Color.blue;
    }
}
