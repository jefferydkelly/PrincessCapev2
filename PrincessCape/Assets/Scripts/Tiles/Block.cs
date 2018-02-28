using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : InteractiveObject{

    Vector3 startPosition;
    bool isBeingPushed = false;
    Vector3 playerDif;
    Rigidbody2D myRigidbody;
    private void Start()
    {
        startPosition = transform.position;
        myRigidbody = GetComponent<Rigidbody2D>();
        EventManager.StartListening("PlayerRespawned", Reset);
    }
    public override void Interact()
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
            if (Mathf.Abs(Game.Instance.Player.Rigidbody.velocity.x) < 0.5f)
            {
                myRigidbody.velocity = Vector3.right * Controller.Instance.Horizontal;// * maxSpeed / 4.0f;
            } else {
                transform.position = Game.Instance.Player.transform.position.SetY(transform.position.y) + Game.Instance.Player.Forward;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isBeingPushed) {
            IsHighlighted = false;
        }
    }
}
