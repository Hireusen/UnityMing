using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile/Universal", order = 1)]
public class SO_Tile : SO_Matter
{
    [Header("ID")]
    [SerializeField] protected ETile _id;
    public ETile ID => _id;
    [SerializeField] protected ETileType _type;
    public ETileType Type => _type;
    [SerializeField] protected Vector2Int _size = new Vector2Int(1, 1);
    public Vector2Int Size => _size;

    // 개발자가 이상한 값을 입력했는지 검사하고 조정합니다.
    protected override void OnValidate()
    {
        base.OnValidate();
        if (_size.x <= 0) _size.x = 1;
        if (_size.y <= 0) _size.y = 1;
    }
}
