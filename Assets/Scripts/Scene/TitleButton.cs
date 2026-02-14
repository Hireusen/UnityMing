using UnityEngine;

public class TitleButton : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private SceneFlow _flow;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void OnClickPlay()
    {
        De.Print("플레이 버튼 클릭을 인식했습니다.");
        _flow.LoadScene(SceneCatalog.ESceneID.GamePlay, 0f);
    }

    public void OnClickQuit()
    {
        De.Print("게임 종료 버튼 클릭을 인식했습니다.");
        Application.Quit();
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void Awake()
    {
        if (_flow == null) {
            _flow = GameObject.Find("SceneManager").GetComponent<SceneFlow>();
            if( De.IsNull(_flow)) {
                Destroy(gameObject);
            }
        }
    }
    #endregion
}
