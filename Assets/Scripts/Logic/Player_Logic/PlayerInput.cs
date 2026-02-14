using System;
using UnityEngine;
/// <summary>
/// 플레이어 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 유저의 키 입력을 인식해서 이벤트를 뿌립니다.
/// GetAxis는 이곳에서 관리하지 않고 각 스크립트에서 알아서 합니다.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Camera _camera;

    [Header("사용자 정의 설정")]
    [SerializeField] private KeyCode _interact = KeyCode.Mouse0;
    [SerializeField] private KeyCode _demolish = KeyCode.Mouse1;
    [SerializeField] private KeyCode _gamePause = KeyCode.Space;
    [SerializeField] private KeyCode _buildReset = KeyCode.Q;
    [SerializeField] private KeyCode _holdBuild = KeyCode.E;
    [SerializeField] private KeyCode _copyBlock = KeyCode.F;
    [SerializeField] private KeyCode _symmetryHori = KeyCode.Z;
    [SerializeField] private KeyCode _symmetryVert = KeyCode.X;
    [SerializeField] private float _dragThreshold = 10f;
    [SerializeField] private float _scrollInterval = 0.1f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public event Action<Vector2> OnMovementPress;
    public event Action<Vector2> OnInteractNow;
    public event Action<Vector2> OnInteractOnce;
    public event Action<Vector2, Vector2> OnInteractDrag;
    public event Action<Vector2, Vector2> OnInteractDragEnd;
    public event Action<Vector2> OnDemolishNow;
    public event Action<Vector2> OnDemolishOnce;
    public event Action<Vector2, Vector2> OnDemolishDrag;
    public event Action<Vector2, Vector2> OnDemolishDragEnd;
    public event Action OnGamePause;
    public event Action OnBuildReset;
    public event Action OnHoldBuild;
    public event Action<Vector2> OnCopyBlockOnce;
    public event Action<Vector2, Vector2> OnCopyBlockDrag;
    public event Action<Vector2, Vector2> OnCopyBlockDragEnd;
    public event Action OnSymmetryHori;
    public event Action OnSymmetryVert;
    public event Action OnScrollUp;
    public event Action OnScrollDown;
    private bool _interactDrag;
    private bool _demolishDrag;
    private bool _copyBlockDrag;
    private float _nextScroll;
    private Vector2 _dragStartInteractPoint = Vector2.zero;
    private Vector2 _dragStartDemolishPoint = Vector2.zero;
    private Vector2 _dragStartCopyBlockPoint = Vector2.zero;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 마스터 매니저의 Update() 에서 호출할 메서드
    public void RunBeforeFrame()
    {
        Vector2 pos = Tool.GetMousePos(_camera);
        // 드래그 시작 판정
        if (_dragStartInteractPoint != Vector2.zero) {
            if (!UMath.IsWithinDistance(_dragStartInteractPoint, pos, _dragThreshold) && !_interactDrag) {
                _interactDrag = true;
            }
        }
        if (_dragStartDemolishPoint != Vector2.zero) {
            if (!UMath.IsWithinDistance(_dragStartDemolishPoint, pos, _dragThreshold) && !_demolishDrag) {
                _demolishDrag = true;
            }
        }
        if (_dragStartCopyBlockPoint != Vector2.zero) {
            if (!UMath.IsWithinDistance(_dragStartCopyBlockPoint, pos, _dragThreshold) && !_copyBlockDrag) {
                _copyBlockDrag = true;
            }
        }
        // 이동 키
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
            OnMovementPress?.Invoke(new Vector2(h, v));
        // 상호작용 키
        if (Input.GetKeyDown(_interact)) {
            OnInteractNow?.Invoke(pos);
            _dragStartInteractPoint = pos;
        }
        if (Input.GetKey(_interact) && _interactDrag) {
            OnInteractDrag?.Invoke(_dragStartInteractPoint, pos);
        }
        if (Input.GetKeyUp(_interact)) {
            if (_interactDrag) {
                OnInteractDragEnd?.Invoke(_dragStartInteractPoint, pos);
                _interactDrag = false;
            } else {
                OnInteractOnce?.Invoke(pos);
            }
            _dragStartInteractPoint = Vector2.zero;
        }
        // 블럭 철거 키
        if (Input.GetKeyDown(_demolish)) {
            OnDemolishNow?.Invoke(pos);
            _dragStartDemolishPoint = pos;
        }
        if (Input.GetKey(_demolish) && _demolishDrag) {
            OnDemolishDrag?.Invoke(_dragStartDemolishPoint, pos);
        }
        if (Input.GetKeyUp(_demolish)) {
            if (_demolishDrag) {
                OnDemolishDragEnd?.Invoke(_dragStartDemolishPoint, pos);
                _demolishDrag = false;
            } else {
                OnDemolishOnce?.Invoke(pos);
            }
            _dragStartDemolishPoint = Vector2.zero;
        }
        // 게임 일시정지
        if (Input.GetKeyDown(_gamePause)) {
            OnGamePause?.Invoke();
        }
        // 디자인 블록 리셋
        if (Input.GetKeyDown(_buildReset)) {
            OnBuildReset?.Invoke();
        }
        // 건설 일시중지
        if (Input.GetKeyDown(_holdBuild)) {
            OnHoldBuild?.Invoke();
        }
        // 블록을 디자인 블록으로 복사
        if (Input.GetKeyDown(_copyBlock)) {
            _dragStartCopyBlockPoint = pos;
        }
        if (Input.GetKey(_copyBlock) && _copyBlockDrag) {
            OnCopyBlockDrag?.Invoke(_dragStartCopyBlockPoint, pos);
        }
        if (Input.GetKeyUp(_copyBlock)) {
            if (_copyBlockDrag) {
                OnCopyBlockDragEnd?.Invoke(_dragStartCopyBlockPoint, pos);
                _copyBlockDrag = false;
            } else {
                OnCopyBlockOnce?.Invoke(pos);
            }
            _dragStartCopyBlockPoint = Vector2.zero;
        }
        // 디자인 블록 대칭 이동
        if (Input.GetKeyDown(_symmetryHori)) {
            OnSymmetryHori?.Invoke();
        }
        if (Input.GetKeyDown(_symmetryVert)) {
            OnSymmetryVert?.Invoke();
        }
        // 마우스 스크롤 인식
        if (_nextScroll < Time.unscaledTime) {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (0 < scroll) { // 오른쪽 회전
                _nextScroll = Time.unscaledTime + _scrollInterval;
                OnScrollUp?.Invoke();
            } else if (scroll < 0) { // 왼쪽 회전
                _nextScroll = Time.unscaledTime + _scrollInterval;
                OnScrollDown?.Invoke();
            }
        }
        // 게임 종료
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
    #endregion
}
