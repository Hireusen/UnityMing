using UnityEngine;
public struct BlockSingle
{
    public EBlock id;
    public ERotation rotation;
    public int index; // 블록의 중심(축)이 되는 셀의 1차원 좌표
    public Vector2Int size;
    // 블록의 각 부품의 회전 값
    public float partAngle_1;
    public float partAngle_2;
    public float partAngle_3;

    public BlockSingle(EBlock id, ERotation rotation, int index, Vector2Int size)
    {
        this.id = id;
        this.rotation = rotation;
        this.index = index;
        this.size = size;
        this.partAngle_1 = 0f;
        this.partAngle_2 = 0f;
        this.partAngle_3 = 0f;
    }

    public bool IsVoid() => id == EBlock.None;

    public void Init()
    {
        id = EBlock.None;
        rotation = ERotation.None;
        index = -1;
        size = Vector2Int.zero;
        partAngle_1 = 0f;
        partAngle_2 = 0f;
        partAngle_3 = 0f;
    }
}
