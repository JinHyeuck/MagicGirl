/// <copyright file="Message.cs">Copyright (c) 2015 All Rights Reserved</copyright>
/// <author>Joris van Leeuwen</author>
/// <date>01/27/2014</date>

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using CodeControl.Internal;


#if CUSTOM_DEBUG
using DebugLog = Debug;


#else
using DebugLog = UnityEngine.Debug;
#endif


/// <summary>
/// A global messaging system that can be used to communicate between Controllers or other classes.
/// </summary>
public class Message
{
	/// <summary>
	/// A delegation type used to callback when messages are sent and received.
	/// </summary>
	/// <param name="callerType">The caller type of the message.</param>
	/// <param name="handlerType">The handler type of the message.</param>
	/// <param name="messageType">The type of the sent message.</param>
	/// <param name="messageName">The name of the sent message.</param>
	/// <param name="handlerMethodName">The name of the method handling the sent message.</param>
    public delegate void OnMessageHandleDelegate(Type callerType,Type handlerType,Type messageType,string messageName);

	/// <summary>
	/// Called when a message is sent and handled. Only works when in the Unity editor.
	/// </summary>
	public static OnMessageHandleDelegate OnMessageHandle;

	private static Dictionary<string, List<Delegate>> handlers = new Dictionary<string, List<Delegate>>();

	/// <summary>
	/// This prefix is added to typeless message names internally to distinct them from the typed messages
	/// </summary>
	private const string TypelessMessagePrefix = "typeless ";

	/// <summary>
	/// Adds a listener that triggers the given callback when the message with the given name is received.
	/// </summary>
	/// <param name="messageName">The message name that will be listened to.</param>
	/// <param name="callback">The callback that will be triggered when the message is received.</param>
	public static void AddListener(string messageName, Action callback)
	{
		RegisterListener(TypelessMessagePrefix + messageName, callback);
	}

	/// <summary>
	/// Adds a listener that triggers the given callback when a message of the given type is received.
	/// </summary>
	/// <typeparam name="T">The message type that will be listened to.</typeparam>
	/// <param name="callback">The callback that will be triggered when the message is received.</param>
	public static void AddListener<T>(Action<T> callback) where T : Message
	{
		RegisterListener(typeof(T).ToString(), callback);
	}

	/// <summary>
	/// Adds a listener that triggers the given callback when a message of the given type and name is received.
	/// </summary>
	/// <typeparam name="T">The message type that will be listened to.</typeparam>
	/// <param name="messageName">The message name that will be listened to.</param>
	/// <param name="callback">The callback that is triggered when the message is received.</param>
	public static void AddListener<T>(string messageName, Action<T> callback) where T : Message
	{
		RegisterListener(typeof(T).ToString() + messageName, callback);
	}

	/// <summary>
	/// Removes a listener that would trigger the given callback when a message with the given name is received.
	/// </summary>
	/// <param name="messageName">The message name that is being listened to.</param>
	/// <param name="callback">The callback that is triggered when the message is received.</param>
	public static void RemoveListener(string messageName, Action callback)
	{
		UnregisterListener(TypelessMessagePrefix + messageName, callback);
	}

	/// <summary>
	/// Removes a listener that would trigger the given callback when a message of the given type is received.
	/// </summary>
	/// <typeparam name="T">The message type that is being listened to.</typeparam>
	/// <param name="callback">The callback that is triggered when the message is received.</param>
	public static void RemoveListener<T>(Action<T> callback) where T : Message
	{
		UnregisterListener(typeof(T).ToString(), callback);
	}

	/// <summary>
	/// Removes a listener that would trigger the given callback when a message of the given type is received.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="messageName"></param>
	/// <param name="callback"></param>
	public static void RemoveListener<T>(string messageName, Action<T> callback) where T : Message
	{
		UnregisterListener(typeof(T).ToString() + messageName, callback);
	}

	/// <summary>
	/// Sends a message of the given name.
	/// </summary>
	/// <param name="messageName">The name of the message.</param>
	public static void Send(string messageName)
	{
		Type callerType = null;
		if (Application.isEditor)
		{
			var stackTrace = new StackTrace();
			var frame = stackTrace.GetFrame(1);
			callerType = (frame != null) ? frame.GetMethod().DeclaringType : null;

			if (string.IsNullOrEmpty(messageName))
				DebugLog.LogWarning("MessageName is empty.");
		}

		SendMessage<Message>(TypelessMessagePrefix + messageName, null, callerType);
	}

	/// <summary>
	/// Sends a message of the given name.
	/// </summary>
	/// <param name="messageName">The name of the message.</param>
	/// <param name="delay">Seconds to wait before sending the message (-1 means send end of frame).</param>
	public static void Send(string messageName, float delay)
	{
		if (Application.isEditor)
		{
			var stackTrace = new StackTrace();
			var frame = stackTrace.GetFrame(1);
			if (frame != null)
			{
				var callerType = frame.GetMethod().DeclaringType;
				ShowMessageFlow(callerType, typeof(Coroutiner), typeof(Message), messageName);
			}

			if (string.IsNullOrEmpty(messageName))
				DebugLog.LogWarning("MessageName is empty.");
		}
		
		Coroutiner.Start(SendDelay<Message>(delay, TypelessMessagePrefix + messageName, null));
	}

	/// <summary>
	/// Sends a message of the given type.
	/// </summary>
	/// <typeparam name="T">The type of the message.</typeparam>
	/// <param name="message">The instance of the message.</param>
	public static void Send<T>(T message) where T : Message
	{
		Type callerType = null;
		if (Application.isEditor)
		{
			var stackTrace = new StackTrace();
			var frame = stackTrace.GetFrame(1);
			callerType = (frame != null) ? frame.GetMethod().DeclaringType : null;
		}

		SendMessage<T>(typeof(T).ToString(), message, callerType);
	}

	/// <summary>
	/// Sends a message of the given type.
	/// </summary>
	/// <typeparam name="T">The type of the message.</typeparam>
	/// <param name="message">The instance of the message.</param>
	/// <param name="delay">Seconds to wait before sending the message (-1 means send end of frame).</param>
	public static void Send<T>(T message, float delay) where T : Message
	{
		if (Application.isEditor)
		{
			var stackTrace = new StackTrace();
			var frame = stackTrace.GetFrame(1);
			if (frame != null)
			{
				var callerType = frame.GetMethod().DeclaringType;
				ShowMessageFlow(callerType, typeof(Coroutiner), typeof(T), typeof(T).ToString());
			}
		}

		Coroutiner.Start(SendDelay<T>(delay, typeof(T).ToString(), message));
	}

	/// <summary>
	/// Sends a message of the given name and type.
	/// </summary>
	/// <typeparam name="T">The type of the message.</typeparam>
	/// <param name="messageName">The name of the message.</param>
	/// <param name="message">The instance of the message.</param>
	public static void Send<T>(string messageName, T message) where T : Message
	{
		Type callerType = null;
		if (Application.isEditor)
		{
			var stackTrace = new StackTrace();
			var frame = stackTrace.GetFrame(1);
			callerType = (frame != null) ? frame.GetMethod().DeclaringType : null;

			if (string.IsNullOrEmpty(messageName))
				DebugLog.LogWarningFormat("MessageName is empty. Type: {0}", typeof(T).Name);
		}

		SendMessage<T>(typeof(T).ToString() + messageName, message, callerType);
	}

	/// <summary>
	/// Sends a message of the given name and type.
	/// </summary>
	/// <typeparam name="T">The type of the message.</typeparam>
	/// <param name="messageName">The name of the message.</param>
	/// <param name="message">The instance of the message.</param>
	/// <param name="delay">Seconds to wait before sending the message (-1 means send end of frame).</param>
	public static void Send<T>(string messageName, T message, float delay) where T : Message
	{
		if (Application.isEditor)
		{
			var stackTrace = new StackTrace();
			var frame = stackTrace.GetFrame(1);
			if (frame != null)
			{
				var callerType = frame.GetMethod().DeclaringType;
				ShowMessageFlow(callerType, typeof(Coroutiner), typeof(T), typeof(T).ToString());
			}

			if (string.IsNullOrEmpty(messageName))
				DebugLog.LogWarningFormat("MessageName is empty. Type: {0}", typeof(T).Name);
		}

		Coroutiner.Start(SendDelay<T>(delay, typeof(T).ToString() + messageName, message));
	}

	private static void RegisterListener(string messageName, Delegate callback)
	{
		if (callback == null)
			throw new ArgumentNullException("callback", "Failed to add Message Listener because the given callback is null!");

		if (!handlers.ContainsKey(messageName))
		{
			handlers.Add(messageName, new List<Delegate>());
		}

		List<Delegate> messageHandlers = handlers[messageName];
		Delegate mh = messageHandlers.Find(x => x.Method == callback.Method && x.Target == callback.Target);
		if (mh != null)
		{
			DebugLog.LogFormat("callback Method : {0}, Callback Target : {1} ", callback.Method, callback.Target);
			throw new ArgumentException("Failed to add Message Listener because the given callback is overlap!", messageName);
		}
		
		messageHandlers.Add(callback);
	}

	private static void UnregisterListener(string messageName, Delegate callback)
	{
		if (!handlers.ContainsKey(messageName))
			return;
		
		List<Delegate> messageHandlers = handlers[messageName];
		Delegate messageHandler = messageHandlers.Find(x => x.Method == callback.Method && x.Target == callback.Target);
		if (messageHandler == null)
			return;
		
		messageHandlers.Remove(messageHandler);
	}

	private static void SendMessage<T>(string messageName, T e, Type callerType) where T : Message
	{
		if (!handlers.ContainsKey(messageName))
		{
#if UNITY_EDITOR
			DebugLog.LogWarningFormat("Failed to send message. MessageName: {0}, Type: {1}, CallerType: {2}", messageName, typeof(T).Name, callerType.Name);
#endif
			return;
		}

		bool sent = false;

		var messageHandlers = new List<Delegate>(handlers[messageName]);
		for (int i = 0; i < messageHandlers.Count; ++i)
		{
			Delegate messageHandler = messageHandlers[i];
			if (messageHandler.GetType() != typeof(Action<T>) && messageHandler.GetType() != typeof(Action))
			{
				continue;
			}

			if (callerType != null)
				ShowMessageFlow(callerType, messageHandler, typeof(T), messageName);

			if (typeof(T) == typeof(Message))
			{
				var action = (Action)messageHandler;
				action();
			}
			else
			{
				var action = (Action<T>)messageHandler;
				action(e);
			}

			sent = true;
		}
	
#if UNITY_EDITOR
		if (!sent)
			DebugLog.LogWarningFormat("Not send message. MessageName: {0}, Type: {1}, CallerType: {2}", messageName, typeof(T).Name, callerType.Name);
#endif
	}

	public static void ShowMessageFlow(Type callerType, Delegate messageHandler, Type messageType, string messageName)
	{
		if (Application.isEditor && OnMessageHandle != null)
		{
			messageName = messageName.Replace(TypelessMessagePrefix, "");

			if (messageType != typeof(Message))
			{
				messageName = messageName.Replace(messageType.ToString(), "");
			}

			OnMessageHandle(callerType, messageHandler.Target.GetType(), messageType, messageName);
		}
	}

	public static void ShowMessageFlow(Type callerType, Type handlerType, Type messageType, string messageName)
	{
		if (Application.isEditor && OnMessageHandle != null)
		{
			messageName = messageName.Replace(TypelessMessagePrefix, "");

			if (messageType != typeof(Message))
			{
				messageName = messageName.Replace(messageType.ToString(), "");
			}

			OnMessageHandle(callerType, handlerType, messageType, messageName);
		}
	}

	static IEnumerator SendDelay<T>(float delay, string messageName, T e) where T : Message
	{
		if (delay < 0.0f)
		{
			yield return new WaitForEndOfFrame();
		}
		else if (delay > 0.0f)
		{
			yield return new WaitForSeconds(delay);
		}

		SendMessage<T>(messageName, e, typeof(Coroutiner));
	}

	protected Message()
	{
	}
}
