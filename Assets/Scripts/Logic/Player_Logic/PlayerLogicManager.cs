using UnityEngine;
/// <summary>
/// 플레이어 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 플레이어 로직 스크립트를 호출합니다.
/// </summary>
public class PlayerLogicManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private PlayerDataBuilder _playerDataBuilder;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private PlayerArchitect _playerArchitect;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 인스펙터 유효성 검사
    public void Verification()
    {
        De.IsNull(_playerDataBuilder);
        De.IsNull(_playerInput);
        De.IsNull(_playerMover);
        De.IsNull(_playerArchitect);
        _playerMover.Verification();
    }
    // 스크립트 내부 변수 초기화
    public void Initialize()
    {
        _playerDataBuilder.Initialize();
        _playerArchitect.Initialize();
        _playerMover.Initialize();
    }
    // 외부에 전달할 데이터 생성
    public void DataBuilder()
    {
        _playerDataBuilder.DataBuilder();
        _playerArchitect.DataBuilder();
    }
    // 마스터 매니저의 Update() 에서 호출할 메서드
    public void RunBeforeFrame()
    {
        _playerDataBuilder.RunBeforeFrame();
        _playerInput.RunBeforeFrame();
        _playerArchitect.RunBeforeFrame();
    }
    // 마스터 매니저의 LateUpdate() 에서 호출할 메서드
    public void RunAfterFrame()
    {

    }
    #endregion
}
