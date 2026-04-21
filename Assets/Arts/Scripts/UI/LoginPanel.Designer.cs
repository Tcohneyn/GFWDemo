using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:6f7574ab-8f68-4dd3-adfe-899ba1395d34
	public partial class LoginPanel
	{
		public const string Name = "LoginPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button LoginButton;
		[SerializeField]
		public UnityEngine.UI.Image btnImage;
		
		private LoginPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			LoginButton = null;
			btnImage = null;
			
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
