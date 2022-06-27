using System;
using Common;
using GameServer.Managers;

namespace GameServer.Services
{
    public class MapService:Singleton<MapService>
    {
        public MapService()
        {
            // todo
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }
    }
}
