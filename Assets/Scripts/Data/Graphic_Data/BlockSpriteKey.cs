using System;

/// <summary>
/// 블록 ID와 스프라이트 인덱스를 조합하여 배치 렌더링의 키로 사용하는 구조체입니다.
/// 같은 블록이라도 레이어(Body, Turret 등)별로 다른 Material을 사용해야 하므로 분리합니다.
/// </summary>
public struct BlockSpriteKey : IEquatable<BlockSpriteKey>
{
    public readonly EBlock id;
    public readonly int spriteIndex;

    public BlockSpriteKey(EBlock id, int spriteIndex)
    {
        this.id = id;
        this.spriteIndex = spriteIndex;
    }

    public bool Equals(BlockSpriteKey other)
    {
        return id == other.id && spriteIndex == other.spriteIndex;
    }

    public override bool Equals(object obj)
    {
        return obj is BlockSpriteKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (((int)id << 5) - 1) ^ spriteIndex;
    }
}
