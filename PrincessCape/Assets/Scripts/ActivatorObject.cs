using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActivatorObject : ActivatedObject {
   
	UnityEvent onActivate;
	UnityEvent onDeactivate;
  
	List<ActivatorConnection> connections;

	public override void Init()
	{
		base.Init();


		if (connections == null)
		{
			connections = new List<ActivatorConnection>();
		}

        RegisterAllEvents();
	}
	/// <summary>
	/// Activate this instance.
	/// </summary>
	public override void Activate()
    {
        IsActivated = true;
		onActivate.Invoke();
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public override void Deactivate()
    {
        IsActivated = false;
		onDeactivate.Invoke();
    }

    /// <summary>
    /// Gets the number of connections.
    /// </summary>
    /// <value>The number connections.</value>
	protected int NumConnections
	{
		get
		{
			return Connections.Count;
		}
	}

    /// <summary>
    /// Gets the on activate event.
    /// </summary>
    /// <value>The on activate.</value>
	public UnityEvent OnActivate {
		get {
			return onActivate;
		}
	}

    /// <summary>
    /// Gets the on deactivate event.
    /// </summary>
    /// <value>The on deactivate.</value>
	public UnityEvent OnDeactivate {
		get {
			return onDeactivate;
		}
	}

    /// <summary>
    /// Creates a connection with the ActivatedObject
    /// </summary>
    /// <param name="ao">Ao.</param>
    /// <param name="inverted">If set to <c>true</c> inverted.</param>
	public void AddConnection(ActivatedObject ao, bool inverted = false)
	{
		if (ao && !HasConnection(ao))
		{
			ao.StartsActive = startActive;
			ao.IsConnected = true;
			
            if (Game.Instance.IsInLevelEditor)
			{
				ActivatorConnection akon = new ActivatorConnection(this, ao, inverted);
                RegisterEvent(akon);
				Connections.Add(akon);
			}

            
		}
	}

    public void AddConection(ActivatorConnection connection) {
        if (!connections.Contains(connection)) {
            RegisterEvent(connection);
            connections.Add(connection);
        }
    }

    /// <summary>
    /// Gets whether this Activator has a connection to the Activated object
    /// </summary>
    /// <returns><c>true</c>, if they are connected, <c>false</c> otherwise.</returns>
    /// <param name="ao">Ao.</param>
	public bool HasConnection(ActivatedObject ao) {
		foreach(ActivatorConnection akon in Connections) {
			if (akon.Activated.ID == ao.ID) {
				return true;
			}
		}

		return false;
	}

    /// <summary>
    /// Removes the connection.
    /// </summary>
    /// <param name="ao">Ao.</param>
	public void RemoveConnection(ActivatedObject ao) {
		int index = -1;

		for (int i = 0; i < Connections.Count; i++) {
			if (Connections[i].Activated == ao) {
				index = i;
				break;
			}
		}
        
		if (index > -1) {
			ao.IsConnected = false;
            ActivatorConnection activator = Connections[index];
            DeregisterConnection(activator);
            LevelEditor.Instance.OnConnectionRemoved.Invoke(activator);
            Connections.RemoveAt(index);
		}
	}

    void RegisterAllEvents() {
        onActivate = new UnityEvent();
        onDeactivate = new UnityEvent();

        foreach(ActivatorConnection ac in connections) {
            RegisterEvent(ac);
        }
    }

    void RegisterEvent(ActivatorConnection connection) {
        if (connection.IsInverted)
        {
            onActivate.AddListener(connection.Activated.DecrementActivator);
            onDeactivate.AddListener(connection.Activated.IncrementActivator);
        }
        else
        {
            onActivate.AddListener(connection.Activated.IncrementActivator);
            onDeactivate.AddListener(connection.Activated.DecrementActivator);
        }
    }

    void DeregisterConnection(ActivatorConnection connection) {
        if (connection.IsInverted) {
            onActivate.RemoveListener(connection.Activated.DecrementActivator);
            onDeactivate.RemoveListener(connection.Activated.IncrementActivator);
        }
        else
        {
            onActivate.RemoveListener(connection.Activated.IncrementActivator);
            onDeactivate.RemoveListener(connection.Activated.DecrementActivator);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:ActivatorObject"/> starts active.
    /// </summary>
    /// <value><c>true</c> if starts active; otherwise, <c>false</c>.</value>
	public override bool StartsActive
	{
		get
		{
			return base.StartsActive;
		}

		set
		{
			base.StartsActive = value;

			foreach (ActivatorConnection akon in Connections)
			{
				akon.Update();
			}
            
		}
	}

    /// <summary>
    /// Gets the connections.
    /// </summary>
    /// <value>The connections.</value>
	public List<ActivatorConnection> Connections {
		get {
			if (connections == null) {
				connections = new List<ActivatorConnection>();
			}
			return connections;
		}
	}

	#if UNITY_EDITOR
    /// <summary>
    /// Draws indicators of the connections between the Activated Object and its connections as well as the activation status of the object
    /// </summary>
    public override void RenderInEditor()
    {
		base.RenderInEditor();
		foreach(ActivatorConnection akon in Connections) {
			akon.RenderInEditor();
		}
    }

#endif
}

/// <summary>
/// For storing the connection between an ActivatorObject and an ActivatedObject
/// </summary>
public class ActivatorConnection
{
    int activatorID;
    int activatedID;
    bool inverted;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ActivatorConnection"/> class.
    /// </summary>
    /// <param name="tor">The ID of the ActivatorObject.</param>
    /// <param name="ted">The ID of the ActivatedObject.</param>
    /// <param name="inv">If set to <c>true</c> the conection is inverted.</param>
    public ActivatorConnection(int tor, int ted, bool inv)
    {
        activatorID = tor;
        activatedID = ted;
        inverted = inv;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ActivatorConnection"/> class.
    /// </summary>
    /// <param name="tor">The ActivatorObject.</param>
    /// <param name="ted">The ActivatedObject.</param>
    /// <param name="invert">If set to <c>true</c> the connection is inverted.</param>
    public ActivatorConnection(ActivatorObject tor, ActivatedObject ted, bool invert = false)
    {
        activatorID = tor.ID;
        activatedID = ted.ID;
        inverted = invert;

        if (!inverted)
        {
            ted.StartsActive = tor.StartsActive;
        }
        else
        {
            ted.StartsActive = !tor.StartsActive;
        }

    }

    /// <summary>
    /// Gets the ActivatorObject.
    /// </summary>
    /// <value>The ActivatorObject.</value>
    public ActivatorObject Activator {
        get {
            return ActivatorTile.GetComponent<ActivatorObject>();
        }
    }
    /// <summary>
    /// Gets the id of the activator object.
    /// </summary>
    /// <value>The activator.</value>
    public int ActivatorID
    {
        get
        {
            return activatorID;
        }
    }

    /// <summary>
    /// Gets the MapTile component of the ActivatorObject.
    /// </summary>
    /// <value>The MapTile component of the ActivatorObject.</value>
    public MapTile ActivatorTile
    {
        get
        {
            return Map.Instance.GetTileByID(activatorID);
        }
    }

    /// <summary>
    /// Gets the ActivatedObject.
    /// </summary>
    /// <value>The ActivatedObject.</value>
    public ActivatedObject Activated
    {
        get
        {
            return ActivatedTile.GetComponent<ActivatedObject>();
        }
    }

    /// <summary>
    /// Gets the id of the activated object.
    /// </summary>
    /// <value>The activated.</value>
    public int ActivatedID
    {
        get
        {
            return activatedID;
        }
    }

    /// <summary>
    /// Gets the MapTile component of the ActivatedObject.
    /// </summary>
    /// <value>The MapTile component of the ActivatedObject.</value>
    public MapTile ActivatedTile
    {
        get
        {
            return Map.Instance.GetTileByID(activatedID);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:ConnectionStruct"/> is inverted.
    /// </summary>
    /// <value><c>true</c> if inverted; otherwise, <c>false</c>.</value>
    public bool IsInverted
    {
        get
        {
            return inverted;
        }

        set {
            inverted = value;
            Update();
        }
    }

    /// <summary>
    /// Update this connection and whether or not the ActivatedObject starts activated based on the inversion state of the connection
    /// </summary>
    public void Update()
    {
        if (!inverted)
        {
            Activated.StartsActive = Activator.StartsActive;
        }
        else
        {
            Activated.StartsActive = !Activator.StartsActive;
        }
    }

    /// <summary>
    /// Generates the save data.
    /// </summary>
    /// <returns>The save data.</returns>
    public string GenerateSaveData()
    {
        string data = PCLParser.StructStart;
        data += PCLParser.CreateAttribute<int>("Activator", activatorID);
        data += PCLParser.CreateAttribute<int>("Activated", activatedID);
        data += PCLParser.CreateAttribute<bool>("Inverted", inverted);
        data += PCLParser.StructEnd;
        return data;
    }
#if UNITY_EDITOR
    /// <summary>
    /// Renders the in editor.
    /// </summary>
    public void RenderInEditor()
    {
        Handles.color = inverted ? Color.red : Color.green;
        Handles.DrawDottedLine(ActivatorTile.Center, ActivatedTile.Center, 8.0f);
    }
#endif
}
