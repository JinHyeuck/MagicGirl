/// <copyright file="ModelRefs.cs">Copyright (c) 2015 All Rights Reserved</copyright>
/// <author>Joris van Leeuwen</author>
/// <date>01/27/2014</date>

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeControl.Internal;


/// <summary>
/// A list of references to models. Automatically removes models from the list when they are manually deleted.
/// </summary>
/// <typeparam name="T">The Model type that will be referenced</typeparam>
[Serializable]
public sealed class ModelRefs<T> : ModelReferencer, IEnumerable<T> where T : Model
{
	public T this[int i]
	{
		get { return _models[i]; }
		set { _models[i] = value; }
	}

	/// <summary>
	/// The number of instances in the list.
	/// </summary>
	public int Count { get { return _models.Count; } }

	/// <summary>
	/// The last model instance in the list. Returns null if the list is empty. 
	/// </summary>
	public T Last { get { return _models.Count == 0 ? null : _models[_models.Count - 1]; } }

	private List<T> _models;


	public ModelRefs()
	{
		_models = new List<T>();
	}

	/// <summary>
	/// Clears and removes the references to the models, potentially deleting models if they have no ModelRef(s) left referencing them.
	/// </summary>
	public void Clear()
	{
		for (int i = _models.Count - 1; i >= 0; i--)
		{
			_models[i].RemoveDeleteListener(OnModelDelete);
			_models[i].DecreaseRefCount();
		}
		_models.Clear();
	}

	/// <summary>
	/// Checks if the list of references contains a reference to the given model.
	/// </summary>
	/// <param name="model">The model that will be checked.</param>
	/// <returns>True if a reference to the given model is found.</returns>
	public bool Contains(T model)
	{
		return _models.Contains(model);
	}

	public T Find(Predicate<T> match)
	{
		return _models.Find(match);
	}

	/// <summary>
	/// Adds a model to the list of references.
	/// </summary>
	/// <param name="model">The model that is added to the list of references.</param>
	public void Add(T model)
	{
		_models.Add(model);
		model.IncreaseRefCount();
		model.AddDeleteListener(OnModelDelete);
	}

	/// <summary>
	/// Removes the reference to the given model from the list of references.
	/// </summary>
	/// <param name="model">The model which reference will be removed from the list.</param>
	public void Remove(T model)
	{
		model.RemoveDeleteListener(OnModelDelete);
		model.DecreaseRefCount();
		_models.Remove(model);
	}

	/// <summary>
	/// Removes the reference to the model at the given index, potentially deleting the model if it has no ModelRef(s) left referencing it.
	/// </summary>
	/// <param name="index">The index at which the model reference will be removed.</param>
	public void RemoveAt(int index)
	{
		if (index >= _models.Count)
		{
			Debug.LogError("Can't remove because '" + index + "' is out of index");
			return;
		}
		_models[index].RemoveDeleteListener(OnModelDelete);
		_models[index].DecreaseRefCount();
		_models.RemoveAt(index);
	}

	/// <summary>
	/// Clears and removes the references to the models, potentially deleting models if they have no ModelRef(s) left referencing them.
	/// </summary>
	public override void Delete()
	{
		Clear();
	}

	public IEnumerator<T> GetEnumerator()
	{
		for (int i = 0; i < _models.Count; i++)
		{
			yield return _models[i];
		}
	}

	private void OnModelDelete(Model model)
	{
		_models.Remove((T)model);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
