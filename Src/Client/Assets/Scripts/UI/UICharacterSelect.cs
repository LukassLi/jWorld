using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Models;
using SkillBridge.Message; // todo 引入整个命名空间和当个引用的区别？
using Services;
using System;

// 初始化角色选择面板和创建面板
// 创建角色与结果响应
// 选择角色与结果响应
// 进入游戏
public class UICharacterSelect : MonoBehaviour
{
    public GameObject penalCreate;
    public GameObject penalSelect;

    private List<GameObject> uiChars = new List<GameObject>();

    public Transform uiCharList; // todo 为啥是transform类型？指的是父节点吗
    public GameObject uiCharInfo;

    private int selectCharacterIdx = -1; // todo 这里不是对象不能是null，而ts里的都是引用？

    public UICharacterView characterView;

    public Image[] titles;

    public Text[] names;

    public Text descs;

    public InputField charName;

    private CharacterClass charClass;

    // Use this for initialization
    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCreateCharacter = OnCharacterCreate;
    }

    private void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitCharacterSelect(bool init)
    {
        // 展示角色选择面板
        penalCreate.SetActive(false);
        penalSelect.SetActive(true);

        if (init)
        {
            // 清掉原有的角色UI对象
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();

            // 根据服务器返回的用户信息，创建角色UI信息
            List<NCharacterInfo> userCharacters = User.Instance.Info.Player.Characters;
            for (int i = 0; i < userCharacters.Count; i++)
            {
                GameObject go = Instantiate(uiCharInfo, uiCharList);
                UICharInfo charInfo = go.GetComponent<UICharInfo>();
                charInfo.info = userCharacters[i];

                // 监听角色的选择，激活某个角色的显示
                Button btn = go.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    OnSelectCharacter(i);
                });
                uiChars.Add(go);
                go.SetActive(true);
            }

        }
    }


    void OnSelectCharacter(int idx)
    {
        // 缓存当前选择的角色序号
        selectCharacterIdx = idx;

        // 找到当前选择角色对应的角色数据信息，并打印出来
        var cha = User.Instance.Info.Player.Characters[idx]; // 用idx去找，而不是去ui char info中取刚才赋的值，因为设计上功能相互是不依赖的
        Debug.LogFormat("Select Char :[{0}]{1}[{2}]", cha.Id, cha.Class, cha.Name);

        // 赋值用户数据里的当前角色属性
        User.Instance.CurrentCharacter = cha;

        // 指定角色展示面板的角色为当前角色
        characterView.CurrentCharacter = idx;

        // 高亮选择面板的当前选择角色
        for (int i = 0; i < uiChars.Count; i++)
        {
            uiChars[i].GetComponent<UICharInfo>().Selected = i == idx;
        }

    }

    public void InitCharacterCreate()
    {
        penalCreate.SetActive(true);
        penalSelect.SetActive(false);
        OnSelectClass(1);
    }

    /// <summary>
    /// 选择职业
    /// </summary>
    /// <param name="charClass"></param>
    void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;

        characterView.CurrentCharacter = charClass - 1;

        // todo 目前只有3个职业
        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == charClass - 1);
            names[i].text = DataManager.Instance.Characters[i + 1].Name; // 这个name是职业的name，和玩家自定义角色的name不一样
        }
        descs.text = DataManager.Instance.Characters[charClass].Description; 
    }

    public void OnClickCreate()
    {
        if (string.IsNullOrEmpty(charName.text))
        {
            MessageBox.Show("请输入角色名称");
            return;
        }
        UserService.Instance.SendCharacterCreate(charName.text, charClass);
    }


    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {
            MessageBox.Show("进入游戏", "进入游戏", MessageBoxType.Confirm);
        }
    }
}
