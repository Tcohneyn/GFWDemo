using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:e14e5543-31b2-47d0-b62d-d7093a20f163
	public partial class PlayerInfoPanel
	{
		public const string Name = "PlayerInfoPanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI textTopleft;
		[SerializeField]
		public TMPro.TextMeshProUGUI textBottomleft;
		[SerializeField]
		public RectTransform playInfoContainer;
		[SerializeField]
		public UnityEngine.UI.Button btnback;
		
		private PlayerInfoPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			textTopleft = null;
			textBottomleft = null;
			playInfoContainer = null;
			btnback = null;
			
			mData = null;
		}
		
		public PlayerInfoPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		PlayerInfoPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new PlayerInfoPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
