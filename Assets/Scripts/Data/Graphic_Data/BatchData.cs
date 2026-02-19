using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DrawMeshInstanced 에 사용하는 배치 데이터
/// </summary>
public class BatchData
{
    public readonly Mesh mesh;
    public readonly Material mat;
    private const int CHUNK_CAPACITY = 1023;

    private readonly List<Matrix4x4[]> _chunks; // 매트릭스를 저장해두는 리스트
    private readonly List<int> _counts; // n번째 청크에서 현재 사용중인 공간 수
    private int _activeChunkCount; // 현재 n번째 청크까지 사용 중

    public BatchData(Mesh mesh, Material mat)
    {
        this.mesh = mesh;
        this.mat = mat;
        _chunks = new List<Matrix4x4[]>(4);
        _counts = new List<int>(4);
        _chunks.Add(new Matrix4x4[CHUNK_CAPACITY]);
        _counts.Add(0);
        _activeChunkCount = 1;
    }

    /// <summary>
    /// count를 0으로 되돌립니다.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < _activeChunkCount; ++i)
            _counts[i] = 0;
        _activeChunkCount = 1;
    }

    /// <summary>
    /// 이번 프레임에 렌더링할 매트릭스를 추가합니다.
    /// </summary>
    /// <param name="matrix"></param>
    public void Add(in Matrix4x4 matrix)
    {
        // 캐싱
        int index = _activeChunkCount - 1; // 현재 청크 인덱스
        int count = _counts[index]; // 현재 청크에서 사용한 공간
        // 한 번의 드로우 콜로 그릴 수 있는 공간이 꽉 참
        if (count >= CHUNK_CAPACITY) {
            _activeChunkCount++;
            index = _activeChunkCount - 1;
            // 청크를 새로 생성해야 함
            if (index >= _chunks.Count) {
                _chunks.Add(new Matrix4x4[CHUNK_CAPACITY]);
                _counts.Add(0);
            }
            // 다음 청크 카운트 0으로
            else {
                _counts[index] = 0;
            }
            count = 0;
        }
        // 렌더링할 매트릭스 추가
        _chunks[index][count] = matrix;
        _counts[index] = count + 1;
    }

    
    public void Draw()
    {
        for (int i = 0; i < _activeChunkCount; ++i) {
            int count = _counts[i];
            if (count > 0)
                Graphics.DrawMeshInstanced(mesh, 0, mat, _chunks[i], count);
        }
    }
}
