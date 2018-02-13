using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler {
    MagicItem item;
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void SetItem(MagicItem mi) {
        item = mi;
        if (item != null)
        {
            image.sprite = mi.Sprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (item.Slot == MagicItemSlot.Second)
            {
                EventManager.TriggerEvent("SwapItems");
            }
            else
            {
                item.RegisterItemOne();
            }
        } else if (eventData.button == PointerEventData.InputButton.Right){
            if (item.Slot == MagicItemSlot.First)
            {
                EventManager.TriggerEvent("SwapItems");
            }
            else
            {
                item.RegisterItemTwo();
            }
        }
    }
}
