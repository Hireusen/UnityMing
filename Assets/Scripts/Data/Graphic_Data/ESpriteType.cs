/// <summary>
/// 스프라이트의 회전 및 동작 방식을 결정하는 열거형입니다.
/// </summary>
public enum ESpriteType
{
    None = 0,
    Body,       // 플레이어를 바라봄
    Turret,     // 플레이어를 바라봄
    Static,     // 회전 없음 (설치 방향 고정)
    Rotation,   // 일정 속도로 자전
    Effect,     // 특정 상황에서 투명도 변화 (추후 구현)
}
