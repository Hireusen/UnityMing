using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 플레이어 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 플레이어의 건설 로직을 관리하고 대부분 키 이벤트를 구독합니다.
/// </summary>
public partial class PlayerArchitect : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerInput _input;

    [Header("사용자 정의 설정")]
    [SerializeField] private float _rotateInterval = 0.1f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private float _nextRotateTime;
    // 왼클과 F키는 동시에 사용 불가
    private bool _dragPlacer;
    private bool _dragCopy;
    // 캐싱
    private GameData _game;
    private int _width;
    private int _height;
    private BlockMap _blockMap;
    private BlockSingle[] _blockArray;
    private PoolManagement<BlockSingle> _blockPool;
    private PlayerSingle _player;
    private Dictionary<int, BuildOrder> _designs;
    private List<int> _sequence;
    private Dictionary<int, SelectedBlock> _selectedList;
    private Dictionary<EBlock, SO_Block> _blockSO;
    private TileMap _tileMap;
    #endregion
    // Selected : 마우스를 따라다니는 그래픽만 있는 블록
    // Design : 맵에 설치된 가상 블록 (건설 예정 블록)

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 인스펙터 유효성 검사
    public void Verification()
    {
        De.IsNull(_camera);
        De.IsNull(_input);
    }
    // 외부에 전달할 데이터 생성
    public void DataBuilder()
    {

    }
    // 스크립트 내부 변수 초기화
    public void Initialize()
    {
        // 캐싱
        _game = GameData.ins;
        _width = _game.TileMap.Width;
        _height = _game.TileMap.Height;
        _blockMap = _game.Blocks;
        _blockPool = _game.BlockPool;
        _player = _game.Player;
        _designs = _player.designs;
        _sequence = _player.sequence;
        _selectedList = _player.selectedBlockList;
        _blockSO = _game.BlockDatabase;
        _tileMap = _game.TileMap;
        // 이벤트 구독
        _input.OnScrollUp += TrySelectedRotateUp; // 블록 시계 회전
        _input.OnScrollDown += TrySelectedRotateDown; // 블록 반시계 회전
        _input.OnInteractOnce += TrySelectedToDesign; // 블럭 설치
        _input.OnInteractDrag += SelectedPlacerDrag; // 블록 드래그 설치
        _input.OnInteractDragEnd += SelectedPlacerDragEnd;
        _input.OnCopyBlockOnce += CopyBlock; // 블록 복사
        _input.OnCopyBlockDrag += CopyBlockDrag; // 블록 드래그 복사
        _input.OnCopyBlockDragEnd += CopyBlockDragEnd;
        _input.OnDemolishNow += SelectedCancel; // 선택 취소
        _input.OnBuildReset += ClearDesign; // 디자인 청소
        _input.OnDemolishOnce += RemoveDesignOnce;
        _input.OnDemolishDragEnd += RemoveDesignForeach;
    }
    // 마스터 매니저의 Update() 에서 호출할 메서드
    public void RunBeforeFrame()
    {
        TryDesignToBlock();
        UpdateBuildState();
    }
    // 마스터 매니저의 LateUpdate() 에서 호출할 메서드
    public void RunAfterFrame()
    {

    }

    private void OnDestroy()
    {
        // 이벤트 해제
        _input.OnScrollUp -= TrySelectedRotateUp; // 블록 시계 회전
        _input.OnScrollDown -= TrySelectedRotateDown; // 블록 반시계 회전
        _input.OnInteractOnce -= TrySelectedToDesign; // 블럭 설치
        _input.OnInteractDrag -= SelectedPlacerDrag; // 블록 드래그 설치
        _input.OnInteractDragEnd -= SelectedPlacerDragEnd;
        _input.OnCopyBlockOnce -= CopyBlock; // 블록 복사
        _input.OnCopyBlockDrag -= CopyBlockDrag; // 블록 드래그 복사
        _input.OnCopyBlockDragEnd -= CopyBlockDragEnd;
        _input.OnDemolishNow -= SelectedCancel; // 선택 취소
        _input.OnBuildReset -= ClearDesign; // 디자인 청소
        _input.OnDemolishOnce -= RemoveDesignOnce;
        _input.OnDemolishDragEnd -= RemoveDesignForeach;
    }
    #endregion
}
