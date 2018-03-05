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
	void Awake () {
        background = GetComponent<Image>();
        itemImage = GetComponentsInChildren<Image>()[1];
        EventManager.StartListening("ShowMessage", StopListening);
        EventManager.StartListening("ShowDialog", StopListening);
        EventManager.StartListening("EndOfMessage", StartListening);
	}

    private void OnEnable()
    {
        StartListening();
    }

    private void OnDisable()
    {
        StopListening();
    }

    void StartListening() {
		string itemName = "Item" + (isFirstItem ? "One" : "Two");
		EventManager.StartListening(itemName + "Equipped", UpdateItemInfo);
		EventManager.StartListening(itemName + "ActivatedSuccessfully", BlueOut);
		EventManager.StartListening(itemName + "DeactivatedSuccessfully", GrayOut);
		EventManager.StartListening(itemName + "Cooldown", WhiteOut);
		EventManager.StartListening("Unequip" + itemName, Clear);
    }

    void StopListening() {
		string itemName = "Item" + (isFirstItem ? "One" : "Two");
		EventManager.StopListening(itemName + "Equipped", UpdateItemInfo);
		EventManager.StopListening(itemName + "ActivatedSuccessfully", BlueOut);
		EventManager.StopListening(itemName + "DeactivatedSuccessfully", GrayOut);
		EventManager.StopListening(itemName + "Cooldown", WhiteOut);
		EventManager.StopListening("Unequip" + itemName, Clear);
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

    void UpdateItemInfo() {
        foreach(MagicItem mi in Game.Instance.Player.Inventory) {
            if ((isFirstItem && mi.Slot == MagicItemSlot.First) || (!isFirstItem && mi.Slot == MagicItemSlot.Second)) {
                itemImage.sprite = mi.Sprite;
                return;
            }
        }
    }

    void Clear() {
        itemImage.sprite = null;
    }
}
