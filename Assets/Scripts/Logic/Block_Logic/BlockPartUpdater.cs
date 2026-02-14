using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 블록 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 매 프레임 모든 활성 블록의 부품 각도와 이펙트 타이머를 업데이트합니다.
///
/// partAngle_1 : Body — 지수 보간으로 플레이어를 바라봄 (가까울수록 감속)
/// partAngle_2 : Turret — 일정 속도로 플레이어를 향해 최단 경로 회전
/// partAngle_3 : Rotation — 일정 속도로 자전
/// effectTimer : Effect — 삼각파 타이머 (투명도 계산용)
/// </summary>
public class BlockPartUpdater : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Transform _playerTransform;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private PoolManagement<BlockSingle> _pool;
    private BlockMap _blockMap;
    private Dictionary<EBlock, SO_Block> _blockSO;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_playerTransform);
    }

    public void Initialize()
    {
        GameData game = GameData.ins;
        _pool = game.BlockPool;
        _blockMap = game.Blocks;
        _blockSO = game.BlockDatabase;
    }

    public void RunBeforeFrame()
    {
        float dt = Time.deltaTime;
        float playerX = _playerTransform.position.x;
        float playerY = _playerTransform.position.y;
        int capacity = _pool.Capacity;

        for (int adress = 0; adress < capacity; ++adress) {
            if (!_pool.IsExist(adress))
                continue;

            ref BlockSingle block = ref _pool.GetRef(adress);
            if (block.IsVoid())
                continue;

            if (!_blockSO.TryGetValue(block.id, out SO_Block so))
                continue;

            // 블록 렌더링 중심
            Vector2 renderPos = _blockMap.GetRenderPos(block.index, block.size, block.rotation);
            float dx = playerX - renderPos.x;
            float dy = playerY - renderPos.y;
            float targetAngle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg - 90f;

            // ── partAngle_1 : Body (지수 보간) ──
            float bodyDelta = Mathf.DeltaAngle(block.partAngle_1, targetAngle);
            block.partAngle_1 += bodyDelta * Mathf.Clamp01(so.BodyRotateSpeed * dt);

            // ── partAngle_2 : Turret (일정 속도, 최단 경로) ──
            float turretDelta = Mathf.DeltaAngle(block.partAngle_2, targetAngle);
            float maxStep = so.TurretRotateSpeed * dt;
            if (Mathf.Abs(turretDelta) <= maxStep)
                block.partAngle_2 = targetAngle;
            else
                block.partAngle_2 += Mathf.Sign(turretDelta) * maxStep;

            // ── partAngle_3 : Rotation (자전) ──
            block.partAngle_3 += so.RotationSpeed * dt;
            if (block.partAngle_3 > 360f) block.partAngle_3 -= 360f;
            else if (block.partAngle_3 < -360f) block.partAngle_3 += 360f;

            // ── effectTimer : Effect (삼각파 누적) ──
            float fullCycle = so.EffectCycleTime * 2f;
            block.effectTimer += dt;
            if (block.effectTimer >= fullCycle)
                block.effectTimer -= fullCycle;
        }
    }
    #endregion
}
