using UnityEngine;

/// <summary>
/// 타일 그래픽 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 타일 그래픽 스크립트를 호출합니다.
/// </summary>
public class TileGraphicManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private TilePainter _tilePainter;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_tilePainter);
        _tilePainter.Verification();
    }

    public void DataBuilder()
    {
        _tilePainter.DataBuilder();
    }

    public void Initialize()
    {

    }

    public void RunBeforeFrame()
    {

    }

    public void RunAfterFrame()
    {
        _tilePainter.RunAfterFrame();
    }
    #endregion
}
