using UnityEngine;
/// <summary>
/// 타일 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 타일 로직 스크립트를 호출합니다.
/// </summary>
public class TileLogicManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private TileSODataBuilder _tileSODataBuilder;
    [SerializeField] private TileMapBuilder _tileMapBuilder;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 인스펙터 유효성 검사
    public void Verification()
    {
        De.IsNull(_tileSODataBuilder);
        De.IsNull(_tileMapBuilder);
        _tileMapBuilder.Verification();
    }
    // 스크립트 내부 변수 초기화
    public void Initialize()
    {

    }
    // 외부에 전달할 데이터 생성
    public void DataBuilder()
    {
        _tileSODataBuilder.DataBuilder();
        _tileMapBuilder.DataBuilder();
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
