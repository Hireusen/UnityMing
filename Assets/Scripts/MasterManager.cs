using UnityEngine;
/// <summary>
/// 마스터 오브젝트에 부착하는 C# 스크립트입니다.
/// 여러 매니저 스크립트를 호출합니다.
/// </summary>
public class MasterManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private TileLogicManager _tileLogicMng;
    [SerializeField] private PlayerLogicManager _playerLogicMng;
    [SerializeField] private BlockLogicManager _blockLogicMng;
    [SerializeField] private TileGraphicManager _tileGraphicMng;
    [SerializeField] private BlockGraphicManager _blockGraphicMng;
    [SerializeField] private EffectGraphicManager _effectGraphicMng;
    [SerializeField] private UserInterfaceManager _userInterfaceMng;
    [SerializeField] private CameraManager _cameraMng;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 개발자 관점에서의 시스템 검증 활동 Verification
    // 유저 관점에서의 시스템 검증 활동 Validation
    private void Verification()
    {
        if (De.IsNull(_tileLogicMng)
            || De.IsNull(_playerLogicMng)
            || De.IsNull(_blockLogicMng)
            || De.IsNull(_tileGraphicMng)
            || De.IsNull(_blockGraphicMng)
            || De.IsNull(_effectGraphicMng)
            || De.IsNull(_userInterfaceMng)
            || De.IsNull(_cameraMng)
        ) {
            Tool.GameStop();
        }
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 스크립트 내의 변수 검증, 스크립트 내부에서만 가능한 초기화 작업
    private void Awake()
    {
        Verification();
        GameData.CreateInstance();
        // 인스펙터 값 유효성 검사
        _tileLogicMng.Verification();
        _playerLogicMng.Verification();
        _blockLogicMng.Verification();
        _tileGraphicMng.Verification();
        _blockGraphicMng.Verification();
        _effectGraphicMng.Verification();
        _userInterfaceMng.Verification();
        _cameraMng.Verification();
    }
    // 스크립트 외부 데이터를 생성하는 작업
    private void OnEnable()
    {
        // 데이터 생성
        _tileLogicMng.DataBuilder();
        _blockLogicMng.DataBuilder();
        GameData.ins.DataBuilder();
        _playerLogicMng.DataBuilder();
        _tileGraphicMng.DataBuilder();
        _blockGraphicMng.DataBuilder();
        _effectGraphicMng.DataBuilder();
        _userInterfaceMng.DataBuilder();
        _cameraMng.DataBuilder();
        // 데이터 유효성 검사
        GameData.ins.DataInspection();
    }
    // 스크립트 외부의 데이터를 사용하는 초기화 작업
    private void Start()
    {
        // 스크립트 내부 값 초기화
        _tileLogicMng.Initialize();
        _playerLogicMng.Initialize();
        _blockLogicMng.Initialize();
        _tileGraphicMng.Initialize();
        _blockGraphicMng.Initialize();
        _effectGraphicMng.Initialize();
        _userInterfaceMng.Initialize();
        _cameraMng.Initialize();
    }
    // 게임 로직
    private void Update()
    {
        _tileLogicMng.RunBeforeFrame();
        _playerLogicMng.RunBeforeFrame();
        _blockLogicMng.RunBeforeFrame();
        _tileGraphicMng.RunBeforeFrame();
        _blockGraphicMng.RunBeforeFrame();
        _effectGraphicMng.RunBeforeFrame();
        _userInterfaceMng.RunBeforeFrame();
        _cameraMng.RunBeforeFrame();
    }
    // 그래픽 로직
    private void LateUpdate()
    {
        _tileLogicMng.RunAfterFrame();
        _playerLogicMng.RunAfterFrame();
        _blockLogicMng.RunAfterFrame();
        _tileGraphicMng.RunAfterFrame();
        _blockGraphicMng.RunAfterFrame();
        _effectGraphicMng.RunAfterFrame();
        _userInterfaceMng.RunAfterFrame();
        _cameraMng.RunAfterFrame();
    }
    // 게임 종료
    private void OnDestroy()
    {
        GameData.ins = null;
    }
    #endregion
}
