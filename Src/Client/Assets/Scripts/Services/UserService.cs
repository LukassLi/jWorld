using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Common;
using SkillBridge.Message;

namespace Services
{
    // 发送登录请求
    // 监听用户登录请求后服务端的响应消息，然后做一些操作
    // 在dispose的时候，取消用户登录的监听

    // 发送注册请求
    // 监听注册响应
    // 取消监听

    // 发送创建角色请求
    // 监听创建角色响应
    // 取消监听

    // 监听服务器网络连接和断连的消息
    // 取消监听
    class UserService : Singleton<UserService>
    {
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;

        public UnityEngine.Events.UnityAction<Result, string> OnRegister;

        public UnityEngine.Events.UnityAction<Result, string> OnCreateCharacter;


        NetMessage pendingMessage = null;

        bool connected = false; // todo 为啥要还需要这个判断连接，不能直接用网络连接判断么？

        public UserService()
        {
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(OnUserLogin); // todo 这个为什么不用像服务端一样指定一个发送者
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(OnGameEnter);
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
        }


        public void Dispose() // todo 这个什么时候会调用？
        {

            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(OnGameEnter);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;

        }


        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start "); // todo这个会调用log4net吗？
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        private void OnGameServerConnect(int result, string reason)
        {
            // 判断网络连接是否连接上了
            // 如果连接上，则判断是否有消息在排队，如果有，发送并清空
            // 如果没有连上，判断是否需要进行断连通知，如果不需要，则用消息盒子提示网络错误消息
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if (pendingMessage != null)
                {
                    NetClient.Instance.SendMessage(pendingMessage);
                    pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }

        }

        private void OnGameServerDisconnect(int result, string reason)
        {
            // 进行断连通知
            DisconnectNotify(result, reason);
        }

        private bool DisconnectNotify(int result, string reason)
        {
            // 判断是否有消息在排队
            // 如果有，判断是否有登录请求的消息
            // 如果有，判断是否有登录监听
            // 如果有，则发送一个失败的消息
            if (pendingMessage != null)
            {
                if (pendingMessage.Request.userLogin != null)
                {
                    if (OnLogin != null)
                    {
                        OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        // 判断当前是否已经有网络连接
        // 如果有，则直接发送消息
        // 如果没有，则将消息放到队列里，并且发起一次网络连接
        public void SendLogin(string user,string psw)
        {

            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.userLogin = new UserLoginRequest();
            msg.Request.userLogin.User = user;
            msg.Request.userLogin.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(msg);
            }else
            {
                this.pendingMessage = msg;
                this.ConnectToServer();
            }
        }


        private void OnUserLogin(object sender, UserLoginResponse response)
        {
            // 保存用户数据
            // 如果有监听者注册了监听，则发送登录成功的消息
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                // 保存用户数据到user model里
                Models.User.Instance.SetupUserInfo(response.Userinfo);
            }

            if (OnLogin != null)
            {
                OnLogin(response.Result, response.Errormsg);
            }

        }

        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        private void OnUserRegister(object sender, UserRegisterResponse response)
        {
            // 保存用户数据
            // 如果有监听者注册了监听，则发送登录成功的消息
            Debug.LogFormat("OnRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (OnRegister != null)
            {
                OnRegister(response.Result, response.Errormsg);
            }

        }

        public void SendCharacterCreate(string name, CharacterClass cls)
        {
            Debug.LogFormat("UserCharacterCreateRequest::name :{0} cls:{1}", name, cls);
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.createChar = new UserCreateCharacterRequest();
            msg.Request.createChar.Class = cls; // 这个是用来约定角色类型的，而角色类型在业务上用于玩家交互
            msg.Request.createChar.Name = name;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(msg);
            }
            else
            {
                this.pendingMessage = msg;
                this.ConnectToServer();
            }
        }

        // 判断是否成功创建
        // 如果成功，更新model中的角色数据（角色计算，增删改，该策略是在服务端做，客户端只是数据缓存，不做重复的计算。而为了保证缓存数据是即时正确的，当服务端更新数据时，缓存也要随着更新）
        // 分发结果消息
        private void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if(response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear(); // todo addrange和clear哪里来的
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }

            if (OnCreateCharacter != null)
            {
                OnCreateCharacter(response.Result, response.Errormsg);
            }

        }

        public void SendGameEnter(int characterIdx)
        {
            Debug.LogFormat("SendGameEnter::characterIdx :{0}", characterIdx);
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.gameEnter = new UserGameEnterRequest();
            msg.Request.gameEnter.characterIdx = characterIdx;
            NetClient.Instance.SendMessage(msg);
        }


        private void OnGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                // todo
            }
        }

    }
}
