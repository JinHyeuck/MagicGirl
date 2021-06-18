/// <copyright file="Coroutiner.cs">Copyright (c) 2015 All Rights Reserved</copyright>
/// <author>Joris van Leeuwen</author>
/// <date>01/27/2014</date>

using UnityEngine;
using System.Collections;


namespace CodeControl.Internal
{
	public class Coroutiner : MonoBehaviour
	{
		private static Coroutiner instance;


		public static Coroutine Start(IEnumerator routine)
		{
			return GetInstance().StartLocalCoroutine(routine);
		}
		
		public Coroutine StartLocalCoroutine(IEnumerator routine)
		{
			return StartCoroutine(routine);
		}

		private static Coroutiner GetInstance()
		{
			if (instance == null)
			{
				GameObject go = new GameObject("Coroutiner");
				DontDestroyOnLoad(go);

				instance = go.AddComponent<Coroutiner>();
			}
			return instance;
		}
	}
}
