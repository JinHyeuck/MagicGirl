

#if CUSTOM_DEBUG
public static class Debug
{
	public enum LOG_LEVEL
	{
		ALL = 0,
		WARNING,
		ERROR,
		EXCEPTION,
		NOTHING
	}

	public static LOG_LEVEL logLevel = LOG_LEVEL.ERROR;

	static bool IsEnable(LOG_LEVEL level)
	{
		if (logLevel <= level)
			return true;
		return false;
	}

	public static void Log(object message)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.Log(message);
	}

	public static void Log(object message, UnityEngine.Object context)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.Log(message, context);
	}

	public static void LogError(object message)
	{
		if (IsEnable(LOG_LEVEL.ERROR))
			UnityEngine.Debug.LogError(message);
	}

	public static void LogError(object message, UnityEngine.Object context)
	{
		if (IsEnable(LOG_LEVEL.ERROR))
			UnityEngine.Debug.LogError(message, context);
	}

	public static void LogWarning(object message)
	{
		if (IsEnable(LOG_LEVEL.WARNING))
			UnityEngine.Debug.LogWarning(message.ToString());
	}

	public static void LogWarning(object message, UnityEngine.Object context)
	{
		if (IsEnable(LOG_LEVEL.WARNING))
			UnityEngine.Debug.LogWarning(message.ToString(), context);
	}

	public static void Assert(bool condition)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.Assert(condition);
	}

	public static void Assert(bool condition, object message)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.Assert(condition, message);
	}

	public static void LogFormat(string format, params object[] args)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.LogFormat(format, args);
	}

	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.LogFormat(context, format, args);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		if (IsEnable(LOG_LEVEL.ERROR))
			UnityEngine.Debug.LogErrorFormat(format, args);
	}

	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (IsEnable(LOG_LEVEL.ERROR))
			UnityEngine.Debug.LogErrorFormat(context, format, args);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		if (IsEnable(LOG_LEVEL.WARNING))
			UnityEngine.Debug.LogWarningFormat(format, args);
	}

	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (IsEnable(LOG_LEVEL.WARNING))
			UnityEngine.Debug.LogWarningFormat(context, format, args);
	}

	public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color = default(UnityEngine.Color), float duration = 0.0f, bool depthTest = true)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	public static void DrawRay(UnityEngine.Vector3 start, UnityEngine.Vector3 dir, UnityEngine.Color color = default(UnityEngine.Color), float duration = 0.0f, bool depthTest = true)
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	public static void LogException(System.Exception e)
	{
		if (IsEnable(LOG_LEVEL.EXCEPTION))
			UnityEngine.Debug.LogException(e);
	}

	public static void LogException(System.Exception e, UnityEngine.Object context)
	{
		if (IsEnable(LOG_LEVEL.EXCEPTION))
			UnityEngine.Debug.LogException(e, context);
	}

	public static void Break()
	{
		if (IsEnable(LOG_LEVEL.ALL))
			UnityEngine.Debug.Break();
	}
}
#endif
