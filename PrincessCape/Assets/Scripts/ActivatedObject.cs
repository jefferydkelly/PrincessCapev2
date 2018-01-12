using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class ActivatedObject : MapTile {

	[SerializeField]
	protected List<ActivatedObject> connections = new List<ActivatedObject>();
	protected bool isActivated = false;
	
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:ActivatedObject"/> is activated.
    /// </summary>
    /// <value><c>true</c> if is activated; otherwise, <c>false</c>.</value>
    public virtual bool IsActivated {
        get {
            return isActivated;
        }

        protected set {
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
}
