using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 렌더링에 필요한 공통 유틸리티입니다.
/// Material 캐싱, 배치 데이터 관리, 좌표 변환 등을 제공합니다.
/// </summary>
public static class UGraphic
{
    /// <summary>
    /// ERotation를 Z축 회전 각도(도)로 변환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RotateToAngle(ERotation rotate)
    {
        switch (rotate) {
            case ERotation.Up: return 0f;
            case ERotation.Left: return 90f;
            case ERotation.Down: return 180f;
            case ERotation.Right: return 270f;
            default: return 0f;
        }
    }

    /// <summary>
    /// ERotation를 Quaternion으로 변환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion RotateToQuaternion(ERotation rotate)
    {
        return Quaternion.Euler(0f, 0f, RotateToAngle(rotate));
    }

    /// <summary>
    /// 각도(도)를 Quaternion으로 변환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion AngleToQuaternion(float degree)
    {
        return Quaternion.Euler(0f, 0f, degree);
    }

    /// <summary>
    /// 위치, 회전, 크기로 TRS 행렬을 생성합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 BuildMatrix(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        return Matrix4x4.TRS(pos, rot, scale);
    }

    /// <summary>
    /// 위치와 크기로 회전 없는 TRS 행렬을 생성합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 BuildMatrix(Vector3 pos, Vector3 scale)
    {
        return Matrix4x4.TRS(pos, Quaternion.identity, scale);
    }

    /// <summary>
    /// 기본 머티리얼과 스프라이트로 새로운 머티리얼을 생성합니다.
    /// </summary>
    public static Material CreateMaterial(Material baseMaterial, Sprite sprite)
    {
        Material mat = new Material(baseMaterial);
        mat.mainTexture = sprite.texture;
        return mat;
    }

    /// <summary>
    /// 중심점을 기준으로 오프셋을 특정 각도(도)만큼 회전시킨 월드 좌표를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 RotateOffset(float centerX, float centerY, float offsetX, float offsetY, float degree, float z)
    {
        float radian = degree * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        float rotX = offsetX * cos - offsetY * sin;
        float rotY = offsetX * sin + offsetY * cos;
        return new Vector3(centerX + rotX, centerY + rotY, z);
    }

    private static Stack<List<Matrix4x4>> _listPool = new Stack<List<Matrix4x4>>();

    /// <summary>
    /// 풀에서 리스트를 꺼내오거나, 없으면 새로 만듭니다.
    /// </summary>
    private static List<Matrix4x4> GetListFromPool()
    {
        if (_listPool.Count > 0)
            return _listPool.Pop();
        return new List<Matrix4x4>(1000);
    }

    /// <summary>
    /// 다 쓴 리스트를 풀에 반납합니다.
    /// </summary>
    private static void ReturnListToPool(List<Matrix4x4> list)
    {
        list.Clear();
        _listPool.Push(list);
    }
}
