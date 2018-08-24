using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour
{

    [SerializeField]
    GameObject connectionLink;
    ActivatorConnection connection;
    SpriteRenderer myRenderer;
    bool deleteConnection;

    /// <summary>
    /// Gets or sets the connection this line represents.
    /// </summary>
    /// <value>The connection.</value>
    public ActivatorConnection Connection
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
                deleteConnection = (ac == Connection);
            });

            LevelEditor.Instance.OnConnectionInverted.AddListener((ActivatorConnection ac) =>
            {
                if (ac == Connection)
                {
                    Color = Connection.IsInverted ? Color.red : Color.blue;
                }
            });
        }
    }

    /// <summary>
    /// Updates the line based on the connection.
    /// </summary>
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
        Color = connection.IsInverted ? Color.red : Color.blue;
    }

    /// <summary>
    /// Sets the length of the connection.
    /// </summary>
    /// <param name="length">Length of the connection.</param>
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

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:ConnectionLine"/> to be deleted.
    /// </summary>
    /// <value><c>true</c> if to be deleted; otherwise, <c>false</c>.</value>
    public bool ToBeDeleted {
        get {
            return deleteConnection;
        }
    }

    /// <summary>
    /// Sets the color.
    /// </summary>
    /// <value>The color.</value>
    Color Color {
        set {
            foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
                sr.color = value;
            }
        }
    }

    /// <summary>
    /// Sets a value indicating whether this <see cref="T:ConnectionLine"/> is hidden.
    /// </summary>
    /// <value><c>true</c> if is hidden; otherwise, <c>false</c>.</value>
    public bool IsHidden {
        set {
            gameObject.SetActive(!value);
        }
    }
}