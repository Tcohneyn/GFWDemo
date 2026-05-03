using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using Unity.VisualScripting;

namespace QFramework.GFW
{
    public class PlayerInfoPanelData : UIPanelData
    {
    }

    public partial class PlayerInfoPanel : UIPanel
    {
        private IGFWDemoModel mModel;
        public LanguageTMPFontAsset LanguageFonts = new LanguageTMPFontAsset();
        private TMP_FontAsset mDefaultFont;
        private enum PlayInfoTabKind
        {
            None,
            Display,
            Setting
        }

        private PlayInfoTabKind _playInfoTab = PlayInfoTabKind.None;
        private ResLoader mResLoader = ResLoader.Allocate();

        protected override void OnInit(IUIData uiData = null)
        {
            mModel = GFWDemo.Interface.GetModel<IGFWDemoModel>();
            mData = uiData as PlayerInfoPanelData ?? new PlayerInfoPanelData();
            // please add init code here
            btnback.onClick.AddListener(() => { this.Back(); });
            mModel.mCurrentLanguage.RegisterWithInitValue(language => { RefreshPlayInfoLabels(); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            ToggleGroupManager.OnToggleChanged += OnToggleChanged;
            lanToggleGroupManager.OnToggleChanged += OnlanToggleChanged;
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
            // Start 里 Toggle 初始化可能早于本面板 OnInit 订阅，这里按当前选中 Tab 补同步一次
            var mainTabMgr = GetComponentInChildren<ToggleGroupManager>(true);
            if (mainTabMgr != null)
            {
                var sel = mainTabMgr.GetSelectedToggle();
                if (sel != null)
                    ApplyMainTabToggle(sel);
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            // ⭐ 关键：必须在这里减掉监听，否则会导致残留
            ToggleGroupManager.OnToggleChanged -= OnToggleChanged;
            lanToggleGroupManager.OnToggleChanged -= OnlanToggleChanged;

            // 清理资源加载器
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }

        private void RefreshPlayInfoLabels()
        {
            var lang = mModel.mCurrentLanguage.Value;
            mDefaultFont = mResLoader.LoadSync<TMP_FontAsset>("FZZCHJW #3275 SDF");
            var TmpFontAsset = mResLoader.LoadSync<TMP_FontAsset>("日本濑户可爱风 SDF");
            if (lang == Language.Japanese)
            {
                ApplyFontForLanguage(lang, textTopleft,TmpFontAsset);
                ApplyFontForLanguage(lang, textBottomleft,TmpFontAsset);;
            }
            else
            {
                textTopleft.font = mDefaultFont;
                textBottomleft.font = mDefaultFont;
            }
            switch (_playInfoTab)
            {
                case PlayInfoTabKind.Display:
                    textTopleft.text = L.DisplayTop(lang);
                    textBottomleft.text = L.DisplayBottom(lang);
                    break;
                case PlayInfoTabKind.Setting:
                    textTopleft.text = L.SettingTop(lang);
                    textBottomleft.text = L.SettingBottom(lang);
                    break;
                default:
                    break;
            }
            UILayoutRefresh.RefreshAll();
        }

        private void ApplyMainTabToggle(Toggle toggle)
        {
            if (toggle == null) return;
            string togname = toggle.name;
            ClearGeneratedItems();
            if (togname == "displayToggle")
            {
                _playInfoTab = PlayInfoTabKind.Display;
                var displayPrefab = mResLoader.LoadSync<GameObject>("Display");
                Instantiate(displayPrefab, playInfoContainer, false);
            }
            else if (togname == "settingToggle")
            {
                _playInfoTab = PlayInfoTabKind.Setting;
                var settingPrefab = mResLoader.LoadSync<GameObject>("Setting");
                Instantiate(settingPrefab, playInfoContainer, false);
            }
            else
            {
                _playInfoTab = PlayInfoTabKind.None;
            }

            RefreshPlayInfoLabels();
        }

        protected void OnToggleChanged(Toggle toggle)
        {
            ApplyMainTabToggle(toggle);
        }

        protected void OnlanToggleChanged(Toggle toggle)
        {
            String togname = toggle.name;
            if (togname == "cnToggle")
            {
                GFWDemo.Interface.SendCommand(new ChangeLanguageCommand(Language.ChineseSimplified));
            }
            else if (togname == "enToggle")
            {
                GFWDemo.Interface.SendCommand(new ChangeLanguageCommand(Language.English));
            }
            else if (togname == "jpToggle")
            {
                GFWDemo.Interface.SendCommand(new ChangeLanguageCommand(Language.Japanese));
            }
        }

        private void ClearGeneratedItems()
        {
            // 假设 SubListContainer 是你存放那些二级菜单 Item 的父物体
            if (playInfoContainer)
            {
                playInfoContainer.transform.DestroyChildren();
            }
        }

        private void ApplyFontForLanguage(Language language, TMP_Text mTmpText, TMP_FontAsset mTmpFontAsset)
        {
            LanguageFonts.Language = language;
            LanguageFonts.FontAsset = mTmpFontAsset;
            if (mTmpText == null || LanguageFonts == null)
            {
                return;
            }

            if (LanguageFonts.Language == language && LanguageFonts.FontAsset)
            {
                var entry = LanguageFonts.FontAsset;
                if (entry != null)
                {
                    mTmpText.font = entry;
                }
            }
        }
    }

    static class L
    {
        public static string DisplayTop(Language lang) => T(lang,
            zh: "身份信息",
            en: "Identity Info",
            ja: "身分証情報");

        public static string DisplayBottom(Language lang) => T(lang,
            zh: "角色卡展示",
            en: "Character Card Display",
            ja: "キャラカード表示");

        public static string SettingTop(Language lang) => T(lang,
            zh: "声音语言设置",
            en: "Audio & language",
            ja: "音声・言語設定");

        public static string SettingBottom(Language lang) => T(lang,
            zh: "画面设置",
            en: "Graphic Settings",
            ja: "画面設定");

        private static string T(Language lang, string zh, string en, string ja)
        {
            switch (lang)
            {
                case Language.ChineseSimplified:
                    return zh;
                case Language.Japanese:
                    return ja;
                case Language.English:
                default:
                    return en;
            }
        }
    }
}