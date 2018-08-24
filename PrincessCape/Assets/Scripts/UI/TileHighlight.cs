using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour {
    MapHighlightState state = MapHighlightState.Normal;
    SpriteRenderer myRenderer;
	// Use this for initialization
	void Awake () {
        myRenderer = GetComponent<SpriteRenderer>();
	}
	
    public MapHighlightState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;

            switch (state)
            {
                case MapHighlightState.Normal:
                    myRenderer.color = Color.white;
                    break;
                case MapHighlightState.Primary:
                    myRenderer.color = Color.blue;
                    break;
                case MapHighlightState.Backup:
                    myRenderer.color = Color.cyan;
                    break;
                case MapHighlightState.Secondary:
                    myRenderer.color = Color.red;
                    break;
            }
        }
    }
}
