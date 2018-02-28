using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarShield : MagicItem
{
    public StarShield() {
		itemName = "Star Shield";
		itemGetMessage = new List<string>() {
			"You got the Star Shield!",
			"Press and hold the item button to reflect projectiles away from you"
		};
    }

    private void OnEnable()
    {
        itemSprite = Resources.Load<Sprite>("Sprites/Shield");
    }
    public override void Activate()
    {
        Game.Instance.Player.IsUsingShield = true;
    }

    public override void Deactivate()
    {
        Game.Instance.Player.IsUsingShield = false;
    }
}
