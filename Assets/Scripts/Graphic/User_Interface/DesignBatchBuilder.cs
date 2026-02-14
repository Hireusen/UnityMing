using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 designs를 순회하여 디자인 블록(건설 예정)의 렌더링 배치 데이터를 구성합니다.
/// BatchLists 기반으로 키 조회 없이 인덱스로 직접 접근합니다.
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

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
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

    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game)) return;
        if (De.IsNull(game.Player)) return;

        var designs = game.Player.designs;
        ClearAllBatches();

        if (designs.Count <= 0)
            return;

        BlockMap blockMap = game.Blocks;

        foreach (var kvp in designs) {
            BuildOrder order = kvp.Value;
            if (order.type != EOrderType.Build)
                continue;

            EBlock id = order.id;
            if (!game.BlockDatabase.TryGetValue(id, out SO_Block so))
                continue;

            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
            Vector2 renderPos = blockMap.GetRenderPos(order.index, size, order.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;
            float blockAngle = UGrid.RotationToAngle(order.rotation);

            // 모든 스프라이트에 블록 크기 적용
            Vector3 scale = new Vector3(so.Size.x, so.Size.y, 1f);

            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                SpriteInfo info = so.GetSpriteInfo(si);
                BlockSpriteKey key = new BlockSpriteKey(id, si);
                int slot = GetOrCreateSlot(key, info.sprite);

                float posZ = -30.5f + info.offset.z;

                Quaternion rot;
                switch (info.type) {
                    case ESpriteType.Body:
                    case ESpriteType.Turret:
                    case ESpriteType.Rotation:
                        rot = UGraphic.AngleToQuaternion(blockAngle);
                        break;
                    default:
                        rot = Quaternion.identity;
                        break;
                }

                Vector3 pos = UGraphic.RotateOffset(baseX, baseY, info.offset.x, info.offset.y, blockAngle, posZ);
                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                AddMatrix(slot, matrix);
            }
        }

        // 그리기
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
