using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using log4net;

public static class UnityLogger
{
    public static void Init()
    {
        Application.logMessageReceived += OnLogMessageReceived;
        Common.Log.Init("Unity");
    }

    private static ILog log = LogManager.GetLogger("Unity");

    private static void OnLogMessageReceived(string condition,string stackTrace,UnityEngine.LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                // \r\n 连用，表示跳到下一行，并且返回到下一行的起始位置
                log.ErrorFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Assert:
                log.DebugFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Exception:
                log.FatalFormat("{0\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Warning:
                log.WarnFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            default:
                log.Info(condition);
                break;

        }
    }

}
