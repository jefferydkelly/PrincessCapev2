using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MapTile {
    [SerializeField]
    GameObject reflectSurface;
    public override void Rotate(float ang)
    {
        if (Mathf.Abs(reflectSurface.transform.rotation.z + ang) >= 90) {
            reflectSurface.transform.rotation *= Quaternion.AngleAxis(ang, Vector3.forward);
        }
    }

    public override bool Overlaps(Vector3 pos)
    {
		Vector3 dif = pos - transform.position;
		Vector3 bounds = Bounds / 2;
		return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
    }

    public override Vector3 Bounds
    {
        get
        {
            return Vector2.one + Vector2.up / 2;
        }
    }
}
