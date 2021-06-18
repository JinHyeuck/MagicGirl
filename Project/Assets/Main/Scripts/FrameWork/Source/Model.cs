/// <copyright file="Model.cs">Copyright (c) 2015 All Rights Reserved</copyright>
/// <author>Joris van Leeuwen</author>
/// <date>01/27/2014</date>

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using CodeControl.Internal;
using System.Collections;


#if !CUSTOM_DEBUG
using Debug = UnityEngine.Debug;
#endif


public class ModelChangeNotifiedMessage : Message
{
}

public class ModelChangeHandledMessage : Message
{
}

public class ModelDeletedMessage : Message
{
}

public class ModelDeleteHandledMessage : Message
{
}


/// <copyright file="Model.cs">Copyright (c) 2015 All Rights Reserved</copyright>
/// <author>Joris van Leeuwen</author>
/// <date>01/27/2014</date>
/// <summary>A Model is a representation of data, which can be saved and loaded into/from different data types. It will delete itself automatically when there is no ModelRef(s) left referencing it. Extend the Model into a custom class to add data fields that can be saved/loaded. Use Models in combination with Controllers for max use.</summary>
[Serializable]
public abstract class Model : ModelReferencer
{
	/// <summary>
	/// Called when the NotifyChange is called on a Model. Only works in Editor.
	/// </summary>
	/// <param name="affecterType">The type that called the NotifyChange on the Model.</param>
	/// <param name="modelType">The type of Model on which the NotifyChange is called.</param>
    public delegate void OnModelAffectDelegate(Type affecterType,Type modelType);

	/// <summary>Called when a model change is notified. Only works when in the Unity editor.</summary>
	public static OnModelAffectDelegate OnModelChangeNotified;

	/// <summary>Called when a model change is notified. Only works when in the Unity editor.</summary>
	public static OnModelAffectDelegate OnModelChangeHandled;

	/// <summary>Called when a model is deleted. Only works when in the Unity editor.</summary>
	public static OnModelAffectDelegate OnModelDeleted;

	/// <summary>Called when a model deletion is handled. Only works when in the Unity editor.</summary>
	public static OnModelAffectDelegate OnModelDeleteHandled;

	/// <summary>
	/// The id of this model, which can be used to find it later on.
	/// </summary>
	public string ID
	{
		get
		{
			return _id;
		}

		private set
		{
			Unregister();
			_id = value;
			Register();
		}
	}

	public Dictionary<string, Action> successMap = new Dictionary<string, Action>();
	protected bool _sendPacket;
	private static Dictionary<string, Model> _sortedInstances = new Dictionary<string, Model>();
	private static Dictionary<Type, List<Model>> _typeSortedInstances = new Dictionary<Type, List<Model>>();
	private static List<Model> _instances = new List<Model>();

	private string _id;
	private bool _isRegistered;
	private int _refCount;

	private List<Delegate> _onChangeHandlers;
	private List<Delegate> _onDeleteHandlers;

	private bool _reservedNotifyChange = false;


#region Create / Delete
	protected Model()
	{
		ID = Guid.NewGuid().ToString();
		_refCount = 0;
		_onChangeHandlers = new List<Delegate>();
		_onDeleteHandlers = new List<Delegate>();
	}

	protected Model(long guid)
	{
		ID = string.Format("S_{0}_{1}", GetType().Name, guid.ToString());
		_refCount = 0;
		_onChangeHandlers = new List<Delegate>();
		_onDeleteHandlers = new List<Delegate>();
	}

	/// <summary>
	/// Finds and returns the model with the given id.
	/// </summary>
	/// <param name="id">The id used to find the model.</param>
	/// <returns>The model found with the given id.</returns>
	static Model Find(string id)
	{
		if (!_sortedInstances.ContainsKey(id))
		{
			Debug.LogError("Could not find model with id '" + id + "'");
			return null;
		}

		return _sortedInstances[id];
	}

	/// <summary>
	/// Finds and returns the model of given type with the given id.
	/// </summary>
	/// <typeparam name="T">The type used to find the model.</typeparam>
	/// <param name="id">The id used to find the model.</param>
	/// <returns>The model found with the given type and id.</returns>
	public static T Find<T>(string id) where T : Model
	{
		var model = Find(id) as T;
		if (model == null)
		{
			Debug.LogError("Could not find model with id '" + id + "' and type '" + typeof(T) + "'");
			return null;
		}

		return model;
	}

	/// <summary>
	/// Finds and returns the model of given type with the given id.
	/// </summary>
	/// <typeparam name="T">The type used to find the model.</typeparam>
	/// <param name="guid">The id used to find the model.</param>
	/// <returns>The model found with the given type and id.</returns>
	public static T Find<T>(long guid) where T : Model
	{
		var stringID = string.Format("S_{0}_{1}", typeof(T).Name, guid.ToString());
		var model = Find(stringID) as T;
		if (model == null)
		{
			Debug.LogError("Could not find model with id '" + guid + "' and type '" + typeof(T) + "'");
			return null;
		}

		return model;
	}

	/// <summary>
	/// Finds and returns the model of given type with the given id.
	/// </summary>
	/// <typeparam name="T">The type used to find the model.</typeparam>
	/// <param name="index">The index used to find the model.</param>
	/// <returns>The model found with the given type and id.</returns>
	public static T Find<T>(int index) where T : Model
	{
		Type type = typeof(T);
		List<Model> instances;
		if (!_typeSortedInstances.TryGetValue(type, out instances))
		{
			Debug.LogError("Could not find model with index '" + index + "' and type '" + typeof(T) + "'");
			return null;
		}

		if (instances == null || instances.Count >= index)
		{
			Debug.LogError("Could not find model with index '" + index + "' and type '" + typeof(T) + "'");
			return null;
		}
		
		var model = instances[index] as T;
		if (model == null)
		{
			Debug.LogError("Could not find model with index '" + index + "' and type '" + typeof(T) + "'");
			return null;
		}

		return model;
	}

	/// <summary>
	/// Finds and returns an instance of the given model type.
	/// </summary>
	/// <typeparam name="T">The type used to find the instance.</typeparam>
	/// <returns>The model found with the given type.</returns>
	public static T First<T>() where T : Model
	{
		Type type = typeof(T);
		List<Model> instances;
		if (!_typeSortedInstances.TryGetValue(type, out instances))
			return null;

		if (instances == null || instances.Count == 0)
			return null;

		return instances[0] as T;
	}

	/// <summary>
	/// Finds and returns all models of the given type.
	/// </summary>
	/// <typeparam name="T">The type used to find the models.</typeparam>
	/// <returns>All models of given type.</returns>
	public static List<T> GetAll<T>() where T : Model
	{
		var models = new List<T>();

		Type type = typeof(T);
		List<Model> instances;
		if (!_typeSortedInstances.TryGetValue(type, out instances))
			return models;	// empty list.

		if (instances == null)
			return models;	// empty list.

		for (int i = 0; i < instances.Count; ++i)
		{
			models.Add((T)instances[i]);
		}

		return models;
	}

	/// <summary>
	/// Deletes all models.
	/// </summary>
	public static void DeleteAll()
	{
		while (_instances.Count > 0)
		{
			_instances[0].Delete();
		}
	}

	/// <summary>
	/// Deletes all models of given type.
	/// </summary>
	/// <typeparam name="T">The type used to find and delete the models.</typeparam>
	public static void DeleteAll<T>() where T : Model
	{
		List<T> models = GetAll<T>();
		while (models.Count > 0)
		{
			if (_sortedInstances.ContainsKey(models[0].ID))
			{
				models[0].Delete();
			}
			models.RemoveAt(0);
		}
	}
#endregion

#region Messages
	/// <summary>
	/// Adds a listener that triggers the given callback when the NotifyChange is called on this Model.
	/// </summary>
	/// <param name="callback">The callback that will be triggered when NotifyChange is called.</param>
	public void AddChangeListener(Action callback)
	{
		if (callback == null)
		{
			Debug.LogError("Failed to add ChangeListener on Model but the given callback is null!");
			return;
		}

		_onChangeHandlers.Add(callback);
	}

	/// <summary>
	/// Adds a listener that triggers the given callback when the NotifyChange is called on this Model.
	/// </summary>
	/// <param name="callback">The callback that will be triggered when NotifyChange is called.</param>
	public void AddChangeListener(Action<Model> callback)
	{
		if (callback == null)
		{
			Debug.LogError("Failed to add ChangeListener on Model but the given callback is null!");
			return;
		}

		_onChangeHandlers.Add(callback);
	}

	/// <summary>
	/// Removes a listener that would trigger the given callback when the NotifyChange is called on this Model.
	/// </summary>
	/// <param name="callback">The callback that is triggered when the NotifyChange is called.</param>
	public void RemoveChangeListener(Action callback)
	{
		_onChangeHandlers.Remove(callback);
	}

	/// <summary>
	/// Removes a listener that triggers the given callback when the NotifyChange is called on this Model
	/// </summary>
	/// <param name="callback">The callback that is triggered when the NotifyChange is called.</param>
	public void RemoveChangeListener(Action<Model> callback)
	{
		_onChangeHandlers.Remove(callback);
	}

	/// <summary>
	/// Adds a listener that triggers the given callback when this Model is deleted.
	/// </summary>
	/// <param name="callback">The callback that will be triggered when NotifyChange is called.</param>
	public void AddDeleteListener(Action callback)
	{
		if (callback == null)
		{
			Debug.LogError("Failed to add DeleteListener on Model but the given callback is null!");
			return;
		}

		_onDeleteHandlers.Add(callback);
	}

	/// <summary>
	/// Adds a listener that triggers the given callback when this Model is deleted.
	/// </summary>
	/// <param name="callback">The callback that will be triggered when NotifyChange is called.</param>
	public void AddDeleteListener(Action<Model> callback)
	{
		if (callback == null)
		{
			Debug.LogError("Failed to add DeleteListener on Model but the given callback is null!");
			return;
		}

		_onDeleteHandlers.Add(callback);
	}

	/// <summary>
	/// Removes a listener that triggers the given callback when this Model is deleted.
	/// </summary>
	/// <param name="callback">The callback that triggers when NotifyChange is called.</param>
	public void RemoveDeleteListener(Action callback)
	{
		_onDeleteHandlers.Remove(callback);
	}

	/// <summary>
	/// Removes a listener that triggers the given callback when this Model is deleted.
	/// </summary>
	/// <param name="callback">The callback that triggers when NotifyChange is called.</param>
	public void RemoveDeleteListener(Action<Model> callback)
	{
		_onDeleteHandlers.Remove(callback);
	}

	/// <summary>
	/// Sends out callbacks to this Model's change listeners.
	/// </summary>
	public virtual void NotifyChange(bool immediate = true)
	{
		if (immediate)
		{
			NotifyChangeImmediate(null);
		}
		else if (!_reservedNotifyChange)
		{
			Type notifierType = null;
			if (Application.isEditor && OnModelChangeNotified != null)
			{
				var stackTrace = new StackTrace();
				notifierType = stackTrace.GetFrame(1).GetMethod().DeclaringType;
			}
			
			Coroutiner.Start(NotifyChangeReserve(notifierType));
		}
	}

	void NotifyChangeImmediate(Type notifierType)
	{
		if (Application.isEditor && OnModelChangeNotified != null)
		{
			if (notifierType == null)
			{
				var stackTrace = new StackTrace();
				notifierType = stackTrace.GetFrame(2).GetMethod().DeclaringType;
			}

			OnModelChangeNotified(notifierType, GetType());
			Message.ShowMessageFlow(notifierType, GetType(), typeof(ModelChangeNotifiedMessage), _id);
		}

		var callbacks = new List<Delegate>(_onChangeHandlers);
		while (callbacks.Count > 0)
		{
			Delegate callback = callbacks[0];

			if (Application.isEditor && OnModelChangeHandled != null)
			{
				OnModelChangeHandled(callback.Target.GetType(), GetType());
				Message.ShowMessageFlow(GetType(), callback.Target.GetType(), typeof(ModelChangeHandledMessage), _id);
			}

			CallbackModelDelegate(callback);
			callbacks.Remove(callback);
		}
	}

	IEnumerator NotifyChangeReserve(Type notifierType)
	{
		_reservedNotifyChange = true;

		yield return new WaitForEndOfFrame();
		NotifyChangeImmediate(notifierType);

		_reservedNotifyChange = false;
	}
#endregion


	/// <summary>
	/// Deletes this Model, removing it from ModelRefs lists and destroying its linked Controllers.
	/// </summary>
	public override void Delete()
	{
		if (!_sortedInstances.ContainsKey(_id))
			return;

		Unregister();

		if (Application.isEditor && OnModelDeleted != null)
		{
			var stackTrace = new StackTrace();
			var deleterType = stackTrace.GetFrame(1).GetMethod().DeclaringType;
			if (deleterType == typeof(Model))
				deleterType = stackTrace.GetFrame(2).GetMethod().DeclaringType;
			
			var myType = GetType();

			OnModelDeleted(deleterType, myType);
			Message.ShowMessageFlow(deleterType, myType, typeof(ModelDeletedMessage), _id);
		}

		while (_onDeleteHandlers.Count > 0)
		{
			Delegate callback = _onDeleteHandlers[0];

			if (Application.isEditor && OnModelDeleteHandled != null)
			{
				OnModelDeleteHandled(callback.Target.GetType(), GetType());
				Message.ShowMessageFlow(GetType(), callback.Target.GetType(), typeof(ModelDeleteHandledMessage), _id);
			}

			CallbackModelDelegate(callback);
			_onDeleteHandlers.Remove(callback);
		}

		ModelReferencer referencer;
		List<ModelReferencer> modelReferencers = GetModelReferencersInFields();
		for (int i = 0; i < modelReferencers.Count; ++i)
		{
			referencer = modelReferencers[i];
			if (referencer == null)
				continue;

			if (Application.isEditor && OnModelDeleted != null)
			{
				var deleterType = GetType();
				var myType = referencer.GetType();

				OnModelDeleted(deleterType, myType);
				Message.ShowMessageFlow(deleterType, myType, typeof(ModelDeletedMessage), _id);
			}
			
			referencer.Delete();
		}
	}

	internal void IncreaseRefCount()
	{
		_refCount++;
	}

	internal void DecreaseRefCount()
	{
		_refCount--;
		if (_refCount <= 0)
		{
			Delete();
		}
	}

	private List<ModelReferencer> GetModelReferencersInFields()
	{
		var modelReferencers = new List<ModelReferencer>();

		FieldInfo field;
		FieldInfo[] fields = GetType().GetFields();
		for (int i = 0; i < fields.Length; ++i)
		{
			field = fields[i];

			if (field.GetValue(this) is ModelReferencer)
			{
				modelReferencers.Add(field.GetValue(this) as ModelReferencer);
			}
		}

		return modelReferencers;
	}

	private void Register()
	{
		if (_isRegistered)
			return;
		
		_isRegistered = true;

		_sortedInstances.Add(_id, this);
        
		if (!_typeSortedInstances.ContainsKey(GetType()))
		{
			_typeSortedInstances.Add(GetType(), new List<Model>());
		}
		_typeSortedInstances[GetType()].Add(this);

		_instances.Add(this);

		//Debug.LogFormat("Register model - Type: {0}, ID: {1}", GetType(), ID);
	}

	private void Unregister()
	{
		if (!_isRegistered)
			return;

		//Debug.LogFormat("Unregister model - Type: {0}, ID: {1}", GetType(), ID);
		
		_isRegistered = false;

		if (_sortedInstances.ContainsValue(this))
		{
			foreach (KeyValuePair<string, Model> pair in _sortedInstances)
			{
				if (pair.Value == this)
				{
					_sortedInstances.Remove(pair.Key);
					break;
				}
			}
		}

		if (_typeSortedInstances.ContainsKey(GetType()))
		{
			_typeSortedInstances[GetType()].Remove(this);
		}

		_instances.Remove(this);
		if (!string.IsNullOrEmpty(_id))
		{
			_sortedInstances.Remove(_id);
		}
		_instances.Remove(this);
	}

	private void CallbackModelDelegate(Delegate callback)
	{
		if (callback is Action<Model>)
		{
			Action<Model> action = callback as Action<Model>;
			action(this);
		}
		else
		{
			Action action = callback as Action;
			action();
		}
	}

	public void AddPacketSuccessCallback(string packetName, Action onSuccess)
	{
		if (!successMap.ContainsKey(packetName) && onSuccess != null)
			successMap.Add(packetName, onSuccess);
	}

	public void ActionPacketSuccessCallback(string packetName)
	{
		if (successMap.ContainsKey(packetName))
		{
			successMap[packetName].Invoke();
			successMap.Remove(packetName);
		}
	}

	public void ActionPacketSuccessCallbackNoRemove(string packetName)
	{
		if (successMap.ContainsKey(packetName))
		{
			successMap[packetName].Invoke();
		}
	}
}
