using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 타일 로직 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// Tile Scriptable Object를 사용할 수 있도록 딕셔너리화 합니다.
/// </summary>
public class TileSODataBuilder : MonoBehaviour
{
    // Resources 폴더에 있는 SO_Tile 자동으로 가져오기
    public void DataBuilder()
    {
        SO_Tile[] loaded = Resources.LoadAll<SO_Tile>("SOData/Tile");
        // 스크립터블 오브젝트 파일이 없음
        int length = loaded.Length;
        if (De.IsTrue(length <= 0)) {
            De.Print("타일 스크립터블 오브젝트가 존재하지 않습니다.");
            return;
        }
        // 딕셔너리화
        var tempDic = new Dictionary<ETile, SO_Tile>(length);
        for (int i = 0; i < length; ++i) {
            ETile key = loaded[i].ID;
            if (De.IsTrue(tempDic.ContainsKey(key)))
                continue;
            tempDic.Add(loaded[i].ID, loaded[i]);
        }
        GameData.ins.TileDatabase = tempDic;
        De.Print("타일 데이터베이스에 데이터를 주입했습니다.");
    }
}