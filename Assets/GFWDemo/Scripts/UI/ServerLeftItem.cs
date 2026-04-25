using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerLeftItem : MonoBehaviour
{
    public Button LeftItemButton;
    public TMP_Text LeftItemText;
    public TMP_Text nowselectText;

    public Image lineIma;

    public ServerType MyType;

    private void Awake()
    {
        nowselectText.Hide();
    }

    void Start()
    {

    }

    public void InitInfo(ServerType type,bool islast,IGFWDemoModel mModel)
    {
        LeftItemButton.onClick.AddListener(() =>
        {
            GFWDemo.Interface.SendCommand(new selectedServerTypeCommand(type));
        } );
        MyType = type;
        //把区间显示的内容 更新了
        LeftItemText.text = type.GetDescription();
        bool bselected = (mModel.selectedServerType.Value == MyType);
        SetSelectState(bselected);
        if(islast) lineIma.DestroySelf();
    }
    public void SetSelectState(bool isSelected)
    {
        // 核心：控制文本的显示和隐藏
        if (isSelected)
        {
            nowselectText.Show();
        }
        else
        {
            nowselectText.Hide();
        }

    }
}
