using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 블록 그래픽 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 PoolManagement&lt;BlockSingle&gt;의 활성 블록만 순회하여
/// 렌더링 배치 데이터를 구성합니다.
///
/// Body/Turret/Rotation/Static → BatchRegistry 배치 렌더링
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
    private BatchRegistry _registry;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_mesh);
        De.IsNull(_baseMaterial);
    }

    public void Initialize()
    {
        _registry = new BatchRegistry(_mesh, _baseMaterial);
    }

    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (De.IsNull(game)) return;

        PoolManagement<BlockSingle> pool = game.BlockPool;
        BlockMap blockMap = game.Blocks;
        int capacity = pool.Capacity;

        _registry.ClearAll();

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

            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                SpriteInfo info = so.GetSpriteInfo(si);

                float posZ = Const.WORLD_BLOCK + info.offset.z;
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
                    case ESpriteType.Static:
                    default:
                        rot = UGraphic.AngleToQuaternion(blockAngle);
                        pos = UGraphic.RotateOffset(baseX, baseY, offsetX, offsetY, blockAngle, posZ);
                        break;
                }

                BlockSpriteKey key = new BlockSpriteKey(block.id, si);
                int slot = _registry.GetOrCreateSlot(key, info.sprite);
                Matrix4x4 matrix = UGraphic.BuildMatrix(pos, rot, scale);
                _registry.Add(slot, in matrix);
            }
        }

        _registry.DrawAll();
    }
    #endregion
}
