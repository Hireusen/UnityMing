using System.Collections.Generic;
/// <summary>
/// 마스터 매니저에서 호출하는 C# 스크립트입니다.
/// 런타임에서 사용하는 변수를 모아놓은 싱글톤 클래스입니다.
/// </summary>
public class GameData
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 & 프로퍼티 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public static GameData ins;

    // 사운드 불러오기
    private SoundAdmin _soundAdmin;
    public SoundAdmin SoundAdmin => _soundAdmin;

    // 플레이어 데이터를 저장할 클래스
    private PlayerSingle _playerSingle;
    public PlayerSingle Player
    {
        get { return _playerSingle; }
        set { _playerSingle = value; }
    }

    // 맵 격자를 기준으로 블록 데이터의 유형과 주소를 저장하는 클래스
    private BlockMap _blockMap;
    public BlockMap Blocks
    {
        get { return _blockMap; }
        set { _blockMap = value; }
    }

    // 블록 원본 데이터를 저장하는 클래스
    private PoolManagement<BlockSingle> _blockPool;
    public PoolManagement<BlockSingle> BlockPool
    {
        get { return _blockPool; }

        set { _blockPool = value; }
    }

    /// <summary>
    /// 여러 타일을 저장하는 타일 맵 구조체
    /// </summary>
    public TileMap TileMap
    {
        get { return _tileMap; }
        set {
            if (_tileMap == null) {
                _tileMap = value;
            } else {
                De.Print("블록 맵을 중복 초기화했습니다.", UnityEngine.LogType.Assert);
            }
        }
    }
    private TileMap _tileMap;

    /// <summary>
    /// 타일 ID로 SO 타일 데이터를 꺼낼 수 있는 딕셔너리
    /// </summary>
    public Dictionary<ETile, SO_Tile> TileDatabase
    {
        get { return _tileDatabase; }
        set {
            if (_tileDatabase == null) {
                _tileDatabase = value;
            } else {
                De.Print("타일 데이터베이스를 중복 초기화했습니다.", UnityEngine.LogType.Assert);
            }
        }
    }
    private Dictionary<ETile, SO_Tile> _tileDatabase;

    /// <summary>
    /// 블록 ID로 SO 블록 데이터를 꺼낼 수 있는 딕셔너리
    /// </summary>
    public Dictionary<EBlock, SO_Block> BlockDatabase
    {
        get { return _blockDatabase; }
        set {
            if (_blockDatabase == null) {
                _blockDatabase = value;
            } else {
                De.Print("블록 데이터베이스를 중복 초기화했습니다.", UnityEngine.LogType.Assert);
            }
        }
    }
    private Dictionary<EBlock, SO_Block> _blockDatabase;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public static void CreateInstance()
    {
        if (ins == null) {
            ins = new GameData();
        }
    }
    public void DataBuilder(SoundAdmin sa)
    {
        _blockPool = new PoolManagement<BlockSingle>();
        _blockMap = new BlockMap(TileMap.Width, TileMap.Height);
        _soundAdmin = sa;
    }
    public bool DataInspection()
    {
        if (De.IsNull(ins)
            || De.IsNull(_blockMap)
            || De.IsNull(_blockDatabase)
            || De.IsNull(_blockPool)
            || De.IsNull(_tileMap)
            || De.IsNull(_tileDatabase)
        ) {
            return false;
        }
        return true;
    }
    #endregion
}
