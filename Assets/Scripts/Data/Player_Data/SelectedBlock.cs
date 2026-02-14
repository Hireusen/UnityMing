using UnityEngine;

/// <summary>
/// 선택한 블록의 정보를 담는 구조체입니다.
/// 마우스를 따라다니는 미리보기 블록 하나에 대응합니다.
/// </summary>
public struct SelectedBlock
{
    /// <summary> 그룹 중심(커서) 기준 상대 그리드 좌표 </summary>
    public float offsetX;
    public float offsetY;
    /// <summary> 블록 종류 </summary>
    public EBlock id;
    /// <summary> 설치 방향 </summary>
    public ERotation rotation;

    public SelectedBlock(float offsetX, float offsetY, EBlock id, ERotation rotation)
    {
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.id = id;
        this.rotation = rotation;
    }

    public bool IsVoid() => id == EBlock.None;
}
