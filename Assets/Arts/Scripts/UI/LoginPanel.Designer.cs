using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:c509e065-451a-4494-8f10-3f5bf57f618a
	public partial class LoginPanel
	{
		public const string Name = "LoginPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button LoginButton;
		[SerializeField]
		public UnityEngine.UI.Image btnImage;
		[SerializeField]
		public RectTransform inside;
		[SerializeField]
		public TMPro.TextMeshProUGUI CurrentServerText;
		[SerializeField]
		public UnityEngine.UI.Button SelectButton;
		[SerializeField]
		public UnityEngine.UI.Button StartGame;
		
		private LoginPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			LoginButton = null;
			btnImage = null;
			inside = null;
			CurrentServerText = null;
			SelectButton = null;
			StartGame = null;
			
			mData = null;
		}
		
		public LoginPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LoginPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LoginPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
