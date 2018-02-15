using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MapTile {
    [SerializeField]
    GameObject stair;
	public override void ScaleX(bool right)
    {
        if (right) {
            GameObject child = Instantiate(stair);
            child.transform.SetParent(transform);
            child.transform.localScale = stair.transform.localScale.SetY(1 + transform.childCount / 2.0f);
            child.transform.localPosition = transform.right * transform.childCount + (transform.up * (transform.childCount) / 4.0f);
        } else if (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject, false);
        }
    }
}
