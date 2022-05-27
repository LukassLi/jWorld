using UnityEngine;
using System.Collections;

// 展示所有角色
// 高亮当前选中的角色
public class UICharacterView : MonoBehaviour
{
    public GameObject[] characters;

    private int currentCharacter = 0;

    public int CurrentCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
            UpdataCharacter();
        }

    }

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdataCharacter()
    {

        for (int i = 0; i < 3; i++)
        {
            characters[i].SetActive(i == currentCharacter);
        }
    }
}
