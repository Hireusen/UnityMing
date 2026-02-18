/// <summary>
/// 타일 하나의 정보를 담는 구조체입니다.
/// </summary>
public struct TileSingle
{
    public ETile id;
    public ETileType type;
    public EItem ore;

    public TileSingle(ETile id, ETileType type, EItem ore)
    {
        this.id = id;
        this.type = type;
        this.ore = ore;
    }

    public readonly bool IsVoid => id == ETile.None;
    public readonly bool IsGround => type == ETileType.Ground;
    public readonly bool IsWall => type == ETileType.Wall;
    public readonly bool HasOre => ore != EItem.None;

    public void Init()
    {
        id = ETile.None;
        type = ETileType.Air;
        ore = EItem.None;
    }
}
