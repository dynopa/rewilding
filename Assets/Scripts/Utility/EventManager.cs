using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager {
	
	private Dictionary<Type, AGPEvent.Handler> _registeredHandlers = new Dictionary<Type, AGPEvent.Handler>();

	public void Register<T>(AGPEvent.Handler handler) where T : AGPEvent 
	{
		var type = typeof(T);
		if (_registeredHandlers.ContainsKey(type)) 
		{
			if (!IsEventHandlerRegistered(type, handler))
				_registeredHandlers[type] += handler;         
		} 
		else 
		{
			_registeredHandlers.Add(type, handler);         
		}     
	} 

	public void Unregister<T>(AGPEvent.Handler handler) where T : AGPEvent 
	{         
		var type = typeof(T);
		if (!_registeredHandlers.TryGetValue(type, out var handlers)) return;
		
		handlers -= handler;  
		
		if (handlers == null) 
		{                 
			_registeredHandlers.Remove(type);             
		} 
		else
		{
			_registeredHandlers[type] = handlers;             
		}
	}      
		
	public void Fire(AGPEvent e) 
	{       
		var type = e.GetType();

		if (_registeredHandlers.TryGetValue(type, out var handlers)) 
		{             
			handlers(e);
		}     
	} 

	public bool IsEventHandlerRegistered (Type typeIn, Delegate prospectiveHandler)
	{
		return _registeredHandlers[typeIn].GetInvocationList().Any(existingHandler => existingHandler == prospectiveHandler);
	}
}

public abstract class AGPEvent 
{
	public readonly float creationTime;

	public AGPEvent ()
	{
		creationTime = Time.time;
	}

	public delegate void Handler (AGPEvent e);
}

public class PlantDestroyed : AGPEvent
{
	public Plant plant;

	public PlantDestroyed(Plant plant){
		this.plant = plant;
	}
}
public class PlantJustFed : AGPEvent
{
	public Plant plant;
	public PlantJustFed(Plant plant){
		this.plant = plant;
	}
}

public class PlantJustUnFed : AGPEvent
{
    public Plant plant;
    public PlantJustUnFed(Plant plant)
    {
        this.plant = plant;
    }
}

public class PlantCreated : AGPEvent
{
    public Plant plant;
	public PlantCreated(Plant plant){
		this.plant = plant;
	}
}
public class PlantGrown : AGPEvent{
	public Plant plant;
	public PlantGrown(Plant plant){
		this.plant = plant;
	}
}
public class FadeOutComplete : AGPEvent{}
public class GameStart : AGPEvent{}
public class After30Seconds : AGPEvent{}
public class Day2 : AGPEvent{}
public class FirstTreePlanted : AGPEvent{}
public class FirstTreeGrown : AGPEvent{}
public class TooManyPlants : AGPEvent{}
public class RunOutOfPlants : AGPEvent{}
public class FirstSleep : AGPEvent { }
public class TestEvent : AGPEvent { }