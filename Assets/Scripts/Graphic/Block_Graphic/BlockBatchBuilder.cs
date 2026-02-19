using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 블록 그래픽 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 렌더링 배치 데이터를 구성하고 관리합니다.
/// </summary>
public class BlockBatchBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _baseMaterial;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private BatchManagement _registry;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_mesh);
        De.IsNull(_baseMaterial);
    }

    public void Initialize()
    {
        _registry = new BatchManagement(_mesh, _baseMaterial);
    }

    public void RunAfterFrame()
    {
        // 캐싱
        GameData game = GameData.ins;
        PoolManagement<BlockSingle> pool = game.BlockPool;
        BlockMap blockMap = game.Blocks;
        int capacity = pool.Capacity;
        // 블록 렌더링 배치 새로 생성
        _registry.ClearAll();
        for (int adress = 0; adress < capacity; ++adress) {
            // 방어 코드
            if (!pool.IsExist(adress))
                continue;
            ref readonly BlockSingle block = ref pool.Read(adress);
            if (block.IsVoid())
                continue;
            if (!game.BlockDatabase.TryGetValue(block.id, out SO_Block so))
                continue;
            // 변수 준비
            Vector2 renderPos = blockMap.GetRenderPos(block.index, block.size, block.rotation);
            float baseX = renderPos.x;
            float baseY = renderPos.y;
            float blockAngle = UGrid.RotationToAngle(block.rotation);
            float bodyAngle = block.partAngle_1;
            float turretAngle = block.partAngle_2;
            float rotationAngle = block.partAngle_3;
            Vector3 scale = new Vector3(so.Size.x, so.Size.y, 1f);
            // 블록 내 부품 그래픽 순환
            int spriteCount = so.SpriteCount;
            for (int si = 0; si < spriteCount; ++si) {
                // 변수 준비
                SpriteInfo info = so.GetSpriteInfo(si);
                float posZ = Const.WORLD_BLOCK + info.offset.z;
                float offsetX = info.offset.x;
                float offsetY = info.offset.y;
                Quaternion rot;
                Vector3 pos;
                // 타입에 따른 좌표와 회전
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
                // 고유 키 생성, 배치 생성
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
