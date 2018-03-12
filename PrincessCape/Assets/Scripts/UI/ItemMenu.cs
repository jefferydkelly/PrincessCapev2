using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class ItemMenu : MonoBehaviour {

    // Use this for initialization
    List<ItemSlot> boxes;
	void Awake () {
        boxes = GetComponentsInChildren<ItemSlot>().ToList();

        StartListening();
		EventManager.StartListening("ShowDialog", StopListening);
		EventManager.StartListening("ShowMessage", StopListening);
        EventManager.StartListening("EndOfMessage", StartListening);
        Hide();

	}

    void StopListening() {
        EventManager.StopListening("ShowItemMenu", Reveal);
        EventManager.StartListening("HideItemMenu", Hide);
    }

    void StartListening() {
        EventManager.StartListening("ShowItemMenu", Reveal);
        EventManager.StopListening("HideItemMenu", Hide);

    }
    void Reveal() {
        gameObject.SetActive(true);
        StopListening();
        for (int i = 0; i < Game.Instance.Player.Inventory.Count; i++) {
            boxes[i].SetItem(Game.Instance.Player.Inventory[i]);
        }
    }

    void Hide() {
        StartListening();
        gameObject.SetActive(false);
    }
}
