using System.Runtime.CompilerServices;
using UnityEngine;
/// <summary>
/// 범위 및 거리를 계산하는 유틸리티입니다.
/// </summary>
public class URange
{
    /// <summary>
    /// 두 좌표 사이의 거리가 일정 거리 이하인지 검사합니다.
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InCircle(Vector2 pos1, Vector2 pos2, float distance)
    {
        float sqrtDistance = distance * distance;
        float sqrtBetween = UMath.GetDistanceSquare(pos1, pos2);
        if (sqrtBetween < sqrtDistance) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 두 좌표 사이의 거리가 일정 거리 이하인지 검사합니다.
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InCircle(float x1, float y1, float x2, float y2, float distance)
    {
        float sqrtDistance = distance * distance;
        float sqrtBetween = UMath.GetDistanceSquare(x1, y1, x2, y2);
        if (sqrtBetween < sqrtDistance) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 좌표가 특정 좌표를 중심으로 한 사각형 범위 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rectPos"></param>
    /// <param name="diameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InRect(Vector2 pos, Vector2 rectPos, float diameter)
    {
        float left = rectPos.x - diameter;
        float right = rectPos.x + diameter;
        float up = rectPos.y - diameter;
        float down = rectPos.y + diameter;
        if (pos.x < left) return false;
        if (right < pos.x) return false;
        if (pos.y < down) return false;
        if (up < pos.y) return false;
        return true;
    }

    /// <summary>
    /// 좌표가 특정 좌표를 중심으로 한 사각형 범위 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="rectX"></param>
    /// <param name="rectY"></param>
    /// <param name="diameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InRect(float x, float y, float rectX, float rectY, float diameter)
    {
        float left = rectX - diameter;
        float right = rectX + diameter;
        float up = rectY - diameter;
        float down = rectY + diameter;
        if (x < left) return false;
        if (right < x) return false;
        if (y < down) return false;
        if (up < y) return false;
        return true;
    }
}
