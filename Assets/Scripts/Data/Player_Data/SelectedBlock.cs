using UnityEngine;

/// <summary>
/// 선택한 블록의 정보를 담는 구조체입니다.
/// 마우스를 따라다니는 미리보기 블록 하나에 대응합니다.
/// </summary>
public struct SelectedBlock
{
    public float offsetX;
    public float offsetY;
    public EBlock id;
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
