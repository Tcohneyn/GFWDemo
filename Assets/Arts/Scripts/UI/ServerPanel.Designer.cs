using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:20c2059d-dc5e-49ff-bf97-39b1fc26ee7c
	public partial class ServerPanel
	{
		public const string Name = "ServerPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button ServerButton;
		[SerializeField]
		public TMPro.TextMeshProUGUI SerbtnText;
		
		private ServerPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ServerButton = null;
			SerbtnText = null;
			
			mData = null;
		}
		
		public ServerPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		ServerPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new ServerPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
