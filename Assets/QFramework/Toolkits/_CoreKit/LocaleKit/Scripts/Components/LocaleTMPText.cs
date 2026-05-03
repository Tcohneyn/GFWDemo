using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 按语言配置的 TMP 字体资源（与 <see cref="LocaleTMPText.LanguageFonts"/> 配套使用）。
    /// </summary>
    [System.Serializable]
    public class LanguageTMPFontAsset
    {
        public Language Language;
        public TMP_FontAsset FontAsset;
    }

    /// <summary>
    /// 与 <see cref="LocaleText"/> 相同的多语言列表配置，用于 TextMeshPro（<see cref="TMP_Text"/>）。
    /// 挂在含 TMP_Text / TextMeshProUGUI 的对象上即可。
    /// 可选：在 <see cref="LanguageFonts"/> 中为每种语言指定 <see cref="TMP_FontAsset"/>，切换语言时会同步换字体。
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocaleTMPText : AbstractLocaleText
    {
        [Tooltip("若配置了某项且 FontAsset 非空，则在切换到对应语言时替换 TMP 字体。")]
        public List<LanguageTMPFontAsset> LanguageFonts = new List<LanguageTMPFontAsset>();

        private TMP_Text mTmpText;

        private void Awake()
        {
            mTmpText = GetComponent<TMP_Text>();

            LocaleKit.CurrentLanguage.Register(ApplyFontForLanguage)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            ApplyFontForLanguage(LocaleKit.CurrentLanguage.Value);
        }

        protected override void SetText(string text)
        {
            if (!mTmpText)
            {
                mTmpText = GetComponent<TMP_Text>();
            }

            if (mTmpText)
            {
                mTmpText.text = text;
            }
        }

        private void ApplyFontForLanguage(Language language)
        {
            if (mTmpText == null || LanguageFonts == null || LanguageFonts.Count == 0)
            {
                return;
            }

            var entry = LanguageFonts.FirstOrDefault(f => f.Language == language && f.FontAsset);
            if (entry != null)
            {
                mTmpText.font = entry.FontAsset;
            }
        }
    }
}
