using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 블록 그래픽 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 PoolManagement&lt;BlockSingle&gt;의 활성 블록만 순회하여
/// 렌더링 배치 데이터를 구성합니다.
///
/// Body/Turret/Rotation/Static → BatchLists 배치 렌더링
/// Effect → 인스턴스별 투명도가 필요하므로 Graphics.DrawMesh + MaterialPropertyBlock
/// </summary>
public class BlockBatchBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _baseMaterial;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private Dictionary<BlockSpriteKey, int> _keyToSlot;
    private List<BatchLists> _batches;
    private int _batchCount;

    // Effect 전용
    private Dictionary<BlockSpriteKey, Material> _effectMaterials;
    private MaterialPropertyBlock _effectMpb;
    private static readonly int _colorID = Shader.PropertyToID("_Color");
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
        _effectMaterials = new Dictionary<BlockSpriteKey, Material>();
        _effectMpb = new MaterialPropertyBlock();
    }

    private int GetOrCreateSlot(BlockSpriteKey key, Sprite sprite)
    {
        if (_keyToSlot.TryGetValue(key, out int slot))
            return slot;
        Material mat = UGraphic.CreateMaterial(_baseMaterial, sprite);
        slot = _batches.Count;
        _batches.Add(new BatchLists(mat));
        _keyToSlot.Add(key, slot);
        _batchCount = _batches.Count;
        return slot;
    }

    private Material GetOrCreateEffectMaterial(BlockSpriteKey key, Sprite sprite)
    {
        if (_effectMaterials.TryGetValue(key, out Material mat))
            return mat;
        mat = UGraphic.CreateMaterial(_baseMaterial, sprite);
        mat.SetTexture("_BaseMap", sprite.texture);
        _effectMaterials.Add(key, mat);
        return mat;
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

        PoolManagement<BlockSingle> pool = game.BlockPool;
        BlockMap blockMap = game.Blocks;
        int capacity = pool.Capacity;

        ClearAllBatches();

        for (int adress = 0; adress < capacity; ++adress) {
            if (!pool.IsExist(adress))
                continue;

            ref readonly BlockSingle block = ref pool.Read(adress);
            if (block.IsVoid())
                continue;

            if (!game.BlockDatabase.TryGetValue(block.id, out SO_Block so))
                continue;

            Vector2 renderPos = blockMap.GetRenderPos(block.index, block.size, block.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;

            float blockAngle = UGrid.RotationToAngle(block.rotation);
            float bodyAngle = block.partAngle_1;
            float turretAngle = block.partAngle_2;
            float rotationAngle = block.partAngle_3;

            Vector3 scale = new Vector3(so.Size.x, so.Size.y, 1f);

            // 이펙트 투명도 계산 (삼각파: 0→1→0)
            float effectAlpha = 0f;
            float cycle = so.EffectCycleTime;
            if (cycle > 0f) {
                float t = block.effectTimer / cycle; // 0 ~ 2
                effectAlpha = (t <= 1f) ? t : 2f - t;
            }

            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                SpriteInfo info = so.GetSpriteInfo(si);

                float posZ = -30f + info.offset.z;
                float offsetX = info.offset.x;
                float offsetY = info.offset.y;

                Quaternion rot;
                Vector3 pos;

                switch (info.type) {
                    case ESpriteType.Body:
                        rot = UGraphic.AngleToQuaternion(bodyAngle);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, bodyAngle, posZ);
                        break;
                    case ESpriteType.Turret:
                        rot = UGraphic.AngleToQuaternion(turretAngle);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, turretAngle, posZ);
                        break;
                    case ESpriteType.Rotation:
                        rot = UGraphic.AngleToQuaternion(rotationAngle);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, rotationAngle, posZ);
                        break;
                    case ESpriteType.Effect:
                        // Effect는 개별 DrawMesh로 처리 (아래 별도)
                        DrawEffect(block, so, info, si, baseX, baseY, blockAngle, scale, effectAlpha);
                        continue; // 배치에 추가하지 않음
                    case ESpriteType.Static:
                    default:
                        rot = UGraphic.AngleToQuaternion(blockAngle);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, blockAngle, posZ);
                        break;
                }

                BlockSpriteKey key = new BlockSpriteKey(block.id, si);
                int slot = GetOrCreateSlot(key, info.sprite);
                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                AddMatrix(slot, matrix);
            }
        }

        // 배치 그리기 (Body, Turret, Rotation, Static)
        for (int i = 0; i < _batchCount; ++i) {
            Material mat = _batches[i].Mat;
            List<List<Matrix4x4>> matrices = _batches[i].matrices;
            for (int j = 0; j < matrices.Count; ++j) {
                if (matrices[j].Count <= 0) continue;
                Graphics.DrawMeshInstanced(_mesh, 0, mat, matrices[j]);
            }
        }
    }

    /// <summary>
    /// Effect 스프라이트를 개별 투명도로 렌더링합니다.
    /// </summary>
    private void DrawEffect(
        in BlockSingle block, SO_Block so, in SpriteInfo info, int spriteIndex,
        float baseX, float baseY, float blockAngle, Vector3 scale, float alpha)
    {
        float posZ = -30f + info.offset.z;
        Quaternion rot = UGraphic.AngleToQuaternion(blockAngle);
        Vector3 pos = UGraphic.RotateOffset(baseX, baseY, info.offset.x, info.offset.y, blockAngle, posZ);
        Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);

        BlockSpriteKey key = new BlockSpriteKey(block.id, spriteIndex);
        Material mat = GetOrCreateEffectMaterial(key, info.sprite);

        _effectMpb.SetColor(_colorID, new Color(1f, 1f, 1f, alpha));
        Graphics.DrawMesh(_mesh, matrix, mat, 0, null, 0, _effectMpb);
    }
    #endregion
}
