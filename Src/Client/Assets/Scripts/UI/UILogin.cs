using System;
using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;


class UILogin: MonoBehaviour
{
    public InputField username;
    public InputField password;

    void Start()
    {
        UserService.Instance.OnLogin = OnLogin;
    }


    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        // Enter Game
        UserService.Instance.SendLogin(this.username.text, this.password.text);

    }

    private void OnLogin(Result result, string message)
    {
        // 登录结果是否为成功
        // 如果为成功，则进行角色选择界面
        // 如果没有成功，用消息盒子进行用户反馈
        if(result == Result.Success)
        {
            // 这里暂时用消息盒子反馈成功消息
            MessageBox.Show("登录成功，准备进行角色选择" + message, "提示", MessageBoxType.Information);
        }
        else
        {
            MessageBox.Show(message, "提示", MessageBoxType.Error);
        }
    }

}
