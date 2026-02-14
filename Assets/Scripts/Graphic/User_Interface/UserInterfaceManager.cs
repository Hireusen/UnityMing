using UnityEngine;
/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 유저 인터페이스 및 가상 블록 스크립트를 호출합니다.
/// </summary>
public class UserInterfaceManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private UIBuildButton[] _uiBuildButton;
    [SerializeField] private SelectedBatchBuilder _selectedBatchBuilder;
    [SerializeField] private SelectedPainter _selectedPainter;
    [SerializeField] private DesignBatchBuilder _designBatchBuilder;
    [SerializeField] private DemolishAreaPainter _demolishAreaPainter;
    [SerializeField] private BuildEffectPainter _buildEffectPainter;
    [SerializeField] private DestroyMarkerPainter _destroyMarkerPainter;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 인스펙터 유효성 검사
    public void Verification()
    {
        De.IsNull(_uiBuildButton);
        for (int i = 0; i < _uiBuildButton.Length; ++i) {
            De.IsNull(_uiBuildButton[i]);
        }
        De.IsNull(_selectedBatchBuilder);
        De.IsNull(_selectedPainter);
        De.IsNull(_designBatchBuilder);
        De.IsNull(_demolishAreaPainter);
        De.IsNull(_buildEffectPainter);
        De.IsNull(_destroyMarkerPainter);
        _selectedBatchBuilder.Verification();
        _selectedPainter.Verification();
        _designBatchBuilder.Verification();
        _demolishAreaPainter.Verification();
        _buildEffectPainter.Verification();
        _destroyMarkerPainter.Verification();
    }
    // 외부에 전달할 데이터 생성
    public void DataBuilder()
    {
        
    }
    // 스크립트 내부 변수 초기화
    public void Initialize()
    {
        for (int i = 0; i < _uiBuildButton.Length; ++i) {
            _uiBuildButton[i].Initialize();
        }
        _selectedBatchBuilder.Initialize();
        _designBatchBuilder.Initialize();
        _demolishAreaPainter.Initialize();
        _buildEffectPainter.Initialize();
        _destroyMarkerPainter.Initialize();
    }
    // 마스터 매니저의 Update() 에서 호출할 메서드
    public void RunBeforeFrame()
    {
        
    }
    // 마스터 매니저의 LateUpdate() 에서 호출할 메서드
    public void RunAfterFrame()
    {
        _selectedBatchBuilder.RunAfterFrame();
        _selectedPainter.RunAfterFrame();
        _designBatchBuilder.RunAfterFrame();
        _demolishAreaPainter.RunAfterFrame();
        _buildEffectPainter.RunAfterFrame();
        _destroyMarkerPainter.RunAfterFrame();
    }
    #endregion
}
