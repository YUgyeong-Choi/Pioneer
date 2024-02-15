using UnityEngine;

/// <summary>
/// Debug.Log 계열의 함수를 성능 저하 없이 사용할 수 있게 Wrapping하는 클래스.
/// 특히, 빌드된 게임에서는 Debug.Log 호출이 성능에 영향을 주지 않도록 설계됨.
/// </summary>
public static class DebugEx
{
    // 로그의 출력 수준을 정의.
    public enum LogLevel
    {
        None,       // 로그를 출력하지 않음.
        Error,      // 오류 메시지만 출력.
        Warning,    // 경고와 오류 메시지를 출력.
        All         // 모든 로그를 출력.
    }

    // CurrentLogLevel은 현재 로그의 출력 수준을 나타냄. 기본값은 All.
    public static LogLevel CurrentLogLevel = LogLevel.All;

    // Log 메서드는 일반 메시지를 로깅.
    public static void Log(object message, Object context = null)
    {
#if UNITY_EDITOR
        if (CurrentLogLevel >= LogLevel.All)
        {
            Debug.Log(message, context);
        }
#endif
    }

    // LogWarning 메서드는 경고 메시지를 로깅.
    public static void LogWarning(object message, Object context = null)
    {
#if UNITY_EDITOR
        if (CurrentLogLevel >= LogLevel.Warning)
        {
            Debug.LogWarning(message, context);
        }
#endif
    }

    // LogError 메서드는 오류 메시지를 로깅.
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