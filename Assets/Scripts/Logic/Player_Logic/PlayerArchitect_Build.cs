using UnityEngine;

/// <summary>
/// 디자인 블록을 플레이어 로직에 따라 자동 건설/철거하기
///
/// _buildInterval마다 buildRange 직사각형 내의 디자인 블록 1개를 처리합니다.
/// Build → 실제 블록 배치, Destroy → 실제 블록 제거
/// 건설/철거 중에는 _player.lookAtBuild = true → PlayerMover가 이동 방향 회전을 스킵합니다.
/// </summary>
public partial class PlayerArchitect
{
    // ================================================================
    //  내부 변수
    // ================================================================
    private float _buildTimer;
    private float _lookAtBuildEndTime;
    private Vector2 _lastBuildPos;

    // 건설/철거 이펙트 콜백 (center, sizeX, sizeY)
    public static event System.Action<Vector2, float, float> OnBuildEffect;
    public static event System.Action<Vector2, float, float> OnDestroyEffect;

    // ================================================================
    //  메인 루프 (RunBeforeFrame에서 호출)
    // ================================================================

    private void UpdateBuild()
    {
        if (_designs.Count <= 0) {
            _player.lookAtBuild = false;
            return;
        }

        // 바라보기 시간 만료 확인
        if (_player.lookAtBuild && Time.time >= _lookAtBuildEndTime)
            _player.lookAtBuild = false;

        _buildTimer -= Time.deltaTime;
        if (_buildTimer > 0f) {
            // 쿨타임 중: 건설 방향 바라보기 유지
            if (_player.lookAtBuild)
                ApplyLookAtBuild();
            return;
        }

        // 건설/철거 시도
        _buildTimer = _player.buildInterval;

        if (TryProcessOneDesign()) {
            // 성공: 바라보기 상태 시작 (다음 빌드 인터벌 동안 유지)
            _player.lookAtBuild = true;
            _lookAtBuildEndTime = Time.time + _player.buildInterval;
        }
    }

    // ================================================================
    //  건설/철거 처리
    // ================================================================

    /// <summary>
    /// buildRange 직사각형 내에서 첫 번째로 발견되는 디자인 블록을 처리합니다.
    /// </summary>
    private bool TryProcessOneDesign()
    {
        Vector2 playerPos = (Vector2)_playerTransform.position;
        float half = _player.buildRange * 0.5f;

        int minX = Mathf.FloorToInt(playerPos.x - half);
        int maxX = Mathf.FloorToInt(playerPos.x + half);
        int minY = Mathf.FloorToInt(playerPos.y - half);
        int maxY = Mathf.FloorToInt(playerPos.y + half);

        if (minX < 0) minX = 0;
        if (minY < 0) minY = 0;
        if (maxX >= _width) maxX = _width - 1;
        if (maxY >= _height) maxY = _height - 1;

        for (int y = minY; y <= maxY; ++y) {
            for (int x = minX; x <= maxX; ++x) {
                int index = UGrid.GridToIndex(x, y, _width);
                int designCenter = _designOccupied[index];
                if (designCenter == -1)
                    continue;
                if (!_designs.TryGetValue(designCenter, out BuildOrder order))
                    continue;

                if (order.type == EOrderType.Build)
                    return TryExecuteBuild(designCenter, order);
                else if (order.type == EOrderType.Destroy)
                    return TryExecuteDestroy(designCenter, order);
            }
        }
        return false;
    }

    /// <summary>
    /// Build 디자인 → 실제 블록 배치
    /// </summary>
    private bool TryExecuteBuild(int centerIndex, BuildOrder order)
    {
        float sizeX = 1f;
        float sizeY = 1f;
        if (_blockSO.TryGetValue(order.id, out SO_Block so)) {
            sizeX = so.Size.x;
            sizeY = so.Size.y;
        }

        if (_blockMap.TryPlace(centerIndex, order.id, order.rotation)) {
            RemoveDesign(centerIndex);
            Vector2Int size = new Vector2Int((int)sizeX, (int)sizeY);
            _lastBuildPos = _blockMap.GetRenderPos(centerIndex, size, order.rotation);
            OnBuildEffect?.Invoke(_lastBuildPos, sizeX, sizeY);
            // 소리 재생
            if (size.x <= 1) {
                _game.SoundAdmin.PlaySound(ESound.BlockBuild_1);
            } else if (size.x <= 2) {
                _game.SoundAdmin.PlaySound(ESound.BlockBuild_2);
            }  else {
                _game.SoundAdmin.PlaySound(ESound.BlockBuild_3);
            }
            return true;
        } else {
            RemoveDesign(centerIndex);
            return false;
        }
    }

    /// <summary>
    /// Destroy 디자인 → 실제 블록 철거
    /// </summary>
    private bool TryExecuteDestroy(int centerIndex, BuildOrder order)
    {
        if (!_blockMap.InMap(centerIndex) || _blockMap.IsVoid(centerIndex)) {
            RemoveDesign(centerIndex);
            return false;
        }

        int adress = _blockMap.GetAdress(centerIndex);
        ref readonly BlockSingle block = ref _blockPool.Read(adress);
        float sizeX = block.size.x;
        float sizeY = block.size.y;
        Vector2 renderPos = _blockMap.GetRenderPos(block.index, block.size, block.rotation);

        _blockMap.TryRemove(centerIndex);
        RemoveDesign(centerIndex);

        _lastBuildPos = renderPos;
        OnDestroyEffect?.Invoke(renderPos, sizeX, sizeY);
        // 소리 재생
        if (sizeX <= 1f) {
            _game.SoundAdmin.PlaySound(ESound.BlockDestory_1);
        } else if (sizeX <= 2f) {
            _game.SoundAdmin.PlaySound(ESound.BlockDestory_2);
        } else {
            _game.SoundAdmin.PlaySound(ESound.BlockDestory_3);
        }
        return true;
    }

    // ================================================================
    //  건설/철거 방향 바라보기
    // ================================================================

    private void ApplyLookAtBuild()
    {
        Vector2 playerPos = (Vector2)_playerTransform.position;
        Vector2 dir = _lastBuildPos - playerPos;
        if (dir.sqrMagnitude < 0.001f)
            return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion target = Quaternion.Euler(0f, 0f, angle - 90f);
        _playerTransform.rotation = Quaternion.Lerp(_playerTransform.rotation, target, 0.15f);
    }
}
