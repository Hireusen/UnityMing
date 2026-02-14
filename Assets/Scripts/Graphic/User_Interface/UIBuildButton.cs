using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 건설 UI의 블록 버튼에 부착하는 C# 스크립트입니다.
/// 버튼을 클릭하면 해당 블록을 플레이어의 selectedBlockList에 등록합니다.
/// </summary>
public class UIBuildButton : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("블록 설정")]
    [SerializeField] private EBlock _blockID;
    #endregion

    Dictionary<int, SelectedBlock> _selectedList;

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 스크립트 내부 변수 초기화
    public void Initialize()
    {
        var game = GameData.ins;
        if (De.IsNull(game)) return;
        if (De.IsNull(game.Player)) return;
        _selectedList = game.Player.selectedBlockList;
    }

    /// <summary>
    /// 기존 선택을 초기화하고, 해당 블록을 하나 선택합니다.
    /// </summary>
    public void OnClickSelect()
    {
        // 기존 선택 초기화
        _selectedList.Clear();
        // 블록 1개 선택
        SelectedBlock selected = new SelectedBlock(Vector2.zero, _blockID, ERotation.Up);
        _selectedList.Add(0, selected);
        De.Print($"블록({_blockID})을 선택했습니다.");
    }
    #endregion
}
