/// <copyright file="ModelRef.cs">Copyright (c) 2015 All Rights Reserved</copyright>
/// <author>Joris van Leeuwen</author>
/// <date>01/27/2014</date>

using System;
using System.Collections.Generic;
using CodeControl.Internal;


/// <summary>
/// A reference to a single model.
/// </summary>
/// <typeparam name="T">The model type that will be referenced</typeparam>
[Serializable]
public sealed class ModelRef<T> : ModelReferencer where T : Model
{
	/// <summary>
	/// The referenced model. Changing this will potentially delete the old model if it has no ModelRef(s) left referencing it.
	/// </summary>
	public T Model
	{
		get
		{
			return model;
		}

		set
		{
			if (model == value)
				return;
			
			if (model != null)
			{
				model.RemoveDeleteListener(OnModelDelete);
				model.DecreaseRefCount();
			}

			model = value;

			if (model != null)
			{
				model.IncreaseRefCount();
				model.AddDeleteListener(OnModelDelete);
			}
		}
	}

	private T model;


	public ModelRef()
	{
	}

	/// <summary>
	/// Creates a new instance of ModelRef, referencing the given model.
	/// </summary>
	/// <param name="model">The model that will be referenced.</param>
	public ModelRef(T model)
	{
		Model = model;
	}

	/// <summary>
	/// Removes the reference to the model, potentially deleting the model if it has no ModelRef(s) left referencing it.
	/// </summary>
	public override void Delete()
	{
		Model = null;
	}

	private void OnModelDelete()
	{
		model = null;
	}
}
