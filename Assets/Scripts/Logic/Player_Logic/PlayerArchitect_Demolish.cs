using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 우클릭 철거 로직.
/// 디자인 블록(Build) → 즉시 삭제, 실제 블록 → Destroy 디자인 등록.
/// </summary>
public partial class PlayerArchitect
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 (철거 이벤트) 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 우클릭 누른 순간 — 선택 블록이 있으면 취소 처리
    /// </summary>
    private void OnDemolishNow(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (_selecteds.Count <= 0)
            return;
        _dragCopy = false;
        _dragPlacer = false;
        _selecteds.Clear();
        _stretchOriginal = default;
    }

    /// <summary>
    /// 우클릭 단일 — 해당 셀 처리
    /// </summary>
    private void OnDemolishOnce(Vector2 pos)
    {
        _player.demolishActive = false;
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (_selecteds.Count > 0)
            return;

        Vector2 grid = UGrid.WorldToGrid(pos);
        int gridX = (int)grid.x;
        int gridY = (int)grid.y;
        if (gridX < 0 || _width <= gridX || gridY < 0 || _height <= gridY)
            return;

        DemolishCell(UGrid.GridToIndex(gridX, gridY, _width));
    }

    /// <summary>
    /// 우클릭 드래그 중 — 영역 시각화 갱신
    /// </summary>
    private void OnDemolishDrag(Vector2 startPos, Vector2 endPos)
    {
        if (_selecteds.Count > 0)
            return;

        (int minX, int minY, int maxX, int maxY) = UGrid.GetForeachPos(startPos, endPos);
        _player.demolishMinX = minX;
        _player.demolishMinY = minY;
        _player.demolishMaxX = maxX;
        _player.demolishMaxY = maxY;
        _player.demolishActive = true;
    }

    /// <summary>
    /// 우클릭 드래그 끝 — 영역 내 모든 셀 철거
    /// </summary>
    private void OnDemolishDragEnd(Vector2 startPos, Vector2 endPos)
    {
        _player.demolishActive = false;
        if (_selecteds.Count > 0)
            return;

        (int minX, int minY, int maxX, int maxY) = UGrid.GetForeachPos(startPos, endPos);
        for (int y = minY; y <= maxY; ++y) {
            for (int x = minX; x <= maxX; ++x) {
                if (x < 0 || _width <= x || y < 0 || _height <= y)
                    continue;
                DemolishCell(UGrid.GridToIndex(x, y, _width));
            }
        }
    }

    /// <summary>
    /// 셀 단위 철거. Build 디자인 → 즉시 삭제, 실제 블록 → Destroy 등록.
    /// </summary>
    private void DemolishCell(int index)
    {
        // 디자인 블록 확인
        int occupiedBy = _designOccupied[index];
        if (occupiedBy != -1) {
            if (_designs.TryGetValue(occupiedBy, out BuildOrder existing)) {
                if (existing.type == EOrderType.Build)
                    RemoveDesign(occupiedBy);
                // Destroy 디자인은 이미 등록됨
                return;
            }
        }

        // 실제 블록 확인
        if (!_blockMap.InMap(index) || _blockMap.IsVoid(index))
            return;

        int adress = _blockMap.GetAdress(index);
        ref readonly BlockSingle block = ref _blockPool.Read(adress);
        int centerIndex = block.index;

        // 중복 등록 방지
        if (_designs.ContainsKey(centerIndex))
            return;

        // Destroy 디자인 등록
        _designs[centerIndex] = new BuildOrder(centerIndex, EOrderType.Destroy, block.id, block.rotation);

        // 점유맵 등록
        if (_blockSO.TryGetValue(block.id, out SO_Block so)) {
            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
            int cellCount = _blockMap.GetOccupiedCells(centerIndex, size, block.rotation, _placeCellBuffer);
            for (int c = 0; c < cellCount; ++c)
                _designOccupied[_placeCellBuffer[c]] = centerIndex;
        }
    }
    #endregion
}
