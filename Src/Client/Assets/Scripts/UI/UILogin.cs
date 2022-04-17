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

    void OnLogin()
    {

    }
}
