using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어 데이터를 저장하는 클래스입니다.
/// </summary>
public class PlayerSingle
{
    #region ─────────────────────────▶ 변수 ◀─────────────────────────
    public Vector2 pos;
    public float angle;

    // 건설
    public float buildRemain;
    public readonly float buildRange;
    public readonly float buildInterval;
    public EBuildState buildState;

    // 커서
    public Vector2 CursorInGrid { get; private set; }
    public bool CursorMoved { get; private set; }

    // 선택 블록 (마우스를 따라다니는 미리보기)
    public readonly List<SelectedBlock> selecteds;
    // 디자인 블록 (맵에 설치된 건설 예정 블록)
    public Dictionary<int, BuildOrder> designs;

    // 철거 영역 시각화
    public bool demolishActive;
    public int demolishMinX;
    public int demolishMinY;
    public int demolishMaxX;
    public int demolishMaxY;

    // 건설/철거 중 바라보기 (PlayerMover에서 이동 방향 회전 스킵용)
    public bool lookAtBuild;
    #endregion

    #region ─────────────────────────▶ 생성자 ◀─────────────────────────
    public PlayerSingle(int width, int height, float buildRange, float buildInterval, int capacity = 256)
    {
        this.pos = new Vector2(width / 2, height / 2);
        this.angle = 0f;
        this.buildRemain = 0f;
        this.buildRange = buildRange;
        this.buildInterval = buildInterval;
        this.buildState = EBuildState.None;
        this.CursorInGrid = Vector2.zero;
        this.CursorMoved = false;
        this.selecteds = new List<SelectedBlock>(capacity);
        this.designs = new Dictionary<int, BuildOrder>(capacity);
        this.demolishActive = false;
    }
    #endregion

    #region ─────────────────────────▶ 메서드 ◀─────────────────────────
    public bool UpdateCursor(Camera camera)
    {
        Vector2 mousePos = Tool.GetMousePos(camera);
        Vector2 newGrid = UGrid.WorldToGrid(mousePos);
        if (CursorInGrid != newGrid) {
            CursorInGrid = newGrid;
            CursorMoved = true;
            return true;
        }
        CursorMoved = false;
        return false;
    }

    public void ClearBuild()
    {
        buildState = EBuildState.None;
        designs.Clear();
    }
    #endregion
}
