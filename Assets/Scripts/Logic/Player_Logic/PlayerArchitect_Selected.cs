using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 블록 선택 · 복사 · 회전 · 드래그 배치
/// List 기반 — 매 프레임 GC 할당 없음
/// </summary>
public partial class PlayerArchitect
{
    // ================================================================
    //  복사: 기존 블록 → Selected
    // ================================================================

    /// <summary>
    /// 해당 좌표에 존재하는 블록 1개를 selected로 복사합니다.
    /// </summary>
    private void TryCopySingle(int index, float offsetX, float offsetY)
    {
        if (!_blockMap.InMap(index))
            return;
        if (_blockMap.IsVoid(index))
            return;
        int adress = _blockMap.GetAdress(index);
        ref readonly BlockSingle block = ref _blockPool.Read(adress);
        _selecteds.Add(new SelectedBlock(offsetX, offsetY, block.id, block.rotation));
    }

    /// <summary>
    /// 두 좌표 사이의 모든 블록을 selected로 복사합니다.
    /// 큰 블록의 중복 등록을 방지하기 위해 adress 기반 검사를 합니다.
    /// </summary>
    private void TryCopyRange(Vector2 startPos, Vector2 endPos)
    {
        (int minX, int minY, int maxX, int maxY) = UGrid.GetForeachPos(startPos, endPos);
        float centerX = (minX + maxX) * 0.5f;
        float centerY = (minY + maxY) * 0.5f;
        _copyAdressSet.Clear();
        for (int y = minY; y <= maxY; ++y) {
            for (int x = minX; x <= maxX; ++x) {
                int index = UGrid.GridToIndex(x, y, _width);
                if (!_blockMap.InMap(index))
                    continue;
                if (_blockMap.IsVoid(index))
                    continue;
                int adress = _blockMap.GetAdress(index);
                if (!_copyAdressSet.Add(adress))
                    continue;
                ref readonly BlockSingle block = ref _blockPool.Read(adress);
                float ox = centerX - x;
                float oy = centerY - y;
                _selecteds.Add(new SelectedBlock(ox, oy, block.id, block.rotation));
            }
        }
    }

    // 복사 시 중복 방지용 (Initialize에서 할당, 이후 재사용)
    private System.Collections.Generic.HashSet<int> _copyAdressSet;

    // ================================================================
    //  드래그 배치: Selected 1개를 영역에 타일링
    // ================================================================

    /// <summary>
    /// 선택된 블록 1개를 드래그 영역에 직사각형 형태로 타일링합니다.
    /// 매 프레임 호출 — GC 할당 없음
    /// </summary>
    private void RenewalStretchSelected(Vector2 startPos, Vector2 endPos)
    {
        if (!_player.CursorMoved)
            return;
        if (_stretchOriginal.IsVoid())
            return;
        if (!_blockSO.TryGetValue(_stretchOriginal.id, out SO_Block so))
            return;
        int sizeX = Mathf.Max(1, (int)so.Size.x);
        int sizeY = Mathf.Max(1, (int)so.Size.y);
        // 회전 시 크기 스왑
        if (_stretchOriginal.rotation == ERotation.Right || _stretchOriginal.rotation == ERotation.Left) {
            int tmp = sizeX;
            sizeX = sizeY;
            sizeY = tmp;
        }
        (int minX, int minY, int maxX, int maxY) = UGrid.GetForeachPos(startPos, endPos);
        Vector2 startGrid = UGrid.WorldToGrid(startPos);
        float sgx = startGrid.x;
        float sgy = startGrid.y;
        _selecteds.Clear();
        for (int y = minY; y <= maxY; y += sizeY) {
            for (int x = minX; x <= maxX; x += sizeX) {
                if (x < 0 || _width <= x || y < 0 || _height <= y)
                    continue;
                _selecteds.Add(new SelectedBlock(
                    sgx - x, sgy - y,
                    _stretchOriginal.id, _stretchOriginal.rotation
                ));
            }
        }
    }

    // ================================================================
    //  회전
    // ================================================================

    private void TrySelectedRotate(bool clockwise)
    {
        int count = _selecteds.Count;
        if (count <= 0)
            return;
        if (Time.unscaledTime < _nextRotateTime)
            return;
        _nextRotateTime = Time.unscaledTime + _rotateInterval;
        float sin, cos;
        if (clockwise) { sin = 1f; cos = 0f; } else { sin = -1f; cos = 0f; }
        for (int i = 0; i < count; ++i) {
            SelectedBlock sel = _selecteds[i];
            float ox = sel.offsetX * cos - sel.offsetY * sin;
            float oy = sel.offsetX * sin + sel.offsetY * cos;
            sel.offsetX = ox;
            sel.offsetY = oy;
            sel.rotation = clockwise
                ? UGrid.RotateCW(sel.rotation)
                : UGrid.RotateCCW(sel.rotation);
            _selecteds[i] = sel;
        }
        if (!_stretchOriginal.IsVoid()) {
            _stretchOriginal.rotation = clockwise
                ? UGrid.RotateCW(_stretchOriginal.rotation)
                : UGrid.RotateCCW(_stretchOriginal.rotation);
        }
    }

    // ================================================================
    //  이벤트 핸들러
    // ================================================================

    private void OnRotateCW() => TrySelectedRotate(true);
    private void OnRotateCCW() => TrySelectedRotate(false);

    // --- 단일 클릭 배치 ---
    private void OnPlaceOnce(Vector2 pos)
    {
        // UI 위에서 클릭한 경우 무시 (건설 버튼 클릭과 충돌 방지)
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (_selecteds.Count <= 0)
            return;
        //TrySelectedToDesign(pos);
        TrySelectedToDesign();
        _selecteds.Clear();
    }

    // --- 드래그 배치 ---
    private void OnPlaceDrag(Vector2 startPos, Vector2 endPos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (_dragCopy)
            return;
        if (!_dragPlacer) {
            _dragPlacer = true;
            if (_selecteds.Count > 0)
                _stretchOriginal = _selecteds[0];
        }
        RenewalStretchSelected(startPos, endPos);
    }

    private void OnPlaceDragEnd(Vector2 startPos, Vector2 endPos)
    {
        if (_dragCopy)
            return;
        _dragPlacer = false;
        TrySelectedToDesign();
        _selecteds.Clear();
        _stretchOriginal = default;
    }

    // --- 단일 복사 ---
    private void OnCopyOnce(Vector2 clickPos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (_dragCopy || _dragPlacer)
            return;
        _selecteds.Clear();
        int index = UGrid.WorldToIndex(clickPos, _width);
        TryCopySingle(index, 0f, 0f);
    }

    // --- 드래그 복사 ---
    private void OnCopyDrag(Vector2 startPos, Vector2 endPos)
    {
        if (_dragPlacer)
            return;
        _dragCopy = true;
        _selecteds.Clear();
        TryCopyRange(startPos, endPos);
    }

    private void OnCopyDragEnd(Vector2 startPos, Vector2 endPos)
    {
        if (_dragPlacer)
            return;
        _dragCopy = false;
        TrySelectedToDesign();
        _selecteds.Clear();
    }

}
