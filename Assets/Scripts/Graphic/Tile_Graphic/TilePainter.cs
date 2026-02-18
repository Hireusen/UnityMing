using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타일 그래픽 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// DataBuilder에서 타일맵을 1회 순회하여 정적 렌더링 데이터를 구성하고,
/// RunAfterFrame에서 DrawMeshInstanced로 매 프레임 그립니다.
/// </summary>
public class TilePainter : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _baseMaterial;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private BatchLists[] _batches;
    private int _batchCount;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_mesh);
        De.IsNull(_baseMaterial);
    }

    /// <summary>
    /// 타일맵을 1회 순회하여 정적 렌더링 데이터를 구성합니다.
    /// 맵 생성 직후 한 번만 호출합니다.
    /// </summary>
    public void DataBuilder()
    {
        GameData game = GameData.ins;
        TileMap map = game.TileMap;
        var tileDatabase = game.TileDatabase;
        int width = map.Width;
        int length = width * map.Height;

        // 타일 종류 → 배치 슬롯 인덱스
        Dictionary<ETile, int> tileToSlot = new Dictionary<ETile, int>();
        List<BatchLists> batchList = new List<BatchLists>();

        for (int index = 0; index < length; ++index) {
            ref readonly TileSingle tile = ref map.Read(index);
            if (tile.IsVoid)
                continue;
            ETile id = tile.id;

            // 슬롯 없으면 생성
            if (!tileToSlot.TryGetValue(id, out int slot)) {
                slot = batchList.Count;
                tileToSlot.Add(id, slot);
                Material mat = UGraphic.CreateMaterial(_baseMaterial, tileDatabase[id].GetSprite(0));
                batchList.Add(new BatchLists(mat));
            }

            // 행렬 생성 (셀 중심)
            (int gx, int gy) = UGrid.IndexToGrid(index, width);
            Vector3 pos = new Vector3(gx + 0.5f, gy + 0.5f, 0f);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);

            // 배치에 추가
            BatchLists batch = batchList[slot];
            List<Matrix4x4> curList = batch.matrices[batch.matrices.Count - 1];
            if (1000 <= curList.Count) {
                curList = new List<Matrix4x4>(1000);
                batch.matrices.Add(curList);
            }
            curList.Add(matrix);
        }

        _batches = batchList.ToArray();
        _batchCount = _batches.Length;
        De.Print($"타일 렌더링 데이터를 빌드했습니다. (종류 {_batchCount}개)");
    }

    /// <summary>
    /// 매 프레임 정적 타일 데이터를 DrawMeshInstanced로 그립니다.
    /// </summary>
    public void RunAfterFrame()
    {
        for (int i = 0; i < _batchCount; ++i) {
            // 캐싱
            List<List<Matrix4x4>> matrices = _batches[i].matrices;
            Material mat = _batches[i].Mat;
            // 구워둔 타일 반복
            for (int j = 0; j < matrices.Count; ++j) {
                Graphics.DrawMeshInstanced(_mesh, 0, mat, matrices[j]);
            }
        }
    }
    #endregion
}
