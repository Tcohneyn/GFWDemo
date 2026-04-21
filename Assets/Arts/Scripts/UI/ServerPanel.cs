using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
    public class ServerPanelData : UIPanelData
    {
        // 状态变量
        public bool isSelected = false;
    }

    public partial class ServerPanel : UIPanel
    {
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as ServerPanelData ?? new ServerPanelData();
            // please add init code here
            ServerButton.onClick.AddListener(() =>
            {
                // 方法A：在当前X坐标基础上增加50（向右移动50像素）
                if (mData.isSelected = true)
                {
                    float targetX = SerbtnText.rectTransform.anchoredPosition.x + 80;
                    SerbtnText.rectTransform.DOAnchorPosX(targetX, 0.1f);
                    SerbtnText.color = Color.white;
                }
        });
    }

    protected override void OnOpen(IUIData uiData = null)
    {
    }

    protected override void OnShow()
    {
    }

    protected override void OnHide()
    {
    }

    protected override void OnClose()
    {
    }
}

}