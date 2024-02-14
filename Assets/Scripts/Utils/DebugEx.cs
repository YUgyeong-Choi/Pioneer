using UnityEngine;

/// <summary>
/// Debug Log 기능을 Wrapping하는 클래스. 기존의 Debug.Log계열 함수는 빌드에 포함되면 성능을 잡아먹는데, 이 클래스 내의 Log 계열 함수는 그렇지 않도록 만들어졌음.
/// </summary>
public static class DebugEx
{
    public enum LogLevel
    {
        None,
        Error,
        Warning,
        All
    }

    public static LogLevel CurrentLogLevel = LogLevel.All;

    public static void Log(object message, Object context = null)
    {
#if UNITY_EDITOR
        if (CurrentLogLevel >= LogLevel.All)
        {
            Debug.Log(message, context);
        }
#endif
    }

    public static void LogWarning(object message, Object context = null)
    {
#if UNITY_EDITOR
        if (CurrentLogLevel >= LogLevel.Warning)
        {
            Debug.LogWarning(message, context);
        }
#endif
    }

    public static void LogError(object message, Object context = null)
    {
#if UNITY_EDITOR
        if (CurrentLogLevel >= LogLevel.Error)
        {
            Debug.LogError(message, context);
        }
#endif
    }
}