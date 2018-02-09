using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : ActivatedObject
{
    [SerializeField]
    GameObject bridgeTile;
    [SerializeField]
    bool startActive = false;

    private void Start()
    {
        if (!startActive) {
            Deactivate();
        }
    }
    public override void Activate()
    {
        StartCoroutine(RevealTiles());
    }

    IEnumerator RevealTiles() {
        for (int i = 0; i < transform.childCount; i++) {
            GameObject go = transform.GetChild(i).gameObject;
            go.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    public override void Deactivate()
    {
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject go = transform.GetChild(i).gameObject;
            go.SetActive(false);
			
		}
    }

    public override void ScaleY(bool up)
    {
        
    }

    public override void ScaleX(bool right)
    {
        if (right) {
            GameObject tile = Instantiate(bridgeTile);
            tile.transform.SetParent(transform);
            tile.transform.localPosition = new Vector3(transform.childCount, -0.5f, 0);
        } else if (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }
    }
}
