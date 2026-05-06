using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.GFW;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySlot : MonoBehaviour
{
    public int SlotIndex; // 在 Inspector 里分别填 0, 1, 2

    [SerializeField] private Button btnAdd;      // 那个“+”号按钮
    //[SerializeField] private GirlItem girlItem;  // 你之前的角色卡脚本
    private IGFWDemoModel mModel;
    private ResLoader mResLoader = ResLoader.Allocate();
    private GameObject girlItem;
    public static event Action<bool> bPush;
    public void Awake()
    {
        mModel = GFWDemo.Interface.GetModel<IGFWDemoModel>();
    }

    private void Start()
    {
        var itemPrefab= mResLoader.LoadSync<GameObject>("GirlItem");
        girlItem = Instantiate(itemPrefab, this.transform, false);
        var rt = girlItem.GetComponent<RectTransform>();
        if (rt != null)
        {
            // 确保锚点和中心点正确的情况下，位置归零即是对齐中心
            rt.anchoredPosition = Vector2.zero;
        }
        // 1. 监听数据变化 (核心：响应式 UI)
        mModel.SlotInfos[SlotIndex].RegisterWithInitValue(info =>
        {
            var view = girlItem.GetComponent<GirlItem>();
            if (info == null)
            {
                btnAdd.gameObject.SetActive(true);
                girlItem.gameObject.SetActive(false);
            }
            else
            {
                btnAdd.gameObject.SetActive(false);
                girlItem.gameObject.SetActive(true);
                view.InitInfo(info,GirlsPanlType.Display, _ => OnSlotClick()); // 调用你之前的异步加载逻辑
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        // 2. 绑定点击逻辑
        // 无论是点“+”还是点已有的卡片，都打开选人面板
        btnAdd.onClick.AddListener(OnSlotClick);
    }

    private void OnSlotClick()
    {
        // 打开选人面板，并告诉它我要填哪个坑
        UIKit.OpenPanel<GirlsSelectPanel>(new GirlsSelectPanelData {
            TargetIndex = SlotIndex 
        });
        bPush?.Invoke(true);
    }

    private void OnDestroy()
    {
        mResLoader.Recycle2Cache();
        mResLoader = null;
    }
}
