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

        EventManager.StartListening("ShowItemMenu", Reveal);
        EventManager.StartListening("HideItemMenu", Hide);
        Hide();

	}

    void Reveal() {
        gameObject.SetActive(true);

        for (int i = 0; i < Game.Instance.Player.Inventory.Count; i++) {
            boxes[i].SetItem(Game.Instance.Player.Inventory[i]);
        }
    }

    void Hide() {
        gameObject.SetActive(false);
    }
}
