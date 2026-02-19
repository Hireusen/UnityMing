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
    private BatchManagement _registry;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 외부 공개 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_mesh);
        De.IsNull(_baseMaterial);
    }

    public void Initialize()
    {
        _registry = new BatchManagement(_mesh, _baseMaterial, _alpha);
    }

    /// <summary>
    /// 매 프레임 designs 순회 → 배치 구성 → DrawMeshInstanced
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game) || De.IsNull(game.Player)) return;

        var designs = game.Player.designs;
        _registry.ClearAll();

        if (designs.Count <= 0)
            return;

        BlockMap blockMap = game.Blocks;
        var blockSO = game.BlockDatabase;

        foreach (var order in designs.Values) {
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
            for (int i = 0; i < spriteCount; ++i) {
                SpriteInfo info = so.GetSpriteInfo(i);
                int slot = _registry.GetOrCreateSlot(new BlockSpriteKey(order.id, i), info.sprite);
                // 회전
                Quaternion rot;
                if (info.type == ESpriteType.Static) {
                    rot = Quaternion.identity;
                } else {
                    rot = UGraphic.AngleToQuaternion(blockAngle);
                }
                // 좌표
                float posZ = Const.DESIGN_BLOCK + info.offset.z;
                Vector3 pos = UGraphic.RotateOffset(baseX, baseY, info.offset.x, info.offset.y, blockAngle, posZ);
                // 매트릭스
                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                _registry.Add(slot, in matrix);
            }
        }

        _registry.DrawAll();
    }
    #endregion
}
