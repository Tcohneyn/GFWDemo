using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.GFW;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GirlItem : MonoBehaviour
{
    private static readonly Dictionary<AttributeType, string> sAttributeSpriteNames =
        new Dictionary<AttributeType, string>
        {
            { AttributeType.Immune, "recommend_icon_white" },
            { AttributeType.Erosive, "recommend_icon_black" },
            { AttributeType.Biological, "recommend_icon_bio" },
            { AttributeType.Mechanical, "recommend_icon_mech" },
            { AttributeType.Psionic, "recommend_icon_psi" },
        };

    private static readonly Dictionary<WeaponType, string> sWeaponTypeSpriteNames = new Dictionary<WeaponType, string>
    {
        { WeaponType.HG, "commonD_icon_HG" },
        { WeaponType.FT, "commonD_icon_FT" },
        { WeaponType.SR, "commonD_icon_SR" },
        { WeaponType.SG, "commonD_icon_SG" },
        { WeaponType.AR, "commonD_icon_AR" },
    };
    [SerializeField] private Button GirlButton;
    [SerializeField] private Image imgAttribute;
    [SerializeField] private GameObject Stars;
    [SerializeField] private Image WeaponIcon;
    [SerializeField] private Image ImageLock;
    [SerializeField] private TMP_Text textLV;
    [SerializeField] private Image bgBorder;
    [SerializeField] private Image girl;
    [SerializeField] private Image imgSelect;
    [Header("立绘布局")] [Tooltip("可选：Mask/裁剪区下的空物体 RectTransform；位移缩放施加在该节点上（推荐）。留空则直接改 girl")] [SerializeField]
    private RectTransform portraitLayoutRoot;

    private GirlsPanel mGirlsPanel;
    private ResLoader mResLoader = ResLoader.Allocate();
    private int mPortraitLoadVersion;
    private RectTransform PortraitLayoutRect => portraitLayoutRoot != null ? portraitLayoutRoot : girl.rectTransform;
    private bool UsePanelPortraitFade => mGirlsPanel != null && mGirlsPanel.HasCanvasGroup;

    private void Awake()
    {
        ImageLock.Hide();
        textLV.text = string.Empty;
        mGirlsPanel = GetComponentInParent<GirlsPanel>();
        if (!UsePanelPortraitFade)
        {
            SetPortraitImageVisible(false);
        }
    }

    private void OnDestroy()
    {
        mResLoader.Recycle2Cache();
        mResLoader = null;
    }

    public void InitInfo(GirlsInfo info, GirlsPanlType type, Action<GirlsInfo> onSelected = null)
    {
        mPortraitLoadVersion++;

        var usePanelFade = UsePanelPortraitFade;
        if (usePanelFade)
        {
            mGirlsPanel.RegisterPortraitLoadPending();
        }
        else
        {
            SetPortraitImageVisible(false);
        }

        if (info.rarity >= 1 && info.rarity <= 4)
        {
            ApplyRarityContent(info, info.rarity, type);
        }

        girl.sprite = null;

        var loadToken = mPortraitLoadVersion;
        mResLoader.Add2Load<Sprite>(info.girlImage);
        mResLoader.LoadAsync(() =>
        {
            try
            {
                if (loadToken != mPortraitLoadVersion)
                {
                    return;
                }

                girl.sprite = mResLoader.LoadSync<Sprite>(info.girlImage);
                ApplyPortraitTransform(info, PortraitLayoutRect);
                if (!usePanelFade)
                {
                    SetPortraitImageVisible(true);
                }
            }
            finally
            {
                if (usePanelFade)
                {
                    mGirlsPanel.NotifyPortraitLoadDone();
                }
            }
        });
        // 设置按钮逻辑
        if (type == GirlsPanlType.GirlsSelectPanel)
        {
            if (GirlButton != null)
            {
                GirlButton.onClick.RemoveAllListeners(); // 清理旧监听
                if (onSelected != null)
                {
                    // 点击时，把自己的 info 传回给 Panel
                    GirlButton.onClick.AddListener(() => onSelected(info));
                }
            }
        }
        else if (type == GirlsPanlType.Display)
        {
            if (GirlButton != null)
            {
                GirlButton.onClick.RemoveAllListeners(); // 清理旧监听
                if (onSelected != null)
                {
                    // 点击时，把自己的 info 传回给 Panel
                    GirlButton.onClick.AddListener(() => onSelected(info));
                }
            }
        }
    }

    private static void SetPortraitImageVisible(Image image, bool visible)
    {
        var c = image.color;
        c.a = visible ? 1f : 0f;
        image.color = c;
    }

    private void SetPortraitImageVisible(bool visible)
    {
        SetPortraitImageVisible(girl, visible);
    }

    private static void ApplyPortraitTransform(GirlsInfo info, RectTransform portraitRt)
    {
        portraitRt.anchoredPosition = new Vector2(info.portraitOffsetX, info.portraitOffsetY);
        var s = info.portraitScale > 0f ? info.portraitScale : 1f;
        portraitRt.localScale = new Vector3(s, s, 1f);
    }

    private void ApplyRarityContent(GirlsInfo info, int rarity, GirlsPanlType type)
    {
        ClearStarChildren();

        bgBorder.sprite = mResLoader.LoadSync<Sprite>($"commonD_bg_order{rarity}");

        mResLoader.Add2Load("StarItem", (succeed, res) =>
        {
            if (!succeed)
            {
                return;
            }

            var starPrefab = res.Asset as GameObject;
            if (starPrefab == null)
            {
                return;
            }

            for (var i = 0; i < rarity; i++)
            {
                Instantiate(starPrefab, Stars.transform, false);
            }
        });
        if (type == GirlsPanlType.GirlsPanel)
        {
            if (info.isLock)
            {
                ImageLock.Show();
            }
            else
            {
                ImageLock.Hide();
            }
        }

        if (type == GirlsPanlType.GirlsSelectPanel)
        {
            if (info.isSelected)
            {
                imgSelect.Show();
            }
            else
            {
                imgSelect.Hide();
            }
        }
        if (sAttributeSpriteNames.TryGetValue(info.attribute, out var spriteName))
        {
            imgAttribute.sprite = mResLoader.LoadSync<Sprite>(spriteName);
        }

        if (sWeaponTypeSpriteNames.TryGetValue(info.weapon, out var spritename))
        {
            WeaponIcon.sprite = mResLoader.LoadSync<Sprite>(spritename);
        }

        textLV.text = $"LV.{info.level}";
    }

    private void ClearStarChildren()
    {
        if (Stars == null)
        {
            return;
        }

        var t = Stars.transform;
        for (var i = t.childCount - 1; i >= 0; i--)
        {
            Destroy(t.GetChild(i).gameObject);
        }
    }
}