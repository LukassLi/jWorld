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
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;

        NetMessage pendingMessage = null;

        bool connected = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServiceConnect;
            NetClient.Instance.OnDisconnect += OnGameServiceDisconnect;
        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start "); // todo这个会写到哪呢？
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }

        void OnGameServiceConnect(int result, string reason)
        {
            //Log.InfoFormat("LoadingMessage::OnGameServiceConnect result:{0} reason:{1}", result, reason);
            //if (NetClient.Instance.Connected)
            //{
            //    connected = true;
            //    if (this.pengdingMesage != null)
            //    {


            //    }

            //}


        }

        void OnGameServiceDisconnect(int result,string reason)
        {

        }

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
            }
            else
            {
                this.pendingMessage = msg;
                this.ConnectToServer();
            }
        }

    }
}
