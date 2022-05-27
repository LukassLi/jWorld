using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Services;
using SkillBridge.Message;
using System;


// 监听UI交互事件
// 监听注册结果
// 关闭注册面板
public class UIRegister : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public InputField passwordConfirm;
    public Button btnRegister;
    public GameObject uiLogin;

    // Use this for initialization
    void Start()
    {
        UserService.Instance.OnRegister = OnRegister;
    }



    // Update is called once per frame
    void Update()
    {

    }

    // 判断用户名输入，如果为空，显示相应信息并返回
    // 判断密码输入，如果为空，～～～
    // 判断密码确认，如果为空，～～～
    // 判断两次密码输入一致性，如果不一致，～～～
    // 若都没有问题的话，发送注册请求
    public void OnClickRegister()
    {
        if (string.IsNullOrEmpty(username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (string.IsNullOrEmpty(passwordConfirm.text))
        {
            MessageBox.Show("请输入确认密码");
            return;
        }
        if (password.text!=passwordConfirm.text)
        {
            MessageBox.Show("两次输入的密码不一致");
            return;
        }
        UserService.Instance.SendRegister(username.text, password.text);
    }

    private void OnRegister(Result result, string message)
    {
        // 判断是否注册成功
        // 如果成功，则显示成功消息弹窗
        // 如果不成功，则显示错误消息弹窗

        if(result == Result.Success)
        {
            MessageBox.Show("注册成功，请登录", "提示", MessageBoxType.Information).OnYes = CloseRegister;
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }

    }

    private void CloseRegister()
    {
        gameObject.SetActive(false);
        uiLogin.SetActive(true);
    }
}

