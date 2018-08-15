using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour
{

    [SerializeField]
    GameObject connectionLink;
    ConnectionStruct connection;
    SpriteRenderer myRenderer;
    bool deleteConnection;

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

            LevelEditor.Instance.OnTileDestroyed.AddListener((MapTile mt) =>
            {
                deleteConnection = (mt == connection.ActivatedTile || mt == connection.ActivatorTile);
            });

            LevelEditor.Instance.OnConnectionRemoved.AddListener((ActivatorConnection ac) =>
            {
                deleteConnection = (ac.Activated == connection.ActivatedTile) && (ac.Activator == connection.ActivatorTile);
            });

            LevelEditor.Instance.OnConnectionInverted.AddListener((int vator, int vated, bool flipped) => {

                if (vator == connection.Activator && vated == connection.Activated)
                {
                    Color = flipped ? Color.red : Color.blue;
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
        Color = connection.Inverted ? Color.red : Color.blue;
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

    public bool ToBeDeleted {
        get {
            return deleteConnection;
        }
    }

    Color Color {
        set {
            foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
                sr.color = value;
            }
        }
    }
}