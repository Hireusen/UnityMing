using UnityEngine;
public enum EOrderType : byte { None, Build, Destory }
 
/// <summary>
/// 건설 예정 데이터를 담는 구조체입니다.
/// </summary>
public struct BuildOrder
{
    public int index;
    public EOrderType type;
    public EBlock id;
    public ERotation rotate;

    public BuildOrder(int index, EOrderType type, EBlock id, ERotation rotate)
    {
        this.index = index;
        this.type = type;
        this.id = id;
        this.rotate = rotate;
    }

    public bool IsVoid()
    {
        if (id == EBlock.None)
            return true;
        return false;
    }
}
