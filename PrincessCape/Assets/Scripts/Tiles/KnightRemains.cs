using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightRemains : InteractiveObject
{

    [SerializeField]
    MagicItemType itemOnRemains = MagicItemType.Cape;
    bool itemGiven = false;

    public override void Activate()
    {
        if (!itemGiven)
        {
            //Add item to the player's inventory
            MessageBox.SetMessage(new List<string>() { "Now this is a story all about how...", "My life got flipped turned upside down", "And I'd like to take a minute, just sit right there", "I'll tell you all about how I became the prince of a town called Bel-Air" });
            EventManager.StartListening("EndOfMessage", GiveItem);
            EventManager.TriggerEvent("Pause");
            EventManager.TriggerEvent("ShowMessage");

        }
    }

    void GiveItem()
    {
        itemGiven = true;
        IsHighlighted = false;
        EventManager.StopListening("EndOfMessage", GiveItem);
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemOnRemains.ToString()) as MagicItem);
    }

    /// <summary>
    /// Highlight the object when it touches the player
    /// </summary>
    /// <param name="collision">Collision.</param>
    public new void OnTriggerEnter2D(Collider2D collision)
    {
        if (!itemGiven && collision.CompareTag("Player"))
        {
            IsHighlighted = true;
        }
    }
}

public enum MagicItemType
{
    Cape,
    PullGlove,
    PushGlove
}