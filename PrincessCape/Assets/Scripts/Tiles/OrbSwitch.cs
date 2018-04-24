using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class OrbSwitch : ActivatorObject
{

    Animator myAnimatior;

    public override void Init() {
		myAnimatior = GetComponent<Animator>();
        isActivated = startActive;
        if (startActive) {
            Activate();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            IsActivated = !IsActivated;
            Debug.Log(isActivated);
            if (IsActivated)
            {
                Activate();

            }
            else
            {
                Deactivate();

            }
        }
    }

    public override void Activate()
    {
        base.Activate();
        myAnimatior.SetTrigger("Activate");
    }

    public override void Deactivate()
    {
        base.Deactivate();
        myAnimatior.SetTrigger("Deactivate");
    }

#if UNITY_EDITOR
    public override void RenderInEditor()
    {
        foreach (ActivatedObject acto in Connections)
        {
            if (acto)
            {
                Handles.DrawDottedLine(transform.position, acto.transform.position, 8.0f);
            }
        }

        if (startActive)
        {
            Handles.color = Color.green;

        }
        else
        {
            Handles.color = Color.red;
        }
        Handles.DrawSolidArc(transform.position + Vector3.up / 20, -Vector3.forward, Vector3.up, 360, 0.33f);
        Handles.color = Color.white;
    }
#endif
}
