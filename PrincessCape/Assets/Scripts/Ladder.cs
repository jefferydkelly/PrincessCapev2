using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MapTile {

    public override void ScaleY(bool up)
    {
        if (up) {

            transform.position += Vector3.up;
            GameObject newChain = Instantiate(gameObject);
           
            newChain.transform.SetParent(transform);
			newChain.transform.position = transform.position + Vector3.down * (transform.childCount - 1);
        } else if (transform.childCount > 1) {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
            transform.position += Vector3.down;
        }
    }
}
