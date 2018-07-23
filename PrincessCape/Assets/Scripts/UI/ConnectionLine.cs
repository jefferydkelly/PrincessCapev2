using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour
{

    ConnectionStruct connection;

    public ConnectionStruct Connection
    {
        get
        {
            return connection;
        }

        set
        {
            connection = value;
            UpdateConnection();
            LevelEditor.Instance.OnTileMoved.AddListener((MapTile mt) =>
            {
                if (mt == connection.ActivatedTile || mt == connection.ActivatorTile) {
                    UpdateConnection();
                }
            });
        }
    }

    void UpdateConnection()
    {
        MapTile activator = connection.ActivatorTile;
        MapTile activated = connection.ActivatedTile;
        Vector3 dif = (activated.Center - activator.Center);
        transform.position = (activator.Center + dif / 2).SetZ(-1);
        transform.localScale = new Vector3(dif.magnitude, 1, 1);
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dif.y, dif.x).ToDegrees(), Vector3.forward);
    }
}