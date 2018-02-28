using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MagicItem
{
    public Shield() {
		itemName = "Star Shield";
		itemGetMessage = new List<string>() {
			"You got the Star Shield!",
			"Press and hold the item button to reflect projectiles away from you"
		};
    }
    public override void Activate()
    {
        throw new NotImplementedException();
    }

    public override void Deactivate()
    {
        throw new NotImplementedException();
    }
}
