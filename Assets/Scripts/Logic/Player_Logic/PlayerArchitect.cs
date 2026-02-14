using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 플레이어의 건설 로직을 관리하고 키 이벤트를 구독합니다.
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
    private bool _dragPlacer;
    private bool _dragCopy;
    // 드래그 타일링 시 원본 블록 보존용
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

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_camera);
        De.IsNull(_input);
    }

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
        _copyAdressSet = new System.Collections.Generic.HashSet<int>();
        InitializeDesign();
        // 이벤트 구독
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
    }

    public void RunBeforeFrame() { }
    public void RunAfterFrame() { }

    private void OnDestroy()
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
    }
    #endregion
}
