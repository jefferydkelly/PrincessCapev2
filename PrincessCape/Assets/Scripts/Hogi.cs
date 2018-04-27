using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hogi : MonoBehaviour {
    bool hasBeenDefeated = false;
    Timer collapseTimer;
    Bridge bridgeToCollapse;
    private void Awake()
    {
        bridgeToCollapse = FindObjectOfType<Bridge>();
        collapseTimer = new Timer(1.0f / 60, 90);
        collapseTimer.OnTick.AddListener(() =>
        {
            transform.Rotate(0, 0, 1);
            transform.position += Vector3.down * 1.0f / 90;
        });

        collapseTimer.OnComplete.AddListener(() => {
            Game.Instance.Player.IsXFrozen = true;
            bridgeToCollapse.Deactivate();
                                             
        });
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Light") && !hasBeenDefeated) {
            hasBeenDefeated = true;
            collapseTimer.Start();
        }
    }
}
