using UnityEngine;
/// <summary>
/// 카메라 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 카메라 스크립트를 호출합니다.
/// </summary>
public class CameraManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private CameraTrackPlayer _minimapCamera;
    [SerializeField] private CameraTrackPlayer _playerCamera;
    [SerializeField] private CameraZoom _cameraZoom;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 인스펙터 유효성 검사
    public void Verification()
    {
        De.IsNull(_playerCamera);
        De.IsNull(_minimapCamera);
        De.IsNull(_cameraZoom);
        _playerCamera.Verification();
        _minimapCamera.Verification();
        _cameraZoom.Verification();
    }
    // 스크립트 내부 변수 초기화
    public void Initialize()
    {
        _playerCamera.Initialize();
        _minimapCamera.Initialize();
        _cameraZoom.Initialize();
    }
    // 외부에 전달할 데이터 생성
    public void DataBuilder()
    {

    }
    // 마스터 매니저의 Update() 에서 호출할 메서드
    public void RunBeforeFrame()
    {

    }
    // 마스터 매니저의 LateUpdate() 에서 호출할 메서드
    public void RunAfterFrame()
    {
        _playerCamera.RunAfter();
        _minimapCamera.RunAfter();
        _cameraZoom.RunAfterFrame();
    }
    #endregion
}
