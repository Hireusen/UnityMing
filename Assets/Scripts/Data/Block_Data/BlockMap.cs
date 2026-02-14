using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 2D 격자 맵의 블록 상태를 관리하는 핵심 클래스입니다.
/// </summary>
public class BlockMap
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private BlockRef[] _blocks;
    private readonly int _width;
    private readonly int _height;
    // 재사용 버퍼 (최대 블록 크기 = 6*6)
    private readonly int[] _cellBuffer = new int[36];

    private GameData _game;
    private TileMap _tile;
    private Dictionary<EBlock, SO_Block> _so;
    private PoolManagement<BlockSingle> _pool;
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
    public int Length => _blocks.Length;

    /// <summary>
    /// 특정 좌표에 있는 블록의 참조를 읽습니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BlockRef GetBlock(int index) => _blocks[index];

    /// <summary>
    /// 특정 좌표에 있는 블록의 참조를 읽습니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetAdress(int index) => _blocks[index].adress;

    /// <summary>
    /// 특정 좌표에 있는 블록의 실제 데이터를 읽습니다.
    /// 셀이 비어있을 경우 호출하면 안됩니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly BlockSingle ReadSingle(int index)
    {
        return ref _pool.Read(_blocks[index].adress);
    }

    /// <summary>
    /// 특정 좌표에 있는 블록의 실제 데이터를 수정 가능하게 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref BlockSingle GetSingleRef(int index)
    {
        return ref _pool.GetRef(_blocks[index].adress);
    }

    /// <summary>
    /// 특정 좌표를 블록이 차지하고 있지 않은지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsVoid(int index) => _blocks[index].IsVoid();

    /// <summary>
    /// 특정 좌표를 블록이 차지하고 있는지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsExist(int index) => !_blocks[index].IsVoid();

    /// <summary>
    /// 특정 좌표가 맵 내부에 있는지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool InMap(int index) => (0 <= index) && (index < Length);
    #endregion

    #region ─────────────────────────▶ 외부 공개 메서드 ◀─────────────────────────
    public BlockMap(int width, int height)
    {
        _width = width;
        _height = height;
        _blocks = new BlockRef[width * height];
        // 모든 셀을 빈 상태로 초기화
        for (int i = 0; i < _blocks.Length; ++i)
            _blocks[i].adress = -1;
        // 캐싱
        _game = GameData.ins;
        _tile = _game.TileMap;
        _so = _game.BlockDatabase;
        _pool = _game.BlockPool;
    }

    /// <summary>
    /// 특정 좌표에 블록을 설치할 수 있는지 검사합니다.
    /// </summary>
    public bool CanPlace(int index, EBlock id, ERotation rotation = ERotation.Up)
    {
        De.IsFalse(_so.ContainsKey(id), LogType.Assert);
        SO_Block so = _so[id];
        Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
        // 점유할 모든 셀 순회
        int count = GetOccupiedCells(index, size, rotation, _cellBuffer);
        for (int i = 0; i < count; ++i) {
            int cellIndex = _cellBuffer[i];
            // 맵 범위, 블록 없음, 지상 타일
            if (!InMap(cellIndex))
                return false;
            if (!IsVoid(cellIndex))
                return false;
            if (!_tile.IsGround(cellIndex))
                return false;
        }
        return true;
    }

    /// <summary>
    /// 특정 좌표에 블록 설치를 시도합니다.
    /// </summary>
    public bool TryPlace(int index, EBlock id, ERotation rotation = ERotation.Up)
    {
        if (!CanPlace(index, id, rotation)) {
            De.Print($"블록 생성 실패 : {UGrid.IndexToGridV(index, _width)}, {id}, {rotation}");
            return false;
        }
        Place(index, id, rotation);
        De.Print($"블록 생성 성공 : {UGrid.IndexToGridV(index, _width)}, {id}, {rotation}");
        return true;
    }

    /// <summary>
    /// 특정 좌표를 차지하고 있는 블록을 삭제합니다.
    /// 해당 블록이 점유하는 모든 셀이 함께 해제됩니다.
    /// </summary>
    public void TryRemove(int index)
    {
        if (_blocks[index].IsVoid())
            return;
        int adress = _blocks[index].adress;
        TryRemoveByAdress(adress);
    }

    /// <summary>
    /// 풀 주소로 블록을 삭제합니다.
    /// </summary>
    public void TryRemoveByAdress(int adress)
    {
        if (!_pool.IsExist(adress))
            return;
        ref readonly BlockSingle data = ref _pool.Read(adress);
        // 점유 셀 해제
        int count = GetOccupiedCells(data.index, data.size, data.rotation, _cellBuffer);
        for (int i = 0; i < count; ++i)
            _blocks[_cellBuffer[i]].Init();
        // 데이터 삭제
        _pool.Destroy(adress);
    }

    /// <summary>
    /// 블록이 점유하는 모든 셀 인덱스를 buffer에 채우고 개수를 반환합니다.
    /// </summary>
    public int GetOccupiedCells(int index, Vector2Int size, ERotation rotation, int[] buffer)
    {
        (int gridX, int gridY) = UGrid.IndexToGrid(index, _width);
        int sizeX = size.x;
        int sizeY = size.y;
        int dirX, dirY; // 확장 방향 (+1 또는 -1)
        int spanX, spanY; // 실제 확장할 가로, 세로 크기

        switch (rotation) {
            // 왼쪽 아래가 중심 = (+x, +y) 확장
            case ERotation.Up:
                dirX = 1;
                dirY = 1;
                spanX = sizeX;
                spanY = sizeY;
                break;
            // 왼쪽 위가 중심 = (+x, -y) 확장
            case ERotation.Right:
                dirX = 1;
                dirY = -1;
                spanX = sizeY;
                spanY = sizeX;
                break;
            // 오른쪽 위가 중심 = (-x, -y) 확장
            case ERotation.Down:
                dirX = -1;
                dirY = -1;
                spanX = sizeX;
                spanY = sizeY;
                break;
            // 오른쪽 아래가 중심 = (-x, +y) 확장
            case ERotation.Left:
                dirX = -1;
                dirY = 1;
                spanX = sizeY;
                spanY = sizeX;
                break;
            // 예외 발생
            default:
                dirX = 1;
                dirY = 1;
                spanX = sizeX;
                spanY = sizeY;
                De.Print("GetOccupiedCells에서 존재하지 않는 회전 열거형을 받았습니다.");
                break;
        }
        // 회전을 고려하여 점유한 셀 순환
        int count = 0;
        for (int y = 0; y < spanY; ++y) {
            for (int x = 0; x < spanX; ++x) {
                int gx = gridX + x * dirX;
                int gy = gridY + y * dirY;
                // 맵 범위 검사
                if (gx < 0 || _width <= gx || gy < 0 || _height <= gy)
                    continue;
                buffer[count++] = UGrid.GridToIndex(gx, gy, _width);
            }
        }
        return count;
    }

    /// <summary>
    /// 블록의 월드 좌표를 반환합니다. (실수)
    /// 어떤 회전이든 블록 스프라이트의 시각적 중심은 점유 영역의 정중앙입니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 GetRenderPos(int index, Vector2Int size, ERotation rotation)
    {
        (int cellX, int cellY) = UGrid.IndexToGrid(index, _width);
        int sizeX = size.x;
        int sizeY = size.y;

        float renderX, renderY;
        switch (rotation) {
            // 왼쪽 아래가 중심 = (+x, +y) 확장
            case ERotation.Up:
                renderX = cellX + sizeX * 0.5f;
                renderY = cellY + sizeY * 0.5f;
                break;
            // 왼쪽 위가 중심 = (+x, -y) 확장
            case ERotation.Right:
                renderX = cellX + sizeY * 0.5f;
                renderY = cellY - sizeX * 0.5f;
                break;
            // 오른쪽 위가 중심 = (-x, -y) 확장
            case ERotation.Down:
                renderX = cellX - sizeX * 0.5f;
                renderY = cellY - sizeY * 0.5f;
                break;
            // 오른쪽 아래가 중심 = (-x, +y) 확장
            case ERotation.Left:
                renderX = cellX - sizeY * 0.5f;
                renderY = cellY + sizeX * 0.5f;
                break;
            // 예외 발생
            default:
                renderX = cellX + sizeX * 0.5f;
                renderY = cellY + sizeY * 0.5f;
                De.Print("GetRenderPos에서 존재하지 않는 회전 열거형을 받았습니다.");
                break;
        }
        return new Vector2(renderX, renderY);
    }
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    /// <summary>
    /// 특정 좌표에 블록을 설치합니다.
    /// </summary>
    private void Place(int index, EBlock id, ERotation rotation = ERotation.Up)
    {
        De.IsFalse(_so.ContainsKey(id), LogType.Assert);
        SO_Block so = _so[id];
        Vector2Int size = new Vector2Int((int)so.Size.x, (int)so.Size.y);
        // 실제 블록 데이터 생성
        BlockSingle data = new BlockSingle(id, rotation, index, size);
        int adress = _pool.Create(data);
        if (De.IsFalse(0 <= adress, LogType.Warning))
            return;
        // 점유할 모든 셀에 같은 주소를 기록
        int count = GetOccupiedCells(index, size, rotation, _cellBuffer);
        for (int i = 0; i < count; ++i)
            _blocks[_cellBuffer[i]].adress = adress;
    }
    #endregion
}
