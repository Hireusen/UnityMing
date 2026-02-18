using UnityEngine;

public struct BlockSingle
{
    public EBlock id;
    public ERotation rotation; // 상하좌우 방향
    public int index; // 블록의 중심이 되는 셀의 1차원 좌표
    public Vector2Int size;

    // 부품 회전 값
    public float partAngle_1; // Body 각도
    public float partAngle_2; // Turret 각도
    public float partAngle_3; // Rotation 자전 각도

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
