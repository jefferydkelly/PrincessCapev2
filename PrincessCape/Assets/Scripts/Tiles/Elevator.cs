using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Elevator : MovingPlatform
{
	bool up = true;
	Vector3 startLocation;
	int ticksPerSec = 60;
    int fwd = 1;
	public override void Init()
	{
		base.Init();
		startLocation = transform.position;
        //direction = Vector3.up;
        minimumDistance = 1.0f;
       
        moveTimer = new Timer(1.0f / (float)ticksPerSec, (int)(travelTime * ticksPerSec));
      
		moveTimer.OnTick.AddListener(() =>
		{
            transform.position += direction * fwd * travelDistance / (travelTime * ticksPerSec);
            Vector3 dif = transform.position - startLocation;
            float dot = Vector3.Dot(dif, direction) / travelDistance;

            if (dot > 1 || dot < 0) {
                moveTimer.Stop();
                transform.position = startLocation + direction * travelDistance * Mathf.Clamp01(dot);
                fwd *= -1;
            }
		});

    }
}
