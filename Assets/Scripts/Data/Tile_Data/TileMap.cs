using System.Runtime.CompilerServices;

/// <summary>
/// 2D 격자 맵의 타일 상태를 관리하는 클래스입니다.
/// </summary>
public class TileMap
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private readonly TileSingle[] _tiles;
    private readonly int _width;
    private readonly int _height;
    private readonly int _outlineWall;
    #endregion

    #region ─────────────────────────▶ 접근자 ◀─────────────────────────
    /// <summary>
    /// 맵의 가로 길이(셀 개수)입니다.
    /// </summary>
    public int Width => _width;

    /// <summary>
    /// 맵의 세로 길이(셀 개수)입니다.
    /// </summary>
    public int Height => _height;

    /// <summary>
    /// 맵의 가로 길이 * 세로 길이입니다.
    /// </summary>
    public int Length => _tiles.Length;

    /// <summary>
    /// 맵 테두리 벽의 두께(셀 개수)입니다.
    /// </summary>
    public int OutlineWall => _outlineWall;

    /// <summary>
    /// 특정 좌표에 있는 타일 데이터를 읽습니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly TileSingle Read(int index) => ref _tiles[index];

    /// <summary>
    /// 특정 좌표가 지상 타일인지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGround(int index) => _tiles[index].type == ETileType.Ground;

    /// <summary>
    /// 특정 좌표가 벽 타일인지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWall(int index) => _tiles[index].type == ETileType.Wall;

    /// <summary>
    /// 특정 좌표가 빈 타일인지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsVoid(int index) => _tiles[index].id == ETile.None;

    /// <summary>
    /// 특정 좌표에 광석이 있는지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasOre(int index) => _tiles[index].ore != EItem.None;

    /// <summary>
    /// 특정 좌표가 맵 내부에 있는지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool InMap(int index) => (0 <= index) && (index < Length);

    /// <summary>
    /// 타일맵이 유효하지 않은 상태인지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInvalid() => (_width <= 0) || (_height <= 0) || (_outlineWall < 0);
    #endregion

    #region ─────────────────────────▶ 외부 공개 메서드 ◀─────────────────────────
    public TileMap(int width, int height, int outlineWall)
    {
        _width = width;
        _height = height;
        _outlineWall = outlineWall;
        // 배열 초기화
        _tiles = new TileSingle[width * height];
        for (int i = 0; i < _tiles.Length; ++i)
            _tiles[i] = new TileSingle(ETile.None, ETileType.Air, EItem.None);
    }

    /// <summary>
    /// 특정 좌표의 타일을 설정합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTile(int index, TileSingle tile)
    {
        _tiles[index] = tile;
    }

    /// <summary>
    /// 특정 좌표의 타일을 설정합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTile(int index, ETile id, ETileType type, EItem ore = EItem.None)
    {
        _tiles[index] = new TileSingle(id, type, ore);
    }

    /// <summary>
    /// 특정 좌표의 타일 요소를 설정합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetID(int index, ETile id)
    {
        _tiles[index].id = id;
    }

    /// <summary>
    /// 특정 좌표의 타일 요소를 설정합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetType(int index, ETileType type)
    {
        _tiles[index].type = type;
    }

    /// <summary>
    /// 특정 좌표의 타일 요소를 설정합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetOre(int index, EItem ore)
    {
        _tiles[index].ore = ore;
    }
    #endregion
}
