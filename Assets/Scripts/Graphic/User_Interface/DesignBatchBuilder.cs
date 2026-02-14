using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 designs 딕셔너리를 순회하여 디자인 블록(건설 예정 블록)의 렌더링 배치 데이터를 구성합니다.
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
    private List<int> _keyBuffer;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 외부 접근 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public Dictionary<BlockSpriteKey, Material> MaterialCache => _materialCache;
    public Dictionary<BlockSpriteKey, List<List<Matrix4x4>>> BatchMap => _batchMap;
    public List<BlockSpriteKey> ActiveKeys => _activeKeys;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
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
        _keyBuffer = new List<int>();
    }

    private SO_Block GetSO(EBlock id)
    {
        if (_soCache.TryGetValue(id, out SO_Block so))
            return so;
        var database = GameData.ins.BlockDatabase;
        if (!database.TryGetValue(id, out so))
            return null;
        _soCache.Add(id, so);
        return so;
    }

    private Material GetOrCreateMaterial(BlockSpriteKey key, SpriteInfo info)
    {
        if (_materialCache.TryGetValue(key, out Material mat))
            return mat;
        if (De.IsNull(info.sprite))
            return null;
        mat = UGraphic.CreateMaterial(_baseMaterial, info.sprite);
        Color color = mat.color;
        color.a = _alpha;
        mat.color = color;
        _materialCache.Add(key, mat);
        return mat;
    }

    /// <summary>
    /// 매 프레임 designs 딕셔너리를 순회하여 렌더링 배치 데이터를 재구성합니다.
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game)) return;
        if (De.IsNull(game.Player)) return;

        var designs = game.Player.designs;
        int width = game.TileMap.Width;

        UGraphic.ClearBatches(_batchMap, _activeKeys);

        if (designs.Count <= 0)
            return;

        // 딕셔너리 순회용 키 버퍼
        _keyBuffer.Clear();
        foreach (var key in designs.Keys) {
            _keyBuffer.Add(key);
        }

        for (int i = 0; i < _keyBuffer.Count; ++i) {
            int index = _keyBuffer[i];
            BuildOrder order = designs[index];
            if (order.type != EOrderType.Build)
                continue;

            EBlock id = order.id;
            SO_Block so = GetSO(id);
            if (so == null)
                continue;

            // 인덱스를 월드 좌표로 변환 (셀 중심)
            (int gridX, int gridY) = UGrid.IndexToGrid(index, width);
            float baseX = gridX + 0.5f;
            float baseY = gridY + 0.5f;

            // 블록 설치 방향 각도
            float blockAngle = UGraphic.RotateToAngle(order.rotate);

            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                SpriteInfo info = so.GetSpriteInfo(si);
                BlockSpriteKey spriteKey = new BlockSpriteKey(id, si);

                GetOrCreateMaterial(spriteKey, info);

                float posZ = -20f + info.offset.z;

                // 디자인 상태에서는 모든 부품이 블록 방향으로만 회전
                Quaternion rot;
                switch (info.type) {
                    case ESpriteType.Body:
                    case ESpriteType.Turret:
                    case ESpriteType.Rotation:
                        rot = UGraphic.AngleToQuaternion(blockAngle);
                        break;
                    case ESpriteType.Static:
                    default:
                        rot = Quaternion.identity;
                        break;
                }

                // 오프셋을 블록 방향으로 회전
                Vector3 pos = UGraphic.RotateOffset(baseX, baseY, info.offset.x, info.offset.y, blockAngle, posZ);

                // 짝수 크기 블록의 중심 보정
                float sizeX = so.Size.x;
                if (sizeX % 2f <= 0.001f) pos.x -= 0.5f;
                float sizeY = so.Size.y;
                if (sizeY % 2f <= 0.001f) pos.y -= 0.5f;
                Vector3 scale = new Vector3(sizeX, sizeY, 1f);

                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                UGraphic.AddMatrix(_batchMap, _activeKeys, spriteKey, matrix);
            }
        }
    }
    #endregion
}
