using UnityEngine;

/// <summary>
/// 아무 오브젝트에 부착하는 스크립트입니다.
/// 카메라가 플레이어를 자동으로 추적합니다.
/// </summary>
public class CameraTrackPlayer : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Camera _camera;

    [Header("인스펙터 │ 매니저")]
    [SerializeField] private Transform _target;

    [Header("사용자 정의 설정")]
    [SerializeField] private float _zOffset = -100f;
    [SerializeField] private float _sharpness = 15f;
    [SerializeField] private float _outlineOffset = 1.5f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private float _minX, _maxX;
    private float _minY, _maxY;
    private float _cameraSize;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void SetPlayer(Transform target) => _target = target;
    // Clamp한 목표 좌표를 반환합니다.
    private Vector3 BuildDesiredPos()
    {
        Vector3 desiredPos = _target.position;
        desiredPos.x = Mathf.Clamp(desiredPos.x, _minX, _maxX);
        desiredPos.y = Mathf.Clamp(desiredPos.y, _minY, _maxY);
        desiredPos.z = _zOffset;
        return desiredPos;
    }
    // 카메라의 위치를 플레이어에게 순간이동시킵니다.
    // Z 좌표와 회전 값을 초기화합니다.
    private void InitPos()
    {
        _camera.transform.position = BuildDesiredPos();
        _camera.transform.rotation = Quaternion.identity;
    }
    // 카메라의 위치를 플레이어에게 부드럽게 이동합니다.
    private void SharpnessPos()
    {

        Vector3 desiredPos = BuildDesiredPos();
        float t = UMath.GetSmoothT(_sharpness, Time.deltaTime);         
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPos, t);
    }

    /// <summary>
    /// Clamp하기 위한 값을 변수에 저장합니다.
    /// </summary>
    private void CacheCameraClamp()
    {
        var tileMap = GameData.ins.TileMap;
        float outlineWall = (float)tileMap.OutlineWall;
        float cameraW = _camera.orthographicSize * _camera.aspect - _outlineOffset;
        float cameraH = _camera.orthographicSize - _outlineOffset;

        _minX = outlineWall + cameraW;
        _maxX = (float)tileMap.Width - outlineWall - cameraW;
        _minY = outlineWall + cameraH;
        _maxY = (float)tileMap.Height - outlineWall - cameraH;
        _cameraSize = _camera.orthographicSize;
        //De.Print($"카메라 Clamp 값을 캐싱했습니다. (cameraW = {cameraW}, cameraH = {cameraH})");
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_camera);
    }
    public void Initialize()
    {
        De.IsTrue(GameData.ins.TileMap.IsInvalid());

        CacheCameraClamp();
        InitPos();
    }
    private bool _targetLost = false;
    public void RunAfter()
    {
        if (De.IsNull(_camera)) return;

        // 카메라 크기 달라졌으면 다시 캐싱
        if (_cameraSize != _camera.orthographicSize) {
            CacheCameraClamp();
            InitPos();
        }

        // 카메라 지수 보간
        if (_targetLost == false) {
            if (_target == null) {
                _targetLost = true;
            } else {
                SharpnessPos();
            }
        }
        // 목표 재발견, 카메라 순간이동
        else if (_target != null) {
            _targetLost = false;
            InitPos();
        }
    }
    #endregion
}
