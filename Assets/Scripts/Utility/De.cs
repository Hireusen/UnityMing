#pragma warning disable IDE0060, CS0162
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class De
{
    // 로그 출력 여부
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private const bool ENABLE_LOG = true;
    private const bool ENABLE_ONCE = false;
    private const bool ENABLE_PAUSE = false;
#else
    private const bool ENABLE_LOG = false;
    private const bool ENABLE_ONCE = true;
    private const bool ENABLE_PAUSE = false;
#endif
    #region 내부 멤버
    private static readonly HashSet<string> _logHistory = new HashSet<string>();

    // 플레이 모드가 시작될 때 자동으로 로그 기록을 초기화합니다.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetHistory()
    {
        _logHistory.Clear();
    }

    private static void LogInternal(string message, string file, int line, LogType type = LogType.Log)
    {
        if (!ENABLE_LOG)
            return;
        if (ENABLE_ONCE) {
            string key = file + line;
            if (_logHistory.Contains(key)) return;
            _logHistory.Add(key);
        }
        switch (type) {
            case LogType.Error:
            case LogType.Assert:
            case LogType.Exception:
                UnityEngine.Debug.LogError(message);
                break;
            case LogType.Warning:
                UnityEngine.Debug.LogWarning(message);
                break;
            default:
                UnityEngine.Debug.Log(message);
                break;
        }
        if (ENABLE_PAUSE) {
            UnityEngine.Debug.Break();
        }
    }
    #endregion

    #region 외부 공개 멤버
    /// <summary>
    /// 오브젝트가 Null 또는 Fake Null일 경우 True를 반환하고 로그를 출력합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull<T>(
        T obj,
        LogType logType = LogType.Warning,
        [CallerArgumentExpression("obj")] string objName = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        bool isNull = false;
        // 검사
        if (obj is UnityEngine.Object unityObj) {
            isNull = unityObj == null;
        } else {
            isNull = obj == null;
        }
        // 로그
        if (isNull && ENABLE_LOG) {
            string msg = $"<color=red>[Null]</color> {objName} is Null";
            LogInternal(msg, file, line, logType);
        }
        return isNull;
    }

    /// <summary>
    /// 조건이 True일 경우 True를 반환하고 로그를 출력합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTrue(
        bool condition,
        LogType logType = LogType.Warning,
        [CallerArgumentExpression("condition")] string message = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (condition) {
            if (ENABLE_LOG) {
                LogInternal($"<color=red>[True]</color> {message}", file, line, logType);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 조건이 False일 경우 True를 반환하고 로그를 출력합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFalse(
        bool condition,
        LogType logType = LogType.Warning,
        [CallerArgumentExpression("condition")] string message = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (!condition)
        {
            if (ENABLE_LOG) {
                LogInternal($"<color=red>[False]</color> {message}", file, line, logType);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 로그를 출력합니다.
    /// </summary>
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Print(
        object message,
        LogType logType = LogType.Log,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        LogInternal($"<color=cyan>[Log]</color> {message}", file, line, logType);
    }
    #endregion

    /// <summary>
    /// GUIStyle 변수에 접근합니다.
    /// </summary>
    private static GUIStyle _rectStyle;
    public static GUIStyle RectStyle
    {
        get {
            if (_rectStyle == null) {
                _rectStyle = new GUIStyle();
                _rectStyle.normal.textColor = Color.black;
                _rectStyle.alignment = TextAnchor.MiddleCenter;
            }
            return _rectStyle;
        }
    }

    public enum EWhere { LeftUp, Up, RightUp, Left, Center, Right, LeftDown, Down, RightDown }
    /// <summary>
    /// 게임 화면에 텍스트를 출력합니다.
    /// OnGUI 이벤트 내부에서 호출해야 합니다.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fontSize"></param>
    /// <param name="where"></param>
    public static void DrawText(string text, int fontSize = 30, EWhere where = EWhere.Up)
    {
        float w = Screen.width, h = Screen.height;
        float halfW = w * 0.5f, halfH = h * 0.5f;
        float x = 0, y = 0;
        // 가로 정렬
        switch (where) {
            case EWhere.LeftUp: case EWhere.Left: case EWhere.LeftDown:
            case EWhere.Up: case EWhere.Center: case EWhere.Down:
                x = 0; break;
            case EWhere.RightUp: case EWhere.Right: case EWhere.RightDown:
                x = halfW; break;
        }
        // 세로 정렬
        switch (where) {
            case EWhere.LeftUp: case EWhere.Up: case EWhere.RightUp: y = 0; break;
            case EWhere.Left: case EWhere.Center: case EWhere.Right: y = 0; break; // 전체 높이 사용
            case EWhere.LeftDown: case EWhere.Down: case EWhere.RightDown: y = halfH; break;
        }
        // 크기 설정
        float rectW = (where == EWhere.Center || where == EWhere.Up || where == EWhere.Down) ? w : halfW;
        float rectH = (where == EWhere.Center || where == EWhere.Left || where == EWhere.Right) ? h : halfH;

        RectStyle.fontSize = fontSize;
        GUI.Label(new Rect(x, y, rectW, rectH), text, RectStyle);
    }
}
// C# 최신 문법을 구버전에서 쓰기 위한 코드입니다.
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
        public string ParameterName { get; }
    }
}
