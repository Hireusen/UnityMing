using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 그래픽을 그리기 위한 데이터를 저장하는 구조체입니다.
/// </summary>
public struct BatchLists
{
    private readonly Material _material;
    public List<List<Matrix4x4>> matrices;

    public Material Mat => _material;

    public BatchLists(Material material, int capacity = 2)
    {
        this._material = material;
        this.matrices = new List<List<Matrix4x4>>();
        matrices.Add(new List<Matrix4x4>(capacity));
    }
}
