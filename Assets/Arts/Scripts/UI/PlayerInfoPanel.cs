using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	public class PlayerInfoPanelData : UIPanelData
	{
	}
	public partial class PlayerInfoPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as PlayerInfoPanelData ?? new PlayerInfoPanelData();
			// please add init code here
			btnback.onClick.AddListener(() =>
			{
				this.Back();
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
