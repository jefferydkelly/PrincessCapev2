using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightRemains : InteractiveObject {

    [SerializeField]
    MagicItemType itemOnRemains = MagicItemType.Cape;
	
    public override void Activate()
    {
        //Add item to the player's inventory
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemOnRemains.ToString()) as MagicItem);    
    }
}

public enum MagicItemType {
    Cape,
    PullGlove,
    PushGlove
}