using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
    public class MainPanelData : UIPanelData
    {
    }

    public partial class MainPanel : UIPanel
    {
        public Sprite LightButton;
        private ResLoader mResLoader = ResLoader.Allocate();
        private bool bfolder;
        private Sprite lastSprite;
        private IGFWDemoModel mModel;
        protected override void OnInit(IUIData uiData = null)
        {  
            mModel = GFWDemo.Interface.GetModel<IGFWDemoModel>();
            mData = uiData as MainPanelData ?? new MainPanelData();
            // please add init code here
            bfolder =false;
            playerinfo.onClick.AddListener(() =>
            {
                UIKit.Stack.Push(this);
                var playerInfoPanel = UIKit.OpenPanel<PlayerInfoPanel>();
            } );
            btnGirls.onClick.AddListener(() =>
            {
                UIKit.Stack.Push(this);
                UIKit.OpenPanel<GirlsPanel>();
            } );
            btnfolder.onClick.AddListener(() =>
            {
                Image imgfolder = btnfolder.transform.Find("imgfolder").GetComponent<Image>();
                if (!bfolder)
                {
                    lastSprite = imgfolder.sprite;
                    Sprite loadedSprite = mResLoader.LoadSync<Sprite>("main_icon_plus");
                    // 设置 Image 的 sprite 属性
                    if (imgfolder != null && loadedSprite != null)
                    {
                        imgfolder.sprite = loadedSprite;
                    }
                    imgfolder.SetNativeSize();
                    MainUI.Hide();
                    bfolder = true;
                }
                else
                {
                    imgfolder.sprite = lastSprite;
                    lastSprite = null;
                    imgfolder.SetNativeSize();
                    MainUI.Show();
                    bfolder = false;
                }
            });
            ChangePressedImage();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
            GFWDemo.Interface.SendCommand(new bShowCommand(true));
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            DOTween.KillAll();
            // 清理资源加载器
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }

        private void ChangePressedImage()
        {
            SpriteState state = btnGirls.spriteState;
            if (mModel.mCurrentLanguage.Value == Language.English)
            {
                state.pressedSprite = LightButton;
                btnGirls.spriteState = state;
            }
        }
    }
}