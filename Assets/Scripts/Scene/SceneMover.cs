using UnityEngine;

public class SceneMover : MonoBehaviour
{
    [Header("필수 요소 등록")]
    [SerializeField] private SceneFlow _flow;
    [SerializeField] private SceneCatalog.ESceneID _id;
    [SerializeField] private float _minimum;

    private void Start()
    {
        _flow.LoadScene(_id, _minimum);
    }
}
