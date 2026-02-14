using UnityEngine;
/// <summary>
/// 스프라이트를 그리기 위한 정보를 담는 구조체입니다.
/// </summary>
[System.Serializable]
public struct SpriteInfo
{
    [Header("스프라이트 정보")]
    public Sprite sprite;
    public ESpriteType type;
    [Tooltip("Z값은 0 ~ 0.0009 이내로 입력할 것")]
    public Vector3 offset;
}
