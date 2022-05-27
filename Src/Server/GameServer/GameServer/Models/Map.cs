using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using GameServer.Entities;
using Network;
using SkillBridge.Message;

namespace GameServer.Models
{

    // 存储了“地图角色”模型，“地图角色”其中又存储了角色实体，和其对应的会话session
    // 存储了地图自身的配置数据
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                connection = conn;
                character = cha;
            }
        }

        internal MapDefine Define;

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        public int ID
        {
            get
            {
                return Define.ID;
            }
        }

        internal Map(MapDefine define)
        {
            Define = define;
        }

        internal void Update()
        {

        }

        // 当一个角色进入该地图时，添加管理该角色，给地图上所有角色发送角色进入地图的响应
        internal void CharacterEnter(NetConnection<NetSession> conn,Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", Define.ID, character.Id);
            
            character.Info.mapId = ID;

           
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Info);
            foreach (var kv in MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                SendCharacterEnterMap(kv.Value.connection,character.Info);
            }

            this.MapCharacters.Add(character.Id, new MapCharacter(conn, character));

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data,0,data.Length);
        }

        private void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = ID;
            message.Response.mapCharacterEnter.Characters.Add(character);
            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
    }
}
