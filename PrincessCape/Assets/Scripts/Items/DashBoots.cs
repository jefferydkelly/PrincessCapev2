using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBoots : MagicItem
{

    public DashBoots()
    {
        itemName = "Dash Boots";
        itemGetMessage = new List<string>() {
            "You got the Dash Boots!",
            "Press and hold to dash with all your might but be careful.  You won't be able to stop until you hit something"
        };

        itemDescritpion = "Press and hold the Item Key to run like the wind.";
    }

    private void OnEnable()
    {
        itemSprite = Resources.Load<Sprite>("Sprites/DashBoots");
    }

    public override void Activate()
    {
        Player player = Game.Instance.Player;
        if (!player.IsFrozen && player.IsOnGround) {
            player.IsDashing = true;
            player.Rigidbody.velocity = player.Forward * 20;
        }
    }

    public override void Deactivate()
    {
        KeyCode code = slot == MagicItemSlot.First ? Controller.Instance.KeyDict["ItemOne"] : Controller.Instance.KeyDict["ItemTwo"];
        if (!Input.GetKeyUp(code))
        {
            Game.Instance.Player.IsDashing = false;
        }
    }
}
