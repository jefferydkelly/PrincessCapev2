using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class ActivatedObject : MapTile
{

    [SerializeField]
    protected List<ActivatedObject> connections = new List<ActivatedObject>();
    protected bool isActivated = false;
    List<int> connectionIDs;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:ActivatedObject"/> is activated.
    /// </summary>
    /// <value><c>true</c> if is activated; otherwise, <c>false</c>.</value>
    public virtual bool IsActivated
    {
        get
        {
            return isActivated;
        }

        protected set
        {
            isActivated = value;
        }
    }

    /// <summary>
    /// Activate this instance.
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public abstract void Deactivate();

    /// <summary>
    /// Adds a connection to another <see cref="T:ActivatedObject"/> if they are not already connected
    /// </summary>
    /// <param name="ao">Ao.</param>
    public void AddConnection(ActivatedObject ao)
    {
        if (!HasConnection(ao))
        {
            connections.Add(ao);
            ao.AddConnection(this);
        }
    }

    /// <summary>
    /// Removes a connection to another <see cref="T:ActivatedObject"/> if they are connected
    /// </summary>
    /// <param name="ao">Ao.</param>
    public void RemoveConnection(ActivatedObject ao)
    {
        if (HasConnection(ao))
        {

            connections.Remove(ao);
            ao.RemoveConnection(this);
        }
    }

    /// <summary>
    /// Gets the connections.
    /// </summary>
    /// <value>The connections.</value>
    public List<ActivatedObject> Connections
    {
        get
        {
            return connections;
        }
    }

    /// <summary>
    /// Whether or not this ActivatedObject is connected to the other.
    /// </summary>
    /// <returns><c>true</c>, If this is connected to ao, <c>false</c> otherwise.</returns>
    /// <param name="ao">Ao.</param>
    public bool HasConnection(ActivatedObject ao)
    {
        return connections.Contains(ao);
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
    }

    public override string SaveData()
    {
        if (transform.parent.name != "Map")
        {
            return "";
        }
        string info = "{\n";
        info += string.Format("\"Name\": \"{0}\"", name.Split('(')[0]) + lineEnding;
        info += string.Format("\"ID\": \"{0}\"", ID) + lineEnding;
        info += string.Format("\"Position\": \"{0}\"", transform.position) + lineEnding;
        info += string.Format("\"Rotation\": \"{0}\"", transform.rotation) + lineEnding;
        info += string.Format("\"Scale\": \"{0}\"", transform.localScale) + lineEnding;
        info += string.Format("\"Connections\": [\n");
        foreach (ActivatedObject ao in connections)
        {
            if (ao)
            {
                info += string.Format("\"CI\": \"{0}\"", ao.ID) + lineEnding;
            }

        }
        info += "]" + lineEnding;
        info += "}" + lineEnding;
        return info;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        connectionIDs = new List<int>();

        if (tile.info[3].Contains("Connections"))
        {
            for (int i = 4; i < tile.info.Count && tile.info[i] != "],"; i++)
            {
                connectionIDs.Add(PCLParser.ParseInt(tile.info[i]));
            }
        }
    }

    public void Reconnect()
    {
        Map map = GetComponentInParent<Map>();
        foreach (int i in connectionIDs)
        {
            AddConnection(map.GetTileByID(i) as ActivatedObject);
        }
    }
#endif
}
