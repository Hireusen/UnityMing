using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/Block/Universal", order = 1)]
public class SO_Block : SO_Matter
{
    [Header("ID")]
    [SerializeField] protected EBlock _id;
    public EBlock ID => _id;
    [SerializeField] protected Vector2Int _size = new Vector2Int(1, 1);
    public Vector2Int Size => _size;

    [Header("블록 특성")]
    [SerializeField] protected bool _rotatable = true;
    public bool Rotatable => _rotatable;
    [SerializeField] protected bool _connection = false;
    public bool Connection => _connection;

    // 개발자가 이상한 값을 입력했는지 검사하고 조정합니다.
    protected override void OnValidate()
    {
        base.OnValidate();
        if (_size.x <= 0) _size.x = 1;
        if (_size.y <= 0) _size.y = 1;
    }
}
