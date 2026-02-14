using UnityEngine;

/// <summary>
/// 선택한 블록을 디자인 블록(건설 예정)으로 전환하기
///
/// _designOccupied: 디자인 점유맵 (맵 크기 배열)
///   값 = 해당 셀을 점유한 디자인 블록의 중심 인덱스
///   -1 = 비점유
/// </summary>
public partial class PlayerArchitect
{
    // ================================================================
    //  내부 변수
    // ================================================================
    private int[] _designOccupied;      // 디자인 점유맵 (길이 = width * height)
    private int[] _placeCellBuffer;     // TrySelectedToDesign 전용 버퍼
    private int[] _removeCellBuffer;    // RemoveDesign 전용 버퍼

    // ================================================================
    //  초기화 (PlayerArchitect.Initialize()에서 호출)
    // ================================================================
    public void InitializeDesign()
    {
        int length = _width * _height;
        _designOccupied = new int[length];
        for (int i = 0; i < length; ++i)
            _designOccupied[i] = -1;
        _placeCellBuffer = new int[36];  // 최대 6×6
        _removeCellBuffer = new int[36];
    }

    // ================================================================
    //  Selected → Design 전환
    // ================================================================

    /// <summary>
    /// 현재 selecteds의 블록들을 디자인 블록으로 월드에 배치합니다.
    /// SelectedBatchBuilder와 동일한 좌표 동기화를 사용합니다.
    /// </summary>
    private void TrySelectedToDesign()
    {
        int selCount = _selecteds.Count;
        if (selCount <= 0)
            return;

        Vector2 cursor = _player.CursorInGrid;
        float cx = cursor.x;
        float cy = cursor.y;
        TileMap tile = _game.TileMap;

        for (int i = 0; i < selCount; ++i) {
            SelectedBlock sel = _selecteds[i];
            if (sel.id == EBlock.None)
                continue;

            // SelectedBatchBuilder와 동일한 스냅
            int gx = Mathf.FloorToInt(cx + sel.offsetX);
            int gy = Mathf.FloorToInt(cy + sel.offsetY);

            // 2차원 범위 검사
            if (gx < 0 || _width <= gx || gy < 0 || _height <= gy)
                continue;

            int centerIndex = UGrid.GridToIndex(gx, gy, _width);

            // SO에서 블록 크기
            if (!_blockSO.TryGetValue(sel.id, out SO_Block so))
                continue;
            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);

            // 이 블록이 점유할 모든 셀 계산 (_placeCellBuffer 사용)
            int cellCount = _blockMap.GetOccupiedCells(centerIndex, size, sel.rotation, _placeCellBuffer);
            if (cellCount <= 0)
                continue;

            // ──────────────────────────────────
            // 검사: 실제 블록 또는 비지상 타일과 겹치면 스킵
            // ──────────────────────────────────
            bool blocked = false;
            for (int c = 0; c < cellCount; ++c) {
                int cell = _placeCellBuffer[c];
                if (_blockMap.IsExist(cell) || !tile.IsGround(cell)) {
                    blocked = true;
                    break;
                }
            }
            if (blocked)
                continue;

            // ──────────────────────────────────
            // 겹치는 기존 디자인 제거
            // RemoveDesign은 _removeCellBuffer를 사용하므로 _placeCellBuffer 안전
            // 같은 centerIndex라도 다른 블록이면 제거 필요 (크기/회전이 다를 수 있음)
            // ──────────────────────────────────
            for (int c = 0; c < cellCount; ++c) {
                int occupiedBy = _designOccupied[_placeCellBuffer[c]];
                if (occupiedBy != -1)
                    RemoveDesign(occupiedBy);
            }

            // ──────────────────────────────────
            // 새 디자인 등록
            // ──────────────────────────────────
            BuildOrder order = new BuildOrder(centerIndex, EOrderType.Build, sel.id, sel.rotation);
            _designs[centerIndex] = order;

            // 점유맵 기록
            for (int c = 0; c < cellCount; ++c)
                _designOccupied[_placeCellBuffer[c]] = centerIndex;
        }
    }

    // ================================================================
    //  디자인 제거
    // ================================================================

    /// <summary>
    /// 중심 인덱스로 디자인 블록 1개를 제거합니다.
    /// _removeCellBuffer를 사용하므로 호출자의 _placeCellBuffer를 오염시키지 않습니다.
    /// </summary>
    private void RemoveDesign(int centerIndex)
    {
        if (!_designs.TryGetValue(centerIndex, out BuildOrder order))
            return;

        // 점유맵에서 해제
        if (_blockSO.TryGetValue(order.id, out SO_Block so)) {
            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
            int cellCount = _blockMap.GetOccupiedCells(centerIndex, size, order.rotation, _removeCellBuffer);
            for (int c = 0; c < cellCount; ++c)
                _designOccupied[_removeCellBuffer[c]] = -1;
        }

        _designs.Remove(centerIndex);
    }

    /// <summary>
    /// 디자인 블록을 모두 제거합니다.
    /// </summary>
    public void ClearAllDesigns()
    {
        _designs.Clear();
        int length = _designOccupied.Length;
        for (int i = 0; i < length; ++i)
            _designOccupied[i] = -1;
    }

    /// <summary>
    /// Q키 — 모든 디자인 블록 + 선택 블록 초기화
    /// </summary>
    private void OnBuildReset()
    {
        _selecteds.Clear();
        _stretchOriginal = default;
        ClearAllDesigns();
    }
}
