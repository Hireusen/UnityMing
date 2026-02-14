using System.Linq;
using UnityEngine;
/// <summary>
/// 블록 선택하기 (건설 UI, 카피)
/// </summary>
public partial class PlayerArchitect
{
    // 해당 좌표에 존재하는 블록을 selected로 복사한다.
    private void TryOnceCopy(int index, Vector2 offset)
    {
        /*// 맵 이내 & 블럭 존재
        if (!UGrid.InMap(index, _width, _height))
            return;
        if (!_blockMap.IsExist(index))
            return;
        // 호스트의 인덱스로 설정
        int hostIndex;
        if (_blockMap.IsRef(index)) {
            Vector2 hostPos = _blockMap.GetHostBlock(index);
            hostIndex = UGrid.GridToIndex(hostPos, _width);
        } else {
            hostIndex = index;
        }
        // 자리를 차지하지 않은 블록
        if (_selectedList.ContainsKey(hostIndex))
            return;
        // 호스트 블록을 저장
        int defaultAdress = _blockArray[hostIndex].defaultAdress;
        ref readonly BlockDefault block = ref _game.BlockPool.GetReadonly(defaultAdress);
        _selectedList.Add(hostIndex, new SelectedBlock(offset, block.id, block.rotate));
        De.Print($"현재 마우스 위치의 블록({block.id})을 카피했습니다.");*/
    }

    // 두 좌표를 기준으로 존재하는 블록을 selected로 복사한다.
    private void TryForeachCopy(Vector2 startPos, Vector2 endPos)
    {
        // 변수 준비
        int minX, minY, maxX, maxY;
        (minX, minY, maxX, maxY) = UGrid.GetForeachPos(startPos, endPos);
        Vector2 mouse = Tool.GetMousePos(_camera);
        Vector2 centerPos = UGrid.WorldToGrid(mouse); // 월드 좌표
        Vector2 centerGrid = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
        // 해당 영역의 블록 모두 읽기
        for (int y = minY; y <= maxY; ++y) {
            for (int x = minX; x <= maxX; ++x) {
                int index = UGrid.GridToIndex(x, y, _width);
                Vector2 localGrid = centerGrid;
                localGrid.x -= x;
                localGrid.y -= y;
                TryOnceCopy(index, localGrid);
            }
        }
        De.Print($"범위 복사를 실행합니다. ({minX}, {minY}) ─ ({maxX}, {maxY})");
    }

    /// <summary>
    /// 선택한 블록 1개를 드래그 영역에 직사각형 형태로 복제합니다.
    /// 매 프레임 호출되므로 _selectedList를 완전히 재구성합니다.
    /// </summary>
    private void RenewalStretchSelected(Vector2 startPos, Vector2 endPos)
    {
        // 변화가 있을 경우만
        if (!_player.CursorMoved)
            return;
        // 선택된 블록이 없으면 무시
        if (_selectedList.Count <= 0)
            return;
        // 첫 번째 블록 정보를 보존
        int firstKey = _selectedList.Keys.First();
        SelectedBlock original = _selectedList[firstKey];
        if (original.IsVoid())
            return;
        // SO 데이터에서 블록 크기 가져오기
        EBlock id = original.id;
        if (!_blockSO.TryGetValue(id, out SO_Block so))
            return;
        int sizeX = Mathf.Max(1, (int)so.Size.x);
        int sizeY = Mathf.Max(1, (int)so.Size.y);
        // 회전 상태에 따라 크기 스왑
        if (original.rotate == ERotation.Right || original.rotate == ERotation.Left) {
            int tmp = sizeX;
            sizeX = sizeY;
            sizeY = tmp;
        }
        // 드래그 영역 계산
        int minX, minY, maxX, maxY;
        (minX, minY, maxX, maxY) = UGrid.GetForeachPos(startPos, endPos);
        // 시작 그리드 좌표 (offset 기준점)
        Vector2 startGrid = UGrid.WorldToGrid(startPos);
        // 기존 선택 목록 초기화 후 영역 채우기
        _selectedList.Clear();
        for (int y = minY; y <= maxY; y += sizeY) {
            for (int x = minX; x <= maxX; x += sizeX) {
                int index = UGrid.GridToIndex(x, y, _width);
                // 맵 이내 검사
                if (!UGrid.InMap(x, y, _width, _height))
                    continue;
                // 이미 등록된 키 중복 방지
                if (_selectedList.ContainsKey(index))
                    continue;
                // offset: 시작점(마우스 클릭 위치) 기준 상대 좌표
                Vector2 offset = new Vector2(startGrid.x - x, startGrid.y - y);
                _selectedList.Add(index, new SelectedBlock(offset, original.id, original.rotate));
            }
        }
    }

    private void TrySelectedRotate(bool clockwise)
    {
        // 선택한 블럭 존재
        int count = _selectedList.Count;
        if (count <= 0)
            return;
        // 회전 쿨타임
        if (Time.unscaledTime < _nextRotateTime)
            return;
        _nextRotateTime = Time.unscaledTime + _rotateInterval;
        // 변수 준비
        float radian = Mathf.Deg2Rad;
        if (clockwise) radian *= 90f;
        else radian *= 270f;
        // 선택된 모든 블록 회전
        int[] keys = new int[count];
        _selectedList.Keys.CopyTo(keys, 0);
        for (int i = 0; i < count; ++i) {
            int key = keys[i];
            SelectedBlock tmp = _selectedList[key];
            tmp.offset = UMath.Rotate(tmp.offset, radian);
            if (clockwise)
                tmp.rotate = UGrid.ClockwisERotation(tmp.rotate);
            else
                tmp.rotate = UGrid.CounterClockwisERotation(tmp.rotate);
            _selectedList[key] = tmp;
        }
        De.Print($"선택 블록을 {radian * Mathf.Rad2Deg}도 만큼 회전했습니다.");
    }

    private void TrySelectedRotateUp()
    {
        TrySelectedRotate(true);
    }

    private void TrySelectedRotateDown()
    {
        TrySelectedRotate(false);
    }

    private void CopyBlock(Vector2 clickPos)
    {
        if (_dragCopy || _dragPlacer)
            return;
        // 실행
        int index = UGrid.WorldToIndex(clickPos, _width);
        _selectedList.Clear();
        TryOnceCopy(index, Vector2.zero);
    }

    private void CopyBlockDrag(Vector2 startPos, Vector2 endPos)
    {
        if (_dragPlacer)
            return;
        _dragCopy = true;
        // 실행
        _selectedList.Clear();
        TryForeachCopy(startPos, endPos);
    }

    private void CopyBlockDragEnd(Vector2 startPos, Vector2 endPos)
    {
        if (_dragPlacer)
            return;
        _dragCopy = false;
        // 실행
        Vector2 center = Tool.GetMousePos(_camera);
        TrySelectedToDesign(center);
        _selectedList.Clear();
    }

    private void SelectedPlacerDrag(Vector2 startPos, Vector2 endPos)
    {
        if (_dragCopy)
            return;
        _dragPlacer = true;
        // 실행
        RenewalStretchSelected(startPos, endPos);
    }

    private void SelectedPlacerDragEnd(Vector2 startPos, Vector2 endPos)
    {
        if (_dragCopy)
            return;
        _dragPlacer = false;
        // 실행
        Vector2 center = Tool.GetMousePos(_camera);
        TrySelectedToDesign(center);
        _selectedList.Clear();
        De.Print($"선택 드래그가 종료되었습니다.");
    }

    private void SelectedCancel(Vector2 pos)
    {
        De.Print($"선택 블록({_selectedList.Count})을 청소했습니다.");
        _dragCopy = false;
        _dragPlacer = false;
        _selectedList.Clear();
    }
}
