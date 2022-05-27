using Common;
using Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    // 管理各种配置数据
    class DataManager:Singleton<DataManager>
    {
        internal string DataPath;
        internal Dictionary<int, MapDefine> Maps = null;

        public DataManager()
        {
            DataPath = "Data/";
            Log.Info("DataManager > DataManager()");
        }

        internal void Load()
        {
            string json = File.ReadAllText(DataPath + "MapDfine.txt"); // todo 这个是去哪个文件夹下拿的？
            Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
        }

    }
}
