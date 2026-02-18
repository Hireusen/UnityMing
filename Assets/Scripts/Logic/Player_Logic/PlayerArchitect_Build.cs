using UnityEngine;

/// <summary>
/// 디자인 블록을 자동 건설/철거합니다.
/// buildInterval마다 buildRange 내의 디자인 블록 1개를 처리합니다.
/// </summary>
public partial class PlayerArchitect
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private float _buildTimer;
    private float _lookAtBuildEndTime;
    private Vector2 _lastBuildPos;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 이벤트 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 건설/철거 시 외부에 알림 (center, sizeX, sizeY)
    // BuildEffectPainter, SoundAdmin 등이 구독
    public event System.Action<Vector2, float, float> OnBuildEffect;
    public event System.Action<Vector2, float, float> OnDestroyEffect;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 매 프레임 호출. 쿨타임 관리 및 건설/철거 시도.
    /// </summary>
    private void UpdateBuild()
    {
        if (_designs.Count <= 0) {
            _player.lookAtBuild = false;
            return;
        }

        // 바라보기 시간 만료
        if (_player.lookAtBuild && Time.time >= _lookAtBuildEndTime)
            _player.lookAtBuild = false;

        // Shift 가속
        float speed = Input.GetKey(KeyCode.LeftShift) ? _player.buildSpeedMultiplier : 1f;
        _buildTimer -= Time.deltaTime * speed;

        if (_buildTimer > 0f) {
            if (_player.lookAtBuild)
                ApplyLookAtBuild();
            return;
        }

        // 쿨타임 리셋 및 건설 시도
        _buildTimer = _player.buildInterval;

        if (TryProcessOneDesign()) {
            _player.lookAtBuild = true;
            _lookAtBuildEndTime = Time.time + _player.buildInterval;
        }
    }

    /// <summary>
    /// buildRange 직사각형 내에서 첫 번째 디자인 블록을 처리합니다.
    /// </summary>
    private bool TryProcessOneDesign()
    {
        Vector2 playerPos = (Vector2)_playerTransform.position;
        float half = _player.buildRange * 0.5f;

        int minX = Mathf.Max(0, Mathf.FloorToInt(playerPos.x - half));
        int maxX = Mathf.Min(_width - 1, Mathf.FloorToInt(playerPos.x + half));
        int minY = Mathf.Max(0, Mathf.FloorToInt(playerPos.y - half));
        int maxY = Mathf.Min(_height - 1, Mathf.FloorToInt(playerPos.y + half));

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
                if (order.type == EOrderType.Destroy)
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
        if (!_blockSO.TryGetValue(order.id, out SO_Block so)) {
            RemoveDesign(centerIndex);
            return false;
        }

        float sizeX = so.Size.x;
        float sizeY = so.Size.y;

        if (!_blockMap.TryPlace(centerIndex, order.id, order.rotation)) {
            RemoveDesign(centerIndex);
            return false;
        }

        RemoveDesign(centerIndex);
        Vector2Int size = new Vector2Int((int)sizeX, (int)sizeY);
        _lastBuildPos = _blockMap.GetRenderPos(centerIndex, size, order.rotation);
        OnBuildEffect?.Invoke(_lastBuildPos, sizeX, sizeY);
        return true;
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
        return true;
    }

    /// <summary>
    /// 건설/철거 대상 방향으로 플레이어를 회전합니다.
    /// </summary>
    private void ApplyLookAtBuild()
    {
        Vector2 dir = _lastBuildPos - (Vector2)_playerTransform.position;
        if (dir.sqrMagnitude < 0.001f)
            return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion target = Quaternion.Euler(0f, 0f, angle - 90f);
        _playerTransform.rotation = Quaternion.Lerp(_playerTransform.rotation, target, 0.15f);
    }
    #endregion
}
