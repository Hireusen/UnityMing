using UnityEngine;
/// <summary>
/// ~ 스크립트에 부착하는 C# 스크립트입니다.
/// ~ 오브젝트에 부착하는 C# 스크립트입니다.
/// ~ 합니다.
/// </summary>
public class Test : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Transform _player;

    [Header("인스펙터 │ 매니저 │ 자동 등록")]
    [SerializeField] private Transform _target;

    [Header("사용자 정의 설정")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, 0f);
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓

    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;
        Vector2 pos = Tool.GetMouseWorldPos(Camera.main);
        int index = UGrid.WorldToIndex(pos, GameData.ins.TileMap.Width);
        GameData.ins.Blocks.TryPlace(index, EBlock.LargeTitaniumWall);
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓

    #endregion
}
