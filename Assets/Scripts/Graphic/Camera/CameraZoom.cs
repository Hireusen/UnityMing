using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 아무 오브젝트에 부착하는 스크립트입니다.
/// 플레이어의 마우스 휠을 인식해서 카메라의 크기를 줄이거나 늘립니다.
/// </summary>
public class CameraZoom : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerInput _input;

    [Header("사용자 정의 설정")]
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _zoomSharpness = 12f;
    [SerializeField] private float _minSize = 3f;
    [SerializeField] private float _maxSize = 10f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    float _desiredSize;
    Dictionary<int, SelectedBlock> _selected;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_camera);
        De.IsNull(_input);
    }
    public void Initialize()
    {
        // 캐싱
        _selected = GameData.ins.Player.selectedBlockList;
        _desiredSize = (_minSize + _maxSize) * 0.5f;
        // 이벤트 구독
        _input.OnScrollUp += ZoomIn;
        _input.OnScrollDown += ZoomOut;
    }
    public void RunAfterFrame()
    {
        float t = UMath.GetSmoothT(_zoomSharpness, Time.deltaTime);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _desiredSize, t);
    }
    private void ZoomIn()
    {
        // UI에 위치, 블록 선택 상태
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (0 < _selected.Count)
            return;
        // orthographicSize를 줄이면 확대
        _desiredSize -= _zoomSpeed;
        _desiredSize = Mathf.Clamp(_desiredSize, _minSize, _maxSize);
    }
    private void ZoomOut()
    {
        // UI에 위치, 블록 선택 상태
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (0 < _selected.Count)
            return;
        // orthographicSize를 늘리면 축소
        _desiredSize += _zoomSpeed;
        _desiredSize = Mathf.Clamp(_desiredSize, _minSize, _maxSize);
    }
    #endregion

    private void OnDestroy()
    {
        _input.OnScrollUp -= ZoomIn;
        _input.OnScrollDown -= ZoomOut;
    }
}
