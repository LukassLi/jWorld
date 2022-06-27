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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin); // todo 接收到的消息中，带有一个sender对象，用于发送响应，为什么这样设计？而不用单例等
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.onRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
        }

        public void Init()
        {
            // todo
        }

        private void onRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            // 创建一个响应消息
            // 判断数据库里是否已经存在用户
            // 如果已经存在，则返回失败消息
            // 如果不存在，创建一个用户，并返回成功消息
            // 打包发送消息
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer()); // todo 同时得增加一个player，user-player-character？
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player }); // todo 为什么赋值是在花括号里的
                DBService.Instance.Entities.SaveChanges(); // todo 这一步是不是破坏封装了

                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);

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

                // message中填入用户信息和角色信息（角色信息返回给客户端是为了可以显示和选择角色）
                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "None";
                NUserInfo userInfo = new NUserInfo();
                userInfo.Id = 1; // todo 这个id是拿来干嘛的？是消息的唯一标识id么
                userInfo.Player = new NPlayerInfo();
                userInfo.Player.Id = user.Player.ID; // todo 这个id是拿来干嘛的？
                foreach (var c in user.Player.Characters)
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

        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            // 构建一个TCharacter，存入数据库里，同时更新会话中的用户数据，并发送一个成功响应给客户端（一般视为成功）
            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1, // todo 第一次是主城
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
            };

            DBService.Instance.Entities.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();
            sender.Session.User.Player.Characters.Add(character);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();
            message.Response.createChar.Errormsg = "None";
            message.Response.createChar.Result = Result.Success;
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
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
