using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class UserService: Singleton<UserService>
    {
        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
        }

        private void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if(user == null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户不存在";
            }
            else if(user.Password!=user.Password)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                // session缓存当前用户
                sender.Session.User = user;
                // 构建用户信息
                // 同时构建角色信息
                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "None";
                NUserInfo userInfo = new NUserInfo();
                userInfo.Id = 1; // todo 为什么为1？
                userInfo.Player = new NPlayerInfo();
                userInfo.Player.Id = user.Player.ID;
                foreach(var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Class = (CharacterClass)c.Class;
                    info.Name = c.Name;
                    userInfo.Player.Characters.Add(info);
                }
                message.Response.userLogin.Userinfo = userInfo;
                // 将信息装箱发送
                byte[] data = PackageHandler.PackMessage(message);
                sender.SendData(data,0,data.Length);
            }
            
        }


        // 角色管理器中添加角色实体
        // 成功后，发送一个成功响应给客户端
        // 地图管理器进入角色实体
        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbchar);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameEnter = new UserGameEnterResponse();
            message.Response.gameEnter.Errormsg = "None";
            message.Response.gameEnter.Result = Result.Success;
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data,0,data.Length);
            sender.Session.Character = character;

            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);
        }
    }
}
