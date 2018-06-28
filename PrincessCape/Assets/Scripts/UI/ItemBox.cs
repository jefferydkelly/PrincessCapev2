using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour {
    Image background;
    Image itemImage;
    Text keyBox;
    [SerializeField]
    bool isFirstItem = true;
	// Use this for initialization
	void Awake () {
        background = GetComponent<Image>();
        itemImage = GetComponentsInChildren<Image>()[1];

        keyBox = transform.parent.GetComponentInChildren<Text>();
		//keyBox.text = Controller.Instance.GetKey(isFirstItem ? "ItemOne" : "ItemTwo");

	}

    private void OnEnable()
    {
        StartListening();

		if (Game.Instance && Game.Instance.Player && Game.Instance.Player.Inventory.Count == 0)
		{
			IsHidden = true;
		}
    }

    private void OnDisable()
    {
        StopListening();
    }

    public void StartListening() {
		string itemName = "Item" + (isFirstItem ? "One" : "Two");
		EventManager.StartListening(itemName + "Equipped", UpdateItemInfo);
		EventManager.StartListening(itemName + "ActivatedSuccessfully", BlueOut);
		EventManager.StartListening(itemName + "DeactivatedSuccessfully", GrayOut);
		EventManager.StartListening(itemName + "Cooldown", WhiteOut);
		EventManager.StartListening("Unequip" + itemName, Clear);
    }

    public void StopListening() {
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

        if (IsHidden) {
            IsHidden = false;
        }
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

	public bool IsHidden
	{
		set
		{
			foreach (Image i in transform.parent.GetComponentsInChildren<Image>())
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

    public string KeyText {
        get {
            return keyBox.text;
        }

        set {
            keyBox.text = value;
        }
    }
}
