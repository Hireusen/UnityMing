using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 매 프레임 designs를 순회하여 디자인 블록의 렌더링 배치 데이터를 구성하고 그립니다.
/// </summary>
public class DesignBatchBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _baseMaterial;

    [Header("사용자 정의 설정")]
    [SerializeField, Range(0.1f, 1f)] private float _alpha = 0.75f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private Dictionary<BlockSpriteKey, int> _keyToSlot;
    private List<BatchLists> _batches;
    private int _batchCount;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 외부 공개 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_mesh);
        De.IsNull(_baseMaterial);
    }

    public void Initialize()
    {
        _keyToSlot = new Dictionary<BlockSpriteKey, int>();
        _batches = new List<BatchLists>();
        _batchCount = 0;
    }

    /// <summary>
    /// 매 프레임 designs 순회 → 배치 구성 → DrawMeshInstanced
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game) || De.IsNull(game.Player)) return;

        var designs = game.Player.designs;
        ClearAllBatches();

        if (designs.Count <= 0)
            return;

        BlockMap blockMap = game.Blocks;
        var blockSO = game.BlockDatabase;

        foreach (var kvp in designs) {
            BuildOrder order = kvp.Value;
            if (order.type != EOrderType.Build)
                continue;
            if (!blockSO.TryGetValue(order.id, out SO_Block so))
                continue;

            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
            Vector2 renderPos = blockMap.GetRenderPos(order.index, size, order.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;
            float blockAngle = UGrid.RotationToAngle(order.rotation);
            Vector3 scale = new Vector3(so.Size.x, so.Size.y, 1f);

            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                SpriteInfo info = so.GetSpriteInfo(si);
                int slot = GetOrCreateSlot(new BlockSpriteKey(order.id, si), info.sprite);
                float posZ = Const.DESIGN_BLOCK + info.offset.z;

                // 디자인 블록은 설치 방향으로만 회전
                Quaternion rot = (info.type == ESpriteType.Static || info.type == ESpriteType.Effect)
                    ? Quaternion.identity
                    : UGraphic.AngleToQuaternion(blockAngle);

                Vector3 pos = UGraphic.RotateOffset(baseX, baseY, info.offset.x, info.offset.y, blockAngle, posZ);
                AddMatrix(slot, UGraphic.BuildMatrix(pos, rot, scale));
            }
        }

        // 그리기
        DrawAllBatches();
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private int GetOrCreateSlot(BlockSpriteKey key, Sprite sprite)
    {
        if (_keyToSlot.TryGetValue(key, out int slot))
            return slot;
        Material mat = UGraphic.CreateMaterial(_baseMaterial, sprite);
        Color c = mat.color;
        c.a = _alpha;
        mat.color = c;
        slot = _batches.Count;
        _batches.Add(new BatchLists(mat));
        _keyToSlot.Add(key, slot);
        _batchCount = _batches.Count;
        return slot;
    }

    private void AddMatrix(int slot, Matrix4x4 matrix)
    {
        List<List<Matrix4x4>> matrices = _batches[slot].matrices;
        List<Matrix4x4> curList = matrices[matrices.Count - 1];
        if (1000 <= curList.Count) {
            curList = new List<Matrix4x4>(1000);
            matrices.Add(curList);
        }
        curList.Add(matrix);
    }

    private void ClearAllBatches()
    {
        for (int i = 0; i < _batchCount; ++i) {
            List<List<Matrix4x4>> matrices = _batches[i].matrices;
            for (int j = 0; j < matrices.Count; ++j)
                matrices[j].Clear();
            while (matrices.Count > 1)
                matrices.RemoveAt(matrices.Count - 1);
        }
    }

    private void DrawAllBatches()
    {
        for (int i = 0; i < _batchCount; ++i) {
            Material mat = _batches[i].Mat;
            List<List<Matrix4x4>> matrices = _batches[i].matrices;
            for (int j = 0; j < matrices.Count; ++j) {
                if (matrices[j].Count <= 0) continue;
                Graphics.DrawMeshInstanced(_mesh, 0, mat, matrices[j]);
            }
        }
    }
    #endregion
}
