using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ServerRightItem : MonoBehaviour
{
    public Button RightItemButton;
    public TMP_Text RightItemText;
    public TMP_Text ServerTypeText;
    public TMP_Text msText;

    public Image imgTip;
    private GFWServerInfo selfserverInfo;
    
    private ResLoader mResLoader = ResLoader.Allocate();

    // Start is called before the first frame update
    void Awake()
    {
        imgTip.Hide();
    }

    void Start()
    {

    }

    public void InitInfo(GFWServerInfo info)
    {
        RightItemButton.onClick.AddListener(() =>
        {
            GFWDemo.Interface.SendCommand(new selectedServerCommand(info));
            UIKit.ClosePanel("ServerPanel");
        });
        RightItemText.text = info.name;
        ServerTypeText.text = info.Type.GetDescription();
        msText.text = info.ms.ToString()+"ms";
        var spriteAtlas = mResLoader.LoadSync<SpriteAtlas>("ServerTip");
        if (info.ms <= 30)
        {
            msText.color = Color.green;
            imgTip.Show();
            imgTip.sprite = spriteAtlas.GetSprite("sactx-128x128-ETC2-UI_ServerList-fd6948ba_0");
        }
        else if (info.ms > 30 && info.ms <= 100)
        {
            msText.color = Color.yellow;
            if (info.isNew)
            {
                imgTip.Show();
                imgTip.sprite = spriteAtlas.GetSprite("sactx-128x128-ETC2-UI_ServerList-fd6948ba_2");
            }
        }
        else
        {
            msText.color = Color.red;
            if (info.isNew)
            {
                imgTip.Show();
                imgTip.sprite = spriteAtlas.GetSprite("sactx-128x128-ETC2-UI_ServerList-fd6948ba_2");
            }
        }
        selfserverInfo = info;
    }
}