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
    class UserService: Singleton<UserService>
    {

        // 监听用户注册请求后服务端的响应消息，然后做一些操作
        // 在dispose的时候，取消用户注册的监听
        // 监听服务器网络连接和断连的消息


        public UnityEngine.Events.UnityAction<Result, string> OnLogin;

        NetMessage pendingMessage = null;

        bool connected = false; // todo 为啥要还需要这个判断连接，不能直接用网络连接判断么？

        public UserService()
        {
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(OnUserLogin); // todo 这个为什么不用像服务端一样指定一个发送者
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
        }

 
        public void Dispose() // todo 这个什么时候会调用？
        {

            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(OnUserLogin);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;

        }


        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start "); // todo这个会调用log4net吗？
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        public void SendLogin(string user,string psw)
        {
            // 判断当前是否已经有网络连接
            // 如果有，则直接发送消息
            // 如果没有，则将消息放到队列里，并且发起一次网络连接

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
                if (!this.DisconnectNotify(result,reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason),"错误",MessageBoxType.Error);
                }
            }

        }

        private bool DisconnectNotify(int result, string reason)
        {
            // 判断是否有消息在排队
            // 如果有，判断是否有登录请求的消息
            // 如果有，判断是否有登录监听
            // 如果有，则发送一个失败的消息
            if (pendingMessage!=null)
            {
                if (pendingMessage.Request.userLogin != null)
                {
                    if (OnLogin != null)
                    {
                        OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}",result,reason));
                    }
                }
                return true;
            }
            return false;
        }


        private void OnGameServerDisconnect(int result, string reason)
        {
            // 进行断连通知
            DisconnectNotify(result,reason);
        }


        private void OnUserLogin(object sender, UserLoginResponse message)
        {
            // 保存用户数据
            // 如果有监听者注册了监听，则发送登录成功的消息
            Debug.LogFormat("OnLogin:{0} [{1}]", message.Result, message.Errormsg);

            if(message.Result == Result.Success)
            {
                // 保存用户数据到user model里
            }

            if (OnLogin != null)
            {
                OnLogin(message.Result, message.Errormsg);
            }
            
        }

    }
}
