using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// 存储角色协议信息
// 响应选择交互
// 展示角色信息
public class UICharInfo : MonoBehaviour
{
    public SkillBridge.Message.NCharacterInfo info;

    public Text charClass;
    public Text charName;
    public Image highlight;

    public bool Selected
    {
        get
        {
            return highlight.IsActive();
        }
        set
        {
            highlight.gameObject.SetActive(true);
        }

    }

    // Use this for initialization
    void Start()
    {
        if (info != null)
        {
            charClass.text = info.Class.ToString();
            charName.text = info.Name;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
