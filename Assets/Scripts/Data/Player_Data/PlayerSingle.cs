using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 플레이어 데이터를 저장하는 클래스입니다.
/// </summary>
public class PlayerSingle
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public Vector2 pos;
    public float angle;

    public float nextBuildTime;
    public readonly float buildRange;
    public readonly float buildInterval;
    public EBuildState buildState;

    public Vector2 CursorInGrid { get; private set; }
    public bool CursorMoved { get; private set; }

    public Dictionary<int, SelectedBlock> selectedBlockList;
    public bool isStretch;
    public bool isCopy;

    public Dictionary<int, BuildOrder> designs;
    public List<int> sequence;

    public PlayerSingle(int width, int height, float buildRange, float buildInterval, int capacity = 100)
    {
        this.pos = new Vector2(width / 2, height / 2);
        this.angle = 0f;
        this.nextBuildTime = Time.time;
        this.buildRange = buildRange;
        this.buildInterval = buildInterval;
        this.buildState = EBuildState.None;
        this.CursorInGrid = Vector2.zero;
        this.CursorMoved = false;
        this.selectedBlockList = new Dictionary<int, SelectedBlock>(capacity);
        this.isStretch = false;
        this.isCopy = false;
        this.designs = new Dictionary<int, BuildOrder>(capacity);
        this.sequence = new List<int>(capacity);
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
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
        designs.Clear();
        sequence.Clear();
    }

    [ContextMenu("Selected 딕셔너리 출력")]
    public void PrintSelected()
    {
        foreach (var pair in selectedBlockList) {
            Debug.Log($"{pair.Key} : {pair.Value}");
        }
    }
    #endregion
}
