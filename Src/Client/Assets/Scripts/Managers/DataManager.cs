using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using System.IO;
using Newtonsoft.Json;

// 将配置表中的数据加载到内存中
public class DataManager : Singleton<DataManager> {

    public string DataPath;
    public Dictionary<int, CharacterDefine> Characters = null;


    public DataManager()
    {
        DataPath = "Data/";
        Debug.LogFormat("DataManager > DataManager()");
    }


    public IEnumerator LoadData()
    {
        string json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
        Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);
        yield return null;

    }
}
