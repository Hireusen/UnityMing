using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BlockSpriteKey → BatchData 매핑을 관리하는 재사용 가능한 배치 레지스트리.
/// 매 프레임 ClearAll() → GetOrCreateSlot() + Add() → DrawAll() 로 운용합니다.
/// 내부 컬렉션을 제거하지 않으므로 정상 상태(워밍업 이후)에서 GC 할당이 0입니다.
/// </summary>
public class BatchRegistry
{
    private readonly Mesh _mesh;
    private readonly Material _baseMaterial;
    private readonly float _alpha;

    private readonly Dictionary<BlockSpriteKey, int> _keyToSlot; // 
    private readonly List<BatchData> _batches;
    private int _batchCount;

    /// <param name="baseMaterial">복제 원본 머테리얼</param>
    /// <param name="alphaOverride">0 이상이면 머테리얼 색상 알파를 이 값으로 덮어씁니다. 음수면 무시.</param>
    public BatchRegistry(Mesh mesh, Material baseMaterial, float alpha = -1f)
    {
        this._mesh = mesh;
        this._baseMaterial = baseMaterial;
        this._alpha = alpha;
        this._keyToSlot = new Dictionary<BlockSpriteKey, int>(sbyte.MaxValue);
        this._batches = new List<BatchData>(20);
        this._batchCount = 0;
    }

    /// <summary>모든 배치의 매트릭스를 비웁니다. 컬렉션 인스턴스는 유지. GC-free.</summary>
    public void ClearAll()
    {
        for (int i = 0; i < _batchCount; ++i)
            _batches[i].Clear();
    }

    /// <summary>
    /// 키에 해당하는 슬롯 인덱스를 반환합니다.
    /// </summary>
    public int GetOrCreateSlot(BlockSpriteKey key, Sprite sprite)
    {
        // 블록 ID와 스프라이트 ID를 고려한 고유값 반환
        if (_keyToSlot.TryGetValue(key, out int slot))
            return slot;
        // 머티리얼 생성
        Material mat = UGraphic.CreateMaterial(_baseMaterial, sprite);
        // 투명도 적용
        if (0f < _alpha) {
            Color color = mat.color;
            color.a = _alpha;
            mat.color = color;
        }
        // 배치 데이터 생성
        slot = _batchCount;
        _batches.Add(new BatchData(_mesh, mat)); // 배치 등록
        _keyToSlot.Add(key, slot); // 키 등록
        _batchCount++;
        return slot;
    }

    /// <summary>
    /// 해당 슬롯에 매트릭스를 추가합니다.
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="matrix"></param>
    public void Add(int slot, in Matrix4x4 matrix)
    {
        _batches[slot].Add(in matrix);
    }

    /// <summary>
    /// 모든 활성 배치를 렌더링합니다.
    /// </summary>
    public void DrawAll()
    {
        for (int i = 0; i < _batchCount; ++i) {
            _batches[i].Draw();
        }
    }
}
