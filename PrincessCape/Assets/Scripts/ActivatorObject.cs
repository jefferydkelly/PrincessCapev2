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
		onActivate = new UnityEvent();
        onDeactivate = new UnityEvent();
		base.Init();


		if (connections == null)
		{
			connections = new List<ActivatorConnection>();
		}
	}
	/// <summary>
	/// Activate this instance.
	/// </summary>
	public override void Activate()
    {
		onActivate.Invoke();
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public override void Deactivate()
    {
		onDeactivate.Invoke();
    }

	protected int NumConnections
	{
		get
		{
			return Connections.Count;
		}
	}
    
	public UnityEvent OnActivate {
		get {
			return onActivate;
		}
	}

	public UnityEvent OnDeactivate {
		get {
			return onDeactivate;
		}
	}

	public void AddConnection(ActivatedObject ao, bool inverted = false)
	{
		if (ao && !HasConnection(ao))
		{
			ao.StartsActive = startActive;
			ao.IsConnected = true;
			if (Application.isPlaying)
			{
				
				if (inverted)
				{
					OnActivate.AddListener(ao.DecrementActivator);
					OnDeactivate.AddListener(ao.IncrementActivator);
				}
				else
				{
					OnActivate.AddListener(ao.IncrementActivator);
					OnDeactivate.AddListener(ao.DecrementActivator);
				}
			}
			else
			{
				ActivatorConnection akon = new ActivatorConnection(this, ao, inverted);
				Connections.Add(akon);
			}

            
		}
	}

	public bool HasConnection(ActivatedObject ao) {
		foreach(ActivatorConnection akon in Connections) {
			if (akon.Activated.ID == ao.ID) {
				return true;
			}
		}
        
		return false;
	}
    
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
			Connections.RemoveAt(index);
		}
	}

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

public class ActivatorConnection
{
	ActivatorObject activator;
	ActivatedObject activated;
	bool inverted;

	public ActivatorConnection(ActivatorObject tor, ActivatedObject ted, bool invert = false)
	{
		activator = tor;
		activated = ted;
		inverted = invert;

		if (!inverted)
		{
			activated.StartsActive = activator.StartsActive;
		}
		else
		{
			activated.StartsActive = !activator.StartsActive;
		}
	}

	public ActivatedObject Activated
	{
		get
		{
			return activated;
		}
	}

	public ActivatorObject Activator {
		get {
			return activator;
		}
	}

	public bool IsInverted {
		get {
			return inverted;
		}

		set {
			inverted = value;

			if (!inverted)
            {
                activated.StartsActive = activator.StartsActive;
            }
            else
            {
                activated.StartsActive = !activator.StartsActive;
            }
		}
	}

	public void Update() {
		if (!inverted)
        {
            activated.StartsActive = activator.StartsActive;
        }
        else
        {
            activated.StartsActive = !activator.StartsActive;
        }
	}

	public string GenerateSaveData() {
		string data = PCLParser.StructStart;
		data += PCLParser.CreateAttribute<int>("Activator", activator.ID);
		data += PCLParser.CreateAttribute<int>("Activated", activated.ID);
		data += PCLParser.CreateAttribute<bool>("Inverted", inverted);
		data += PCLParser.StructEnd;
		return data;
	}
#if UNITY_EDITOR
	public void RenderInEditor()
	{
		Handles.color = inverted ? Color.red : Color.green;
		Handles.DrawDottedLine(activator.Center, activated.Center, 8.0f);
	}
#endif
}
