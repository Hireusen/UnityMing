using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 블록 그래픽 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 PoolManagement&lt;BlockSingle&gt;의 활성 블록만 순회하여
/// 렌더링 배치 데이터를 구성합니다.
/// </summary>
public class BlockBatchBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Material _baseMaterial;
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
        _materialCache.Add(key, mat);
        return mat;
    }

    /// <summary>
    /// 매 프레임 블록 풀을 순회하여 렌더링 배치 데이터를 재구성합니다.
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game)) return;

        PoolManagement<BlockSingle> pool = game.BlockPool;
        BlockMap blockMap = game.Blocks;
        int capacity = pool.Capacity;

        // 이전 프레임 데이터 초기화
        UGraphic.ClearBatches(_batchMap, _activeKeys);

        // 풀의 활성 슬롯만 순회
        for (int adress = 0; adress < capacity; ++adress) {
            if (!pool.IsExist(adress))
                continue;

            ref readonly BlockSingle block = ref pool.Read(adress);
            if (block.IsVoid())
                continue;

            SO_Block so = GetSO(block.id);
            if (so == null)
                continue;

            // 렌더링 중심 좌표 (GetRenderPos가 회전 + 크기를 모두 고려)
            Vector2 renderPos = blockMap.GetRenderPos(block.index, block.size, block.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;

            // 블록 설치 방향 각도
            float blockAngle = UGrid.RotationToAngle(block.rotation);

            // 부품별 독립 회전값 (터렛 포탑, 드릴 날 등)
            float angle1 = block.partAngle_1;
            float angle2 = block.partAngle_2;
            float angle3 = block.partAngle_3;

            // SpriteInfo 전체 순회
            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                SpriteInfo info = so.GetSpriteInfo(si);
                BlockSpriteKey key = new BlockSpriteKey(block.id, si);

                GetOrCreateMaterial(key, info.sprite);

                float posZ = -30f + info.offset.z;
                float offsetX = info.offset.x;
                float offsetY = info.offset.y;

                Quaternion rot;
                Vector3 pos;

                switch (info.type) {
                    // 블록 방향을 따라 회전
                    case ESpriteType.Body:
                        rot = UGraphic.AngleToQuaternion(blockAngle);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, blockAngle, posZ);
                        break;
                    // 독립 회전 1
                    case ESpriteType.Turret:
                        rot = UGraphic.AngleToQuaternion(angle1);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, angle1, posZ);
                        break;
                    // 독립 회전 2
                    case ESpriteType.Rotation:
                        rot = UGraphic.AngleToQuaternion(angle2);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, blockAngle, posZ);
                        break;
                    // 회전 없음
                    case ESpriteType.Static:
                    default:
                        rot = Quaternion.identity;
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, blockAngle, posZ);
                        break;
                }

                Vector3 scale = new Vector3(so.Size.x, so.Size.y, 1f);
                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                UGraphic.AddMatrix(_batchMap, _activeKeys, key, matrix);
            }
        }
    }
    #endregion
}
