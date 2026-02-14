using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 우클릭 철거
/// - 디자인 블록(Build) → 즉시 삭제
/// - 실제 블록 → designs에 Destroy로 등록 (플레이어가 자동 철거)
/// - 빈 셀 → 무시
/// </summary>
public partial class PlayerArchitect
{
    // ================================================================
    //  철거 이벤트 핸들러
    // ================================================================

    private void OnDemolishNow(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (_selecteds.Count > 0) {
            _dragCopy = false;
            _dragPlacer = false;
            _selecteds.Clear();
            _stretchOriginal = default;
        }
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
        int gx = (int)grid.x;
        int gy = (int)grid.y;
        if (gx < 0 || _width <= gx || gy < 0 || _height <= gy)
            return;
        int index = UGrid.GridToIndex(gx, gy, _width);

        DemolishCell(index);
    }

    /// <summary>
    /// 우클릭 드래그 중 — 빨간 사각형 시각화
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
    /// 우클릭 드래그 끝 — 영역 내 모든 셀 처리
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
                int index = UGrid.GridToIndex(x, y, _width);
                DemolishCell(index);
            }
        }
    }

    // ================================================================
    //  셀 단위 철거 처리
    // ================================================================

    /// <summary>
    /// 1개 셀에 대해:
    /// - 디자인 블록(Build)이 있으면 즉시 삭제
    /// - 실제 블록이 있으면 Destroy 디자인으로 등록
    /// - 이미 Destroy 디자인이 등록된 셀은 무시
    /// </summary>
    private void DemolishCell(int index)
    {
        // 1) 디자인 블록 확인
        int occupiedBy = _designOccupied[index];
        if (occupiedBy != -1) {
            if (_designs.TryGetValue(occupiedBy, out BuildOrder existing)) {
                if (existing.type == EOrderType.Build) {
                    // Build 디자인 즉시 삭제
                    RemoveDesign(occupiedBy);
                    return;
                }
                // Destroy 디자인은 이미 등록됨 → 무시
                return;
            }
        }

        // 2) 실제 블록 확인
        if (!_blockMap.InMap(index) || _blockMap.IsVoid(index))
            return;

        // 실제 블록의 중심 인덱스 찾기 (대형 블록은 중심이 다른 셀)
        int adress = _blockMap.GetAdress(index);
        ref readonly BlockSingle block = ref _blockPool.Read(adress);
        int centerIndex = block.index;

        // 이미 Destroy 디자인이 등록되어 있으면 무시
        if (_designs.ContainsKey(centerIndex))
            return;

        // Destroy 디자인 등록
        BuildOrder order = new BuildOrder(centerIndex, EOrderType.Destroy, block.id, block.rotation);
        _designs[centerIndex] = order;

        // 점유맵에 등록 (블록이 점유하는 모든 셀)
        if (_blockSO.TryGetValue(block.id, out SO_Block so)) {
            Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
            int cellCount = _blockMap.GetOccupiedCells(centerIndex, size, block.rotation, _placeCellBuffer);
            for (int c = 0; c < cellCount; ++c)
                _designOccupied[_placeCellBuffer[c]] = centerIndex;
        }
    }
}
