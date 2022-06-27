using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;

namespace GameServer
{
    class GameServer
    {
        Thread thread;
        bool running = false;
        NetService network;
        public bool Init()
        {
            int port = 8000;
            network = new NetService();
            network.Init(port);

            // 各种服务初始化
            DBService.Instance.Init();
            DataManager.Instance.Load();
            UserService.Instance.Init();
            MapService.Instance.Init();

            //DBService.Instance.Init();
            // db是否成功可以在这里打印一个数据字段看看

            thread = new Thread(new ThreadStart(this.Update));

            SkillBridge.Message.UserRegisterRequest ur = new SkillBridge.Message.UserRegisterRequest();
            ur.Age = 5;

            return true;
        }

        public void Start()
        {
            network.Start();
            HelloworldService.Instance.Start();
            running = true;
            thread.Start();
        }


        public void Stop()
        {
            network.Stop();
            running = false;
            thread.Join();
        }

        public void Update()
        {
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
            }
        }
    }
}
