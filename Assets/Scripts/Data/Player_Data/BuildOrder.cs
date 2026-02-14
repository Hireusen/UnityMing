/// <summary>
/// 건설 예정 데이터를 담는 구조체입니다.
/// </summary>
public struct BuildOrder
{
    public int index;
    public EOrderType type;
    public EBlock id;
    public ERotation rotation;

    public BuildOrder(int index, EOrderType type, EBlock id, ERotation rotation)
    {
        this.index = index;
        this.type = type;
        this.id = id;
        this.rotation = rotation;
    }

    public bool IsVoid() => type == EOrderType.None;
}

public enum EOrderType : byte { None, Build, Destroy }
