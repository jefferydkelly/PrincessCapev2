using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : HeldItem{

    Vector3 startPosition;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        canBeThrown = false;
		startPosition = transform.position;
		EventManager.StartListening("PlayerRespawned", Reset);
    }

    private void Reset()
    {
        transform.position = startPosition;
    }
}
