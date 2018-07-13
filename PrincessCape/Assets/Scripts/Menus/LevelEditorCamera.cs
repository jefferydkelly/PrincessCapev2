using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorCamera : MonoBehaviour {
    [SerializeField]
    float moveSpeed = 3;
	
	// Update is called once per frame
	void Update () {
        
	}

    private void LateUpdate()
    {
        if (!Game.Instance.IsPlaying)
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
        }
    }
}
