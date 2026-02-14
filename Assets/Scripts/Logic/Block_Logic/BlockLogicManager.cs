using UnityEngine;
/// <summary>
/// 블록 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 블록 로직 스크립트를 호출합니다.
/// </summary>
public class BlockLogicManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private BlockSODataBuilder _blockSODataBuilder;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 인스펙터 유효성 검사
    public void Verification()
    {
        De.IsNull(_blockSODataBuilder);
    }
    // 스크립트 내부 변수 초기화
    public void Initialize()
    {

    }
    // 외부에 전달할 데이터 생성
    public void DataBuilder()
    {
        _blockSODataBuilder.DataBuilder();
    }
    // 마스터 매니저의 Update() 에서 호출할 메서드
    public void RunBeforeFrame()
    {

    }
    // 마스터 매니저의 LateUpdate() 에서 호출할 메서드
    public void RunAfterFrame()
    {

    }
    #endregion
}
