using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneCatalog : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Serializable]
    public class SceneEntry
    {
        public ESceneID id;
        public string name;
    }
    [Header("씬 등록")]
    [SerializeField] private List<SceneEntry> _scenes = new List<SceneEntry>();

    [Header("옵션")]
    [SerializeField] private bool _buildOnAwake = true;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public enum ESceneID
    {
        Manager,
        Title,
        Planet,
        GamePlay,
        GameEnd,
        Count
    }
    private readonly Dictionary<ESceneID, string> _idToName = new Dictionary<ESceneID, string>((int)ESceneID.Count);
    private readonly Dictionary<string, ESceneID> _nameToID = new Dictionary<string, ESceneID>((int)ESceneID.Count);
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 씬 카탈로그를 다시 빌드합니다.
    /// </summary>
    [ContextMenu("카탈로그를 다시 빌드합니다.")]
    public void BuildMaps()
    {
        _idToName.Clear();
        _nameToID.Clear();
        De.Print($"씬 카탈로그를 청소했습니다.");
        for (int i = 0; i < _scenes.Count; ++i) {
            SceneEntry e = _scenes[i];
            // 비어있거나 중복 검사
            if(e == null)
                continue;
            if (De.IsTrue(string.IsNullOrEmpty(e.name)))
                continue;
            if (De.IsTrue(_idToName.ContainsKey(e.id)))
                continue;
            if (De.IsTrue(_nameToID.ContainsKey(e.name)))
                continue;
            // 등록
            _idToName.Add(e.id, e.name);
            _nameToID.Add(e.name, e.id);
            De.Print($"씬({e.id}, {e.name})을 카탈로그에 등록했습니다.");
        }
    }

    /// <summary>
    /// 해당 ID를 가진 씬이 존재할 경우 이름을 반환합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool TryGetSceneName(ESceneID id, out string name)
    {
        return _idToName.TryGetValue(id, out name);
    }

    /// <summary>
    /// 해당 ID를 가진 씬이 존재할 경우 이름을 반환합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetSceneName(ESceneID id)
    {
        if(_idToName.TryGetValue(id, out string name)) {
            return name;
        }
        return string.Empty;
    }

    /// <summary>
    /// 해당 이름을 가진 씬이 존재할 경우 ID를 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool TryGetSceneID(string name, out ESceneID id)
    {
        return _nameToID.TryGetValue(name, out id);
    }
    
    /// <summary>
    /// 원본 씬 리스트를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public List<SceneEntry> GetEntries()
    {
        return _scenes;
    }

    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void Awake()
    {
        if (_buildOnAwake) {
            BuildMaps();
        }
    }
    #endregion
}
