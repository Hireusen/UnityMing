using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어의 건설 로직을 관리하고 키 이벤트를 구독합니다.
/// partial class로 Build / Demolish / Design / Selected 파일과 연결됩니다.
/// </summary>
public partial class PlayerArchitect : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Transform _playerTransform;

    [Header("사용자 정의 설정")]
    [SerializeField] private float _rotateInterval = 0.1f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private float _nextRotateTime;
    private bool _dragPlacer;
    private bool _dragCopy;
    private SelectedBlock _stretchOriginal;

    // 캐싱
    private GameData _game;
    private int _width;
    private int _height;
    private BlockMap _blockMap;
    private PoolManagement<BlockSingle> _blockPool;
    private Dictionary<EBlock, SO_Block> _blockSO;
    private PlayerSingle _player;
    private List<SelectedBlock> _selecteds;
    private Dictionary<int, BuildOrder> _designs;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 외부 공개 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 인스펙터 필수 요소 검증
    /// </summary>
    public void Verification()
    {
        De.IsNull(_camera);
        De.IsNull(_input);
        De.IsNull(_playerTransform);
    }

    /// <summary>
    /// 캐싱 및 이벤트 구독
    /// </summary>
    public void Initialize()
    {
        _game = GameData.ins;
        _width = _game.TileMap.Width;
        _height = _game.TileMap.Height;
        _blockMap = _game.Blocks;
        _blockPool = _game.BlockPool;
        _blockSO = _game.BlockDatabase;
        _player = _game.Player;
        _selecteds = _player.selecteds;
        _designs = _player.designs;
        _stretchOriginal = default;
        _copyAdressSet = new HashSet<int>();

        InitializeDesign();
        SubscribeEvents();
    }

    /// <summary>
    /// 매 프레임 로직 갱신
    /// </summary>
    public void RunBeforeFrame()
    {
        UpdateBuild();
    }

    public void RunAfterFrame() { }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void SubscribeEvents()
    {
        _input.OnScrollUp += OnRotateCW;
        _input.OnScrollDown += OnRotateCCW;
        _input.OnInteractOnce += OnPlaceOnce;
        _input.OnInteractDrag += OnPlaceDrag;
        _input.OnInteractDragEnd += OnPlaceDragEnd;
        _input.OnCopyBlockOnce += OnCopyOnce;
        _input.OnCopyBlockDrag += OnCopyDrag;
        _input.OnCopyBlockDragEnd += OnCopyDragEnd;
        _input.OnDemolishNow += OnDemolishNow;
        _input.OnDemolishOnce += OnDemolishOnce;
        _input.OnDemolishDrag += OnDemolishDrag;
        _input.OnDemolishDragEnd += OnDemolishDragEnd;
        _input.OnBuildReset += OnBuildReset;
    }

    private void UnsubscribeEvents()
    {
        _input.OnScrollUp -= OnRotateCW;
        _input.OnScrollDown -= OnRotateCCW;
        _input.OnInteractOnce -= OnPlaceOnce;
        _input.OnInteractDrag -= OnPlaceDrag;
        _input.OnInteractDragEnd -= OnPlaceDragEnd;
        _input.OnCopyBlockOnce -= OnCopyOnce;
        _input.OnCopyBlockDrag -= OnCopyDrag;
        _input.OnCopyBlockDragEnd -= OnCopyDragEnd;
        _input.OnDemolishNow -= OnDemolishNow;
        _input.OnDemolishOnce -= OnDemolishOnce;
        _input.OnDemolishDrag -= OnDemolishDrag;
        _input.OnDemolishDragEnd -= OnDemolishDragEnd;
        _input.OnBuildReset -= OnBuildReset;
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion
}
