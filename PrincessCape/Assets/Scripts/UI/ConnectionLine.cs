using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour
{

    [SerializeField]
    GameObject connectionLink;
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
       //(activator.Center + dif / 2).SetZ(-1);
        //transform.localScale = new Vector3(dif.magnitude, 1, 1);
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dif.y, dif.x).ToDegrees(), Vector3.forward);
        transform.position = activator.Center.SetZ(-1);
        SetLength(dif.magnitude);
    }

    void SetLength(float length) {
        if (length < transform.childCount)
        {
            int numChildren = transform.childCount + 1;

            while (length < numChildren)
            {
                Destroy(transform.GetChild(transform.childCount - 1).gameObject);
                numChildren--;
            }
        }
        else
        {

            while (transform.childCount + 1 < length)
            {
                GameObject link = Instantiate(connectionLink);
                link.transform.SetParent(transform);
                link.transform.localPosition = Vector3.right * transform.childCount;
                link.transform.localRotation = Quaternion.identity;
            }
        }

    }
}