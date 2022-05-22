using Common;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    // 初始化时根据配置生成所有地图model
    // 存储管理所有地图
    // upate中更新所有地图
    // 提供kv索引地图的接口
    class MapManager:Singleton<MapManager>
    {
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        public void Init()
        {
            foreach (var mapDefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapDefine);
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                Maps.Add(mapDefine.ID, map);
            }
        }

        public Map this[int key]
        {
            get
            {
                return Maps[key];
            }
        }

        public void Updata()
        {
            foreach (var map in Maps.Values)
            {
                map.Update();
            }
        }

    }
}
