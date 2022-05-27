using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Models;
using SkillBridge.Message; // todo 引入整个命名空间和当个引用的区别？

// 初始化角色选择面板和创建面板
// 创建角色与结果响应
// 选择角色与结果响应
// 进入游戏
public class UICharacterSelect : MonoBehaviour
{
    public GameObject penalCreate;
    public GameObject penalSelect;

    public List<GameObject> uiChars = new List<GameObject>();

    public Transform uiCharList; // todo 为啥是transform类型？指的是父节点吗
    public GameObject uiCharInfo;

    private int selectCharacterIdx = -1; // todo 这里不是对象不能是null，而ts里的都是引用？

    public UICharacterView characterView;

    private CharacterClass charClass;

    // Use this for initialization
    void Start()
    {

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
            // 根据服务器返回的用户信息展示角色信息
            foreach (var old in uiChars)
            {
                Destroy(old); // todo 为什么要先清掉游戏对象，不会自己回收吗？
            }
            uiChars.Clear();
            // 监听角色的选择，激活某个角色的显示

            List<NCharacterInfo> userCharacters = User.Instance.Info.Player.Characters;
            for (int i = 0; i < userCharacters.Count; i++)
            {
                GameObject go = Instantiate(uiCharInfo, uiCharList);
                UICharInfo charInfo = go.GetComponent<UICharInfo>();
                charInfo.info = userCharacters[i];

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
        OnSelectClass(1); // todo
    }

    void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;

        characterView.CurrentCharacter = charClass - 1;

        for (int i = 0; i < 3; i++)
        {

        }
    }

    void OnClickCreate()
    {

    }


    void OnCharacterCreate()
    {

    }

    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {
            MessageBox.Show("进入游戏", "进入游戏", MessageBoxType.Confirm);
        }
    }
}
