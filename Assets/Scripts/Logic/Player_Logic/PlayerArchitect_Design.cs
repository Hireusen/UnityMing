using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 선택한 블록 디자인 블록으로 넘기기
/// </summary>
public partial class PlayerArchitect
{
    private void TrySelectedToDesign(Vector2 center)
    {
        BuildOrder order = new BuildOrder(-1, EOrderType.Build, EBlock.None, ERotation.None);
        foreach (var selected in _selectedList.Values) { // 여기에 Ref는 저장 안함
            // 배치할 실제 좌표 구하기
            Vector2 pos = center + selected.offset;
            int index = UGrid.WorldToIndex(pos, _width);
            // 배치 가능한지 확인
            if (_blockMap.IsExist(index))
                continue;
            Vector2Int size = _blockSO[selected.id].Size;
            ERotation rotate = selected.rotate;
            if (!_blockMap.CanPlace(index, selected.id, rotate))
                continue;
            // 디자인 블록은 덮어씌우기
            (int startX, int endX, int startY, int endY) = UGrid.GetLoopBlock(index, size, rotate, _width, _height);
            for (int loopY = startY; loopY <= endY; ++loopY) {
                for (int loopX = startX; loopX <= endX; ++loopX) {
                    int loopIndex = UGrid.GridToIndex(loopX, loopY, _width);
                    if (_designs.ContainsKey(loopIndex)) {
                        _designs.Remove(loopIndex);
                    }
                }
            }
            // 디자인 블록으로 등록
            order.index = index;
            order.id = selected.id;
            order.rotate = selected.rotate;
            _designs.Add(index, order);
        }
        De.Print($"SelectedList({_selectedList.Count})를 Design으로 변환했습니다.");
    }

    private void RemoveDesignOnce(int index)
    {
        // 자리를 차지한 디자인 블록
        if (!_designs.ContainsKey(index))
            return;
        _designs.Remove(index);
    }

    private void RemoveDesignOnce(Vector2 pos)
    {
        int index = UGrid.WorldToIndex(pos, _width);
        // 자리를 차지한 디자인 블록
        if (!_designs.ContainsKey(index))
            return;
        _designs.Remove(index);
    }

    private void RemoveDesignForeach(Vector2 startPos, Vector2 endPos)
    {
        // 변수 준비
        int minX, minY, maxX, maxY;
        (minX, minY, maxX, maxY) = UGrid.GetForeachPos(startPos, endPos);
        Vector2 mouse = Tool.GetMousePos(_camera);
        Vector2 centerPos = UGrid.WorldToGrid(mouse); // 월드 좌표
        Vector2 centerGrid = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
        // 해당 영역의 디자인 블록 삭제
        for (int y = minY; y <= maxY; ++y) {
            for (int x = minX; x <= maxX; ++x) {
                int index = UGrid.GridToIndex(x, y, _width);
                Vector2 localGrid = centerGrid;
                localGrid.x -= x;
                localGrid.y -= y;
                RemoveDesignOnce(index);
            }
        }
        De.Print($"범위 삭제(디자인)를 실행합니다. ({minX}, {minY}) ─ ({maxX}, {maxY})");
    }

    private void ClearDesign()
    {
        _player.ClearBuild();
    }
}
