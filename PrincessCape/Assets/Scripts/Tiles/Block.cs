using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : InteractiveObject{

    Vector3 startPosition;
    bool isBeingPushed = false;
    private void Start()
    {
        startPosition = transform.position;
        EventManager.StartListening("PlayerRespawned", Reset);
    }
    public override void Activate()
    {
        //Let the player push
        isBeingPushed = !isBeingPushed;
        if (isBeingPushed) {
            EventManager.TriggerEvent("StartPush");
        } else {
            EventManager.TriggerEvent("StopPush");
        }

    }

    private void Reset()
    {
        transform.position = startPosition;
    }
}
