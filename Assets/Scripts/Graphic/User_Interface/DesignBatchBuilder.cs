using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 designs를 순회하여 디자인 블록(건설 예정 블록)의 렌더링 배치 데이터를 구성합니다.
/// sequence 리스트를 직접 순회하므로 키 버퍼 할당이 없습니다.
/// </summary>
public class DesignBatchBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Material _baseMaterial;

    [Header("사용자 정의 설정")]
    [SerializeField, Range(0.1f, 1f)] private float _alpha = 0.75f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private Dictionary<BlockSpriteKey, Material> _materialCache;
    private Dictionary<EBlock, SO_Block> _soCache;
    private Dictionary<BlockSpriteKey, List<List<Matrix4x4>>> _batchMap;
    private List<BlockSpriteKey> _activeKeys;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 접근자 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public Dictionary<BlockSpriteKey, Material> MaterialCache => _materialCache;
    public Dictionary<BlockSpriteKey, List<List<Matrix4x4>>> BatchMap => _batchMap;
    public List<BlockSpriteKey> ActiveKeys => _activeKeys;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_baseMaterial);
    }

    public void Initialize()
    {
        _materialCache = new Dictionary<BlockSpriteKey, Material>();
        _soCache = new Dictionary<EBlock, SO_Block>();
        _batchMap = new Dictionary<BlockSpriteKey, List<List<Matrix4x4>>>();
        _activeKeys = new List<BlockSpriteKey>();
    }

    private SO_Block GetSO(EBlock id)
    {
        if (_soCache.TryGetValue(id, out SO_Block so))
            return so;
        if (!GameData.ins.BlockDatabase.TryGetValue(id, out so))
            return null;
        _soCache.Add(id, so);
        return so;
    }

    private Material GetOrCreateMaterial(BlockSpriteKey key, Sprite sprite)
    {
        if (_materialCache.TryGetValue(key, out Material mat))
            return mat;
        if (sprite == null)
            return null;
        mat = UGraphic.CreateMaterial(_baseMaterial, sprite);
        Color color = mat.color;
        color.a = _alpha;
        mat.color = color;
        _materialCache.Add(key, mat);
        return mat;
    }

    /// <summary>
    /// 매 프레임 designs를 순회하여 렌더링 배치 데이터를 재구성합니다.
    /// sequence 리스트를 직접 순회 — GC 할당 없음
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game)) return;
        if (De.IsNull(game.Player)) return;

        var designs = game.Player.designs;
        int count = designs.Count;

        UGraphic.ClearBatches(_batchMap, _activeKeys);

        if (count <= 0)
            return;

        BlockMap blockMap = game.Blocks;
        int width = blockMap.Width;

        foreach(var index in designs.Keys) {
            if (!designs.TryGetValue(index, out BuildOrder order))
                continue;
            if (order.type != EOrderType.Build)
                continue;

            EBlock id = order.id;
            SO_Block so = GetSO(id);
            if (so == null)
                continue;

            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);

            // GetRenderPos로 정확한 렌더링 중심 계산
            Vector2 renderPos = blockMap.GetRenderPos(index, size, order.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;

            float blockAngle = UGrid.RotationToAngle(order.rotation);

            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                SpriteInfo info = so.GetSpriteInfo(si);
                BlockSpriteKey spriteKey = new BlockSpriteKey(id, si);

                GetOrCreateMaterial(spriteKey, info.sprite);

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
                Vector3 scale = new Vector3(so.Size.x, so.Size.y, 1f);

                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                UGraphic.AddMatrix(_batchMap, _activeKeys, spriteKey, matrix);
            }
        }
    }
    #endregion
}
