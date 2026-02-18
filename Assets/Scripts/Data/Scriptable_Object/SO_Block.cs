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

    [Header("부품 회전")]
    [SerializeField] protected float _bodyRotateSpeed = 5f;
    public float BodyRotateSpeed => _bodyRotateSpeed;
    [SerializeField] protected float _turretRotateSpeed = 180f;
    public float TurretRotateSpeed => _turretRotateSpeed;
    [SerializeField] protected float _rotationSpeed = 90f;
    public float RotationSpeed => _rotationSpeed;

    [Header("이펙트")]
    [SerializeField] protected float _effectCycleTime = 1f;
    public float EffectCycleTime => _effectCycleTime;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_size.x <= 0) _size.x = 1;
        if (_size.y <= 0) _size.y = 1;
        if (_bodyRotateSpeed < 0.1f) _bodyRotateSpeed = 0.1f;
        if (_turretRotateSpeed < 0.1f) _turretRotateSpeed = 0.1f;
        if (_effectCycleTime < 0.01f) _effectCycleTime = 0.01f;
    }
}
