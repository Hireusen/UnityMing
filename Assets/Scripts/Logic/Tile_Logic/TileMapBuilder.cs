using System.Collections.Generic;
using UnityEngine;

public class TileMapBuilder : MonoBehaviour
{
    [System.Serializable]
    public class OreConfig
    {
        [Tooltip("희귀한 광석이 앞 번호를 차지해야 한다.")]
        public EItem ore = EItem.None;
        [Tooltip("값이 클수록 광석 덩어리가 작다.")]
        [Range(0, 1)] public float scale;
        [Tooltip("값이 클수록 광석이 희귀하다.")]
        [Range(0, 1)] public float threshold;
    }
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private List<ETile> _groundTileID;
    [SerializeField] private List<ETile> _wallTileID;

    [Header("지형 생성 설정")]
    [SerializeField, Range(10, 1000)] private short _mapWidth = 100;
    [SerializeField, Range(10, 1000)] private short _mapHeight = 100;
    [SerializeField, Range(0, 100)] private int _noisePercent = 60;
    [SerializeField, Range(0, 10)] private int _smoothCount = 5;
    [SerializeField, Range(0, 10)] private int _outlineWall = 5;

    [Header("광석 생성 설정")]
    [SerializeField] private OreConfig[] _oreConfig;

    [Header("시드")]
    [SerializeField] private bool _isRandomSeed = true;
    [SerializeField] private int _seed;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    const int WALL_AROUND_RANGE = 1;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 맵을 생성하고 세포 자동자 알고리즘으로 지형과 타일을 결정합니다.
    /// 사용하는 타일은 TileBuilder의 인스펙터에서 등록해야 합니다.
    /// </summary>
    public void DataBuilder()
    {
        // 시드 결정
        if (_isRandomSeed) {
            _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            UnityEngine.Random.InitState(_seed);
            De.Print($"맵 생성을 위한 시드를 생성했습니다. ({_seed})");
        }
        System.Random rand = new System.Random(_seed);
        // 맵 생성
        TileMap tempMap = new TileMap(_mapWidth, _mapHeight, _outlineWall);
        // 지형 결정
        SetRandomMap(ref tempMap, rand);
        for (int i = 0; i < _smoothCount; ++i) {
            SmoothMap(ref tempMap);
        }
        // 타일 랜덤 배치
        SetRandomTile(ref tempMap, rand);
        De.Print($"맵 생성을 완료했습니다. ({_mapWidth} * {_mapHeight})");
        // 광석 랜덤 적용
        SetRandomOre(ref tempMap);
        // 결과물 적용
        GameData.ins.TileMap = tempMap;
    }

    // 맵에 지상과 벽을 무작위로 결정합니다.
    private void SetRandomMap(ref TileMap map, System.Random rand)
    {
        TileSingle wall = new TileSingle(ETile.None, ETileType.Wall, EItem.None);
        TileSingle ground = new TileSingle(ETile.None, ETileType.Ground, EItem.None);
        for (short y = 0; y < _mapHeight; ++y) {
            for (short x = 0; x < _mapWidth; ++x) {
                int index = y * _mapWidth + x;
                // 외곽 벽 생성
                if (x < _outlineWall
                    || _mapWidth - _outlineWall < x
                    || y < _outlineWall
                    || _mapHeight - _outlineWall < y
                ) {
                    map.SetTile(index, wall);
                }
                // 랜덤 지형 생성
                else {
                    if (rand.Next(0, 100) < _noisePercent)
                        map.SetTile(index, ground);
                    else
                        map.SetTile(index, wall);
                }
            }
        }
    }

    // 자연스러운 지형이 되도록 개선합니다.
    private void SmoothMap(ref TileMap map)
    {
        TileSingle wall = new TileSingle(ETile.None, ETileType.Wall, EItem.None);
        TileSingle ground = new TileSingle(ETile.None, ETileType.Ground, EItem.None);
        for (short y = 0; y < _mapHeight; ++y) {
            for (short x = 0; x < _mapWidth; ++x) {
                // 주변에 벽이 5개 이상일 경우 Wall, 그렇지 않을 경우 Ground
                int aroundWall = GetAroundWall(ref map, x, y);
                int index = y * _mapWidth + x;
                if (5 <= aroundWall)
                    map.SetTile(index, wall);
                else
                    map.SetTile(index, ground);
            }
        }
    }

    // 주변에 있는 벽 개수를 셉니다.
    private int GetAroundWall(ref TileMap map, short centerX, short centerY)
    {
        int count = 0;
        // 주변 한 칸 순회 (3*3)
        for (int y = centerY - WALL_AROUND_RANGE; y <= centerY + WALL_AROUND_RANGE; ++y) {
            for (int x = centerX - WALL_AROUND_RANGE; x <= centerX + WALL_AROUND_RANGE; ++x) {
                // 맵을 벗어남
                if (x < 0) continue;
                if (_mapWidth <= x) continue;
                if (y < 0) continue;
                if (_mapHeight <= y) continue;
                // 벽 발견
                int index = y * _mapWidth + x;
                if (map.IsWall(index))
                    count++;
            }
        }
        return count;
    }

    // 지형에 맞춰 타일을 랜덤으로 배치합니다.
    private void SetRandomTile(ref TileMap map, System.Random rand)
    {
        for (short y = 0; y < _mapHeight; ++y) {
            for (short x = 0; x < _mapWidth; ++x) {
                int index = y * _mapWidth + x;
                // Random Wall
                if (map.IsWall(index)) {
                    int num = rand.Next(0, _wallTileID.Count);
                    map.SetID(index, _wallTileID[num]);
                }
                // Random Ground
                else {
                    int num = rand.Next(0, _groundTileID.Count);
                    map.SetID(index, _groundTileID[num]);
                }
            }
        }
    }

    // 바닥 타일에 광석을 펄린 노이즈로 자연스럽게 밀집시켜 생성합니다.
    private void SetRandomOre(ref TileMap map)
    {
        int length = _oreConfig.Length;
        // 광석마다 서로 다른 랜덤 오프셋
        Vector2[] offset = new Vector2[length];
        for (int i = 0; i < length; ++i) {
            offset[i].x = Random.Range(0f, 10000f);
            offset[i].y = Random.Range(0f, 10000f);
        }
        // 모든 타일 순회
        for (int y = 0; y < _mapHeight; ++y) {
            for (int x = 0; x < _mapWidth; ++x) {
                int index = UGrid.GridToIndex(x, y, _mapWidth);
                
                // 바닥 타일이어야 함
                if (!map.IsGround(index))
                    continue;
                // 가장 높은 노이즈를 가진 광석
                for (int i = 0; i < length; ++i) {
                    float scale = _oreConfig[i].scale;
                    float noiseX = x * scale + offset[i].x;
                    float noiseY = y * scale + offset[i].y;
                    float noise = Mathf.PerlinNoise(noiseX, noiseY);
                    // 광석이 임계값을 넘는가?
                    if (noise <_oreConfig[i].threshold)
                        continue;
                    // 광석 적용 완료
                    map.SetOre(index, _oreConfig[i].ore);
                    break;
                }
            }
        }
    }

    public void Verification()
    {
        UArray.IsInitedList(_groundTileID);
        UArray.IsInitedList(_wallTileID);
        UArray.IsInitedArray(_oreConfig);
    }
    #endregion
}
