using UnityEngine;
/// <summary>
/// 선택한 블록의 정보를 담는 구조체입니다.
/// </summary>
public struct SelectedBlock
{
    public Vector2 offset;
    public EBlock id;
    public ERotation rotate;

    public SelectedBlock(Vector2 offset, EBlock id, ERotation rotate)
    {
        this.offset = offset;
        this.id = id;
        this.rotate = rotate;
    }

    public bool IsVoid()
    {
        if (id == EBlock.None || rotate == ERotation.None)
            return true;
        return false;
    }

    public bool IsExist()
    {
        if (id != EBlock.None && rotate != ERotation.None)
            return true;
        return false;
    }
}
