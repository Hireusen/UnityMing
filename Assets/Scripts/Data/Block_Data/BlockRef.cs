/// <summary>
/// 격자 맵에 저장할 블록 데이터입니다.
/// </summary>
public struct BlockRef
{
    public int adress;

    public BlockRef(int adress)
    {
        this.adress = adress;
    }

    public bool IsVoid() => adress == -1;
    public void Init() => adress = -1;
}
