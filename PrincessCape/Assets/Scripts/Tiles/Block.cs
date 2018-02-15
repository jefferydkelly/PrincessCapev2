using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : InteractiveObject{

    Vector3 startPosition;
    bool isBeingPushed = false;
    Vector3 playerDif;
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
            playerDif = transform.position - Game.Instance.Player.transform.position;
        } else {
            EventManager.TriggerEvent("StopPush");
        }

    }

    private void Reset()
    {
        transform.position = startPosition;
    }

    private void Update()
    {
        if (isBeingPushed) {
            transform.position = Game.Instance.Player.transform.position + playerDif;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isBeingPushed) {
            IsHighlighted = false;
        }
    }
}
