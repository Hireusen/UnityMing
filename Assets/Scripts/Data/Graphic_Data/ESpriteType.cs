/// <summary>
/// 스프라이트의 회전 여부 등을 결정하는 열거형입니다.
/// </summary>
public enum ESpriteType
{
    None = 0,
    Body, // 이동 방향으로 회전
    Turret, // 공격 방향으로 회전
    Static, // 회전 없음
    Rotation, // 독립적인 로직으로 회전
}