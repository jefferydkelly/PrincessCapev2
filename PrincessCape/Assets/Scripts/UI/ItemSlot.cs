using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
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
                SwapItems();
            }
            else
            {
                UIManager.Instance.ItemOne.Item = item;
            }
        } else if (eventData.button == PointerEventData.InputButton.Right){
            if (item.Slot == MagicItemSlot.First)
            {
                SwapItems();
            }
            else
            {
                UIManager.Instance.ItemTwo.Item = item;
            }
        }
    }

    void SwapItems() {
        MagicItem temp = UIManager.Instance.ItemOne.Item;
        UIManager.Instance.ItemOne.Item = UIManager.Instance.ItemTwo.Item;
        UIManager.Instance.ItemTwo.Item = temp;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            UIManager.Instance.SetMainText(item.Description);
            //EventManager.TriggerEvent("ShowLine");
            image.color = Color.gray;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }
}
