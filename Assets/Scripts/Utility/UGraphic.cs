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

        // 풀이 비었을 때만 새로 할당 (초반에만 발생)
        return new List<Matrix4x4>(1000);
    }

    /// <summary>
    /// 다 쓴 리스트를 풀에 반납합니다.
    /// </summary>
    private static void ReturnListToPool(List<Matrix4x4> list)
    {
        list.Clear(); // 내용은 비우고
        _listPool.Push(list); // 창고에 넣음
    }

    
    public static void ClearBatches<TKey>(
        Dictionary<TKey, List<List<Matrix4x4>>> batchMap,
        List<TKey> activeKeys)
    {
        // 활성화되었던 키들을 순회하며 리스트 회수
        for (int i = 0; i < activeKeys.Count; i++) {
            var key = activeKeys[i];
            if (batchMap.TryGetValue(key, out var lists)) {
                // 사용된 모든 하위 리스트를 풀에 반납
                for (int j = 0; j < lists.Count; j++) {
                    ReturnListToPool(lists[j]);
                }
                lists.Clear(); // 껍데기 리스트 비우기
            }
        }
        activeKeys.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddMatrix<TKey>(
        Dictionary<TKey, List<List<Matrix4x4>>> batchMap,
        List<TKey> keys,
        TKey key,
        Matrix4x4 matrix,
        int batchLimit = 1000)
    {
        List<List<Matrix4x4>> lists;
        if (!batchMap.TryGetValue(key, out lists)) {
            lists = new List<List<Matrix4x4>>();
            batchMap.Add(key, lists);
        }

        // 해당 키가 이번 프레임에 처음 쓰이는 경우 (비어있음)
        if (lists.Count == 0) {
            lists.Add(GetListFromPool()); // 풀에서 꺼내옴
            keys.Add(key); // 활성 키 목록에 추가
        }

        // 현재 채우고 있는 리스트 가져오기
        List<Matrix4x4> curList = lists[lists.Count - 1];

        // 꽉 찼으면 새 리스트 배급
        if (batchLimit <= curList.Count) {
            curList = GetListFromPool(); // new List() 대신 풀 사용!
            lists.Add(curList);
        }

        curList.Add(matrix);
    }

    /// <summary>
    /// batchMap에 저장된 렌더링 데이터를 DrawMeshInstanced로 그립니다.
    /// </summary>
    /// <typeparam name="TKey">딕셔너리 키 타입</typeparam>
    public static void DrawBatches<TKey>(
        Mesh mesh,
        Dictionary<TKey, List<List<Matrix4x4>>> batchMap,
        List<TKey> activeKeys,
        Dictionary<TKey, Material> materialCache)
    {
        for (int i = 0; i < activeKeys.Count; ++i) {
            TKey key = activeKeys[i];
            if (!materialCache.TryGetValue(key, out Material mat))
                continue;
            var lists = batchMap[key];
            for (int j = 0; j < lists.Count; ++j) {
                if (lists[j].Count <= 0)
                    continue;
                Graphics.DrawMeshInstanced(mesh, 0, mat, lists[j]);
            }
        }
    }
}
