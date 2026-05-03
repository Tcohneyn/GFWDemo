using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    [RequireComponent(typeof(Image))]
    public class LocaleImage : MonoBehaviour
    {
        public bool SetImageOnInit = true;
        public List<LanguageSprite> LanguageSprites = new List<LanguageSprite>();

        private Image mImage;

        private void Start()
        {
            mImage = GetComponent<Image>();

            LocaleKit.CurrentLanguage.Register(UpdateImage)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            if (SetImageOnInit)
            {
                UpdateImage(LocaleKit.CurrentLanguage.Value);
            }
        }

        public void UpdateImage(Language language)
        {
            if (!mImage || LanguageSprites == null || LanguageSprites.Count == 0)
            {
                return;
            }

            var languageSprite = LanguageSprites.FirstOrDefault(ls => ls.Language == language);

            if (languageSprite != null && languageSprite.Sprite)
            {
                mImage.sprite = languageSprite.Sprite;
            }
        }
    }
}
