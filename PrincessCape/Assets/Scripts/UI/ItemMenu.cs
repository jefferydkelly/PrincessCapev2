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
        gameObject.SetActive(false);

	}

    private void OnEnable()
    {
        if (Game.Instance && Game.Instance.Player)
        {
            for (int i = 0; i < Game.Instance.Player.Inventory.Count; i++)
            {
                boxes[i].SetItem(Game.Instance.Player.Inventory[i]);
            }
        }
    }
}
