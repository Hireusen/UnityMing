using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 매 프레임 selecteds를 순회하여 반투명 블록의 배치 데이터를 구성하고 그립니다.
/// </summary>
public class SelectedBatchBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _baseMaterial;

    [Header("사용자 정의 설정")]
    [SerializeField, Range(0.1f, 1f)] private float _alpha = 0.5f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private BatchManagement _registry;
    private Dictionary<EBlock, SO_Block> _soCache;
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
        _soCache = new Dictionary<EBlock, SO_Block>();
    }

    /// <summary>
    /// 매 프레임 selecteds 순회 → 배치 구성 → DrawMeshInstanced
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game) || De.IsNull(game.Player)) return;

        List<SelectedBlock> selecteds = game.Player.selecteds;
        int count = selecteds.Count;

        _registry.ClearAll();

        if (count <= 0)
            return;

        Vector2 cursor = game.Player.CursorInGrid;
        float cursorX = cursor.x;
        float cursorY = cursor.y;

        // SO 연속 캐싱 (드래그 타일링에서 같은 id 반복)
        EBlock lastId = EBlock.None;
        SO_Block lastSO = null;
        float lastSizeX = 0f;
        float lastSizeY = 0f;
        Vector2Int lastSize = Vector2Int.zero;
        int lastSpriteCount = 0;

        for (int i = 0; i < count; ++i) {
            SelectedBlock sel = selecteds[i];
            if (sel.id == EBlock.None)
                continue;

            // SO 조회 (같은 id 연속 시 스킵)
            if (sel.id != lastId) {
                lastId = sel.id;
                lastSO = GetSO(sel.id);
                if (lastSO == null) continue;
                lastSizeX = lastSO.Size.x;
                lastSizeY = lastSO.Size.y;
                lastSize = new Vector2Int((int)lastSizeX, (int)lastSizeY);
                lastSpriteCount = lastSO.SpriteCount;
            }
            if (lastSO == null) continue;

            // 그리드 스냅 → 렌더링 중심
            float gridX = Mathf.Floor(cursorX + sel.offsetX);
            float gridY = Mathf.Floor(cursorY + sel.offsetY);
            Vector2 renderPos = GetSelectedRenderPos(gridX, gridY, lastSize, sel.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;
            float blockAngle = UGrid.RotationToAngle(sel.rotation);
            Vector3 scale = new Vector3(lastSizeX, lastSizeY, 1f);

            for (int si = 0; si < lastSpriteCount; ++si) {
                SpriteInfo info = lastSO.GetSpriteInfo(si);
                int slot = _registry.GetOrCreateSlot(new BlockSpriteKey(sel.id, si), info.sprite);
                float posZ = Const.SELECTED_BLOCK + info.offset.z;

                Quaternion rot = (info.type == ESpriteType.Static || info.type == ESpriteType.Effect)
                    ? Quaternion.identity
                    : UGraphic.AngleToQuaternion(blockAngle);

                Vector3 pos = UGraphic.RotateOffset(baseX, baseY, info.offset.x, info.offset.y, blockAngle, posZ);
                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                _registry.Add(slot, in matrix);
            }
        }

        _registry.DrawAll();
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private SO_Block GetSO(EBlock id)
    {
        if (_soCache.TryGetValue(id, out SO_Block so))
            return so;
        if (!GameData.ins.BlockDatabase.TryGetValue(id, out so))
            return null;
        _soCache.Add(id, so);
        return so;
    }

    /// <summary>
    /// 맵 밖에서도 사용 가능한 selected 전용 렌더링 좌표 계산
    /// </summary>
    private static Vector2 GetSelectedRenderPos(float gridX, float gridY, Vector2Int size, ERotation rotation)
    {
        int sx = size.x;
        int sy = size.y;
        switch (rotation) {
            case ERotation.Up: return new Vector2(gridX + sx * 0.5f, gridY + sy * 0.5f);
            case ERotation.Right: return new Vector2(gridX + sy * 0.5f, gridY - sx * 0.5f);
            case ERotation.Down: return new Vector2(gridX - sx * 0.5f, gridY - sy * 0.5f);
            case ERotation.Left: return new Vector2(gridX - sy * 0.5f, gridY + sx * 0.5f);
            default: return new Vector2(gridX + sx * 0.5f, gridY + sy * 0.5f);
        }
    }
    #endregion
}
