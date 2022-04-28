using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class UserService: Singleton<UserService>
    {
        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);

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
    }
}
