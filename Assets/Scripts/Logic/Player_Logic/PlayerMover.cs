using UnityEngine;

/// <summary>
/// 플레이어 오브젝트에 부착하는 스크립트입니다.
/// 키를 입력받아 8방향으로 이동합니다.
/// </summary>
public class PlayerMover : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Transform _player;
    [SerializeField] private PlayerInput _input;

    [Header("사용자 정의 설정")]
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _rotationSpeed = 10;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private float _minX, _maxX;
    private float _minY, _maxY;
    private PlayerSingle _playerData;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void SetRotation(Vector2 axis)
    {
        // 건설/철거 중에는 해당 방향을 바라봐야 하므로 이동 방향 회전 스킵
        if (_playerData.lookAtBuild)
            return;
        float desiredAngle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
        Quaternion desiredQuaternion = Quaternion.Euler(0f, 0f, desiredAngle - 90f);
        _player.rotation = Quaternion.Lerp(_player.rotation, desiredQuaternion, 0.1f);
    }
    // 플레이어가 이동을 시도합니다.
    private void TryMove(Vector2 axis)
    {
        // 이동량 계산
        axis = Vector2.ClampMagnitude(axis, 1f);
        Vector2 movement = axis * _moveSpeed * Time.deltaTime;
        if (movement == Vector2.zero)
            return;
        // 이동 실행
        float prevX = _player.position.x;
        float prevY = _player.position.y;
        float x = prevX + movement.x;
        x = Mathf.Clamp(x, _minX, _maxX);
        float y = prevY + movement.y;
        y = Mathf.Clamp(y, _minY, _maxY);
        _player.position = new Vector3(x, y, -60f);
    }
    public void Verification()
    {
        De.IsNull(_player);
        De.IsNull(_input);
    }
    public void Initialize()
    {
        // 캐싱
        _playerData = GameData.ins.Player;
        var map = GameData.ins.TileMap;
        De.IsTrue(map.IsInvalid(), LogType.Assert, "타일 맵이 생성되지 않은 채 PlayerMover에 도착했습니다.");
        float outlineWall = (float)map.OutlineWall;
        _minX = outlineWall;
        _maxX = (float)map.Width - outlineWall;
        _minY = outlineWall;
        _maxY = (float)map.Height - outlineWall;
        // 이벤트
        _input.OnMovementPress += TryMove;
        _input.OnMovementPress += SetRotation;
    }
    #endregion

    private void OnDestroy()
    {
        _input.OnMovementPress -= TryMove;
        _input.OnMovementPress -= SetRotation;
    }
}
