using UnityEngine;

/// <summary>
/// 선택한 블록을 디자인 블록(건설 예정)으로 전환하고, 점유맵을 관리합니다.
/// </summary>
public partial class PlayerArchitect
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 디자인 점유맵 (값 = 중심 인덱스, -1 = 비점유)
    private int[] _designOccupied;
    private int[] _placeCellBuffer;
    private int[] _removeCellBuffer;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 디자인 점유맵 및 셀 버퍼 초기화
    /// </summary>
    private void InitializeDesign()
    {
        int length = _width * _height;
        _designOccupied = new int[length];
        for (int i = 0; i < length; ++i)
            _designOccupied[i] = -1;
        _placeCellBuffer = new int[36];
        _removeCellBuffer = new int[36];
    }

    /// <summary>
    /// 현재 selecteds를 커서 위치 기준으로 디자인 블록에 배치합니다.
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

            // 그리드 스냅
            int gridX = Mathf.FloorToInt(cx + sel.offsetX);
            int gridY = Mathf.FloorToInt(cy + sel.offsetY);
            if (gridX < 0 || _width <= gridX || gridY < 0 || _height <= gridY)
                continue;

            int centerIndex = UGrid.GridToIndex(gridX, gridY, _width);

            if (!_blockSO.TryGetValue(sel.id, out SO_Block so))
                continue;
            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);

            // 점유할 셀 계산
            int cellCount = _blockMap.GetOccupiedCells(centerIndex, size, sel.rotation, _placeCellBuffer);
            if (cellCount <= 0)
                continue;

            // 실제 블록 또는 비지상 타일과 겹치면 스킵
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

            // 겹치는 기존 디자인 제거
            for (int c = 0; c < cellCount; ++c) {
                int occupiedBy = _designOccupied[_placeCellBuffer[c]];
                if (occupiedBy != -1)
                    RemoveDesign(occupiedBy);
            }

            // 새 디자인 등록
            _designs[centerIndex] = new BuildOrder(centerIndex, EOrderType.Build, sel.id, sel.rotation);
            for (int c = 0; c < cellCount; ++c)
                _designOccupied[_placeCellBuffer[c]] = centerIndex;
        }
    }

    /// <summary>
    /// 중심 인덱스로 디자인 블록 1개를 제거합니다.
    /// </summary>
    private void RemoveDesign(int centerIndex)
    {
        if (!_designs.TryGetValue(centerIndex, out BuildOrder order))
            return;

        // 점유맵 해제
        if (_blockSO.TryGetValue(order.id, out SO_Block so)) {
            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
            int cellCount = _blockMap.GetOccupiedCells(centerIndex, size, order.rotation, _removeCellBuffer);
            for (int c = 0; c < cellCount; ++c)
                _designOccupied[_removeCellBuffer[c]] = -1;
        }

        _designs.Remove(centerIndex);
    }

    /// <summary>
    /// 모든 디자인 블록을 제거합니다.
    /// </summary>
    public void ClearAllDesigns()
    {
        _designs.Clear();
        int length = _designOccupied.Length;
        for (int i = 0; i < length; ++i)
            _designOccupied[i] = -1;
    }

    /// <summary>
    /// Q키 — 선택 블록 + 디자인 블록 전체 초기화
    /// </summary>
    private void OnBuildReset()
    {
        _selecteds.Clear();
        _stretchOriginal = default;
        ClearAllDesigns();
    }
    #endregion
}
