// -------------------------------------------------------------------------------------------------
// Filename: Singleton.cs
// Author: Song Ji Hun. [aka. CraZy GolMae] <jihun.song@pocatcom.com>
// Date: 2015.04.23
//
// Desc:
//
// Copyright (c) 2015 Pocatcom. All rights reserved.
// -------------------------------------------------------------------------------------------------
using UnityEngine;


namespace GameBerry
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		static T _instance = null;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType(typeof(T)) as T;
					if (_instance == null)
					{
						Debug.LogFormat("Create Singleton-instance. - Begin - Type: {0}", typeof(T).FullName);

						var obj = new GameObject(typeof(T).ToString());
						_instance = obj.AddComponent<T>();	// 이 때 Awake() 호출됨

						Debug.LogFormat("Create Singleton-instance. - End - Type: {0}, InstanceID: {1}", typeof(T).FullName, _instance.GetInstanceID());

						// Problem during the creation, this should not happen
						if (_instance == null)
						{
							Debug.LogError("Problem during the creation of " + typeof(T).ToString());
						}
					}
					else
					{
						Debug.LogFormat("Find Singleton-instance. Type: {0}, InstanceID: {1}", typeof(T).FullName, _instance.GetInstanceID());
						_instance.Init();
					}
				}
				return _instance;
			}
		}

		public static bool isAlive { get { return (_instance != null); } }


		void Awake()
		{
			DontDestroyOnLoad(gameObject);

			if (_instance == null)
			{
				_instance = this as T;
				Debug.LogFormat("Awake Singleton-instance. - OK - Type: {0}, InstanceID: {1}", typeof(T).FullName, _instance.GetInstanceID());

				_instance.Init();
			}
			else if (_instance != this)
			{
				Debug.LogFormat("Awake Singleton-instance. - Duplicate - Type: {0}, InstanceID: {1}, This: {2}", typeof(T).FullName, _instance.GetInstanceID(), this.GetInstanceID());
				Destroy(gameObject);
			}
		}

		// This function is called when the instance is used the first time
		// Put all the initializations you need here, as you would do in Awake
		protected virtual void Init()
		{
			/* BLANK */
		}

		protected virtual void Release()
		{
			/* BLANK */
		}

		void OnDestroy()
		{
			if (_instance == this)
			{
				Debug.LogFormat("Destroy : {0}, InstanceID: {1}", typeof(T).FullName, _instance.GetInstanceID());
				
				_instance.Release();
				_instance = null;
			}
		}
	}
}
