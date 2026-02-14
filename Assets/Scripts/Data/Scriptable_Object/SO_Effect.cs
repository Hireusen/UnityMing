using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effect/Universal", order = 1)]
public class SO_Effect : SO_Matter
{
    [Header("ID")]
    [SerializeField] protected EEffect _id;
    public EEffect ID => _id;
    [SerializeField] protected Vector2 _size = new Vector2(1f, 1f);
    public Vector2 Size => _size;

    [Header("애니메이션")]
    [SerializeField] float _frameTime;
    public float FrameTime => _frameTime;
    public float Duration => _frameTime * _sprite.Length;

    // 개발자가 이상한 값을 입력했는지 검사하고 조정합니다.
    protected override void OnValidate()
    {
        base.OnValidate();
        if (_size.x <= 0) _size.x = 0.1f;
        if (_size.y <= 0) _size.y = 0.1f;
        if (_frameTime < 0f) _frameTime = 0.1f;
    }
}
