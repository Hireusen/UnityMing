using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 selecteds(List)를 순회하여 반투명 블록의 배치 데이터를 구성합니다.
/// </summary>
public class SelectedBatchBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Material _baseMaterial;

    [Header("사용자 정의 설정")]
    [SerializeField, Range(0.1f, 1f)] private float _alpha = 0.5f;
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
    /// selected 전용 렌더링 좌표 계산.
    /// BlockMap.GetRenderPos는 인덱스 기반이라 맵 밖에서 사용할 수 없으므로,
    /// 그리드 좌표(float)로부터 직접 계산합니다.
    /// </summary>
    private static Vector2 GetSelectedRenderPos(float gx, float gy, Vector2Int size, ERotation rotation)
    {
        int sizeX = size.x;
        int sizeY = size.y;
        switch (rotation) {
            case ERotation.Up:
                return new Vector2(gx + sizeX * 0.5f, gy + sizeY * 0.5f);
            case ERotation.Right:
                return new Vector2(gx + sizeY * 0.5f, gy - sizeX * 0.5f);
            case ERotation.Down:
                return new Vector2(gx - sizeX * 0.5f, gy - sizeY * 0.5f);
            case ERotation.Left:
                return new Vector2(gx - sizeY * 0.5f, gy + sizeX * 0.5f);
            default:
                return new Vector2(gx + sizeX * 0.5f, gy + sizeY * 0.5f);
        }
    }

    /// <summary>
    /// 매 프레임 selecteds를 순회하여 렌더링 배치 데이터를 재구성합니다.
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game)) return;
        if (De.IsNull(game.Player)) return;

        List<SelectedBlock> selecteds = game.Player.selecteds;
        int count = selecteds.Count;

        UGraphic.ClearBatches(_batchMap, _activeKeys);

        if (count <= 0)
            return;

        Vector2 cursor = game.Player.CursorInGrid;
        float cursorX = cursor.x;
        float cursorY = cursor.y;

        // 드래그 타일링에서는 모든 selected가 같은 id → SO 1번만 조회
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

            // SO 캐싱 (같은 id가 연속되면 Dictionary 조회 스킵)
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

            // 그리드 좌표 (정수로 스냅)
            float gx = Mathf.Floor(cursorX + sel.offsetX);
            float gy = Mathf.Floor(cursorY + sel.offsetY);

            // 렌더링 중심 (맵 밖이어도 계산 가능)
            Vector2 renderPos = GetSelectedRenderPos(gx, gy, lastSize, sel.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;
            float blockAngle = UGrid.RotationToAngle(sel.rotation);

            for (int si = 0; si < lastSpriteCount; ++si) {
                SpriteInfo info = lastSO.GetSpriteInfo(si);
                BlockSpriteKey spriteKey = new BlockSpriteKey(sel.id, si);

                GetOrCreateMaterial(spriteKey, info.sprite);

                float posZ = -31f + info.offset.z;

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
                Vector3 scale = new Vector3(lastSizeX, lastSizeY, 1f);
                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                UGraphic.AddMatrix(_batchMap, _activeKeys, spriteKey, matrix);
            }
        }
    }
    #endregion
}
