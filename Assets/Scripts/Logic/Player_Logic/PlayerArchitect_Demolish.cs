using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 디자인 블록 철거
/// 우클릭 단일: selected 취소 또는 해당 셀의 디자인 블록 1개 제거
/// 우클릭 드래그: 영역 내 모든 디자인 블록 제거 + 빨간 사각형 시각화
/// </summary>
public partial class PlayerArchitect
{
    // ================================================================
    //  철거 이벤트 핸들러
    // ================================================================

    /// <summary>
    /// 우클릭 즉시 (KeyDown)
    /// selected가 있으면 취소, 없으면 아무것도 하지 않음
    /// (드래그 판정은 PlayerInput이 담당)
    /// </summary>
    private void OnDemolishNow(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        // selected 블록이 있으면 취소만 수행
        if (_selecteds.Count > 0) {
            _dragCopy = false;
            _dragPlacer = false;
            _selecteds.Clear();
            _stretchOriginal = default;
        }
    }

    /// <summary>
    /// 우클릭 단일 (클릭 후 드래그 없이 놓기) — 해당 셀의 디자인 블록 제거
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

        // 점유맵에서 디자인 블록 찾기
        int occupiedBy = _designOccupied[index];
        if (occupiedBy != -1)
            RemoveDesign(occupiedBy);
    }

    /// <summary>
    /// 우클릭 드래그 중 — 철거 영역 시각화 갱신
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
    /// 우클릭 드래그 끝 — 영역 내 모든 디자인 블록 제거
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
                int occupiedBy = _designOccupied[index];
                if (occupiedBy != -1)
                    RemoveDesign(occupiedBy);
            }
        }
    }
}
