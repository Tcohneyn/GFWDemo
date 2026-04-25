using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:d0cf831f-80dd-4ecc-8733-06f0f8e69c24
	public partial class ServerPanel
	{
		public const string Name = "ServerPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button ServerClose;
		[SerializeField]
		public UnityEngine.UI.ScrollRect ServerScrollLeft;
		[SerializeField]
		public UnityEngine.UI.Button ServerButton2;
		[SerializeField]
		public TMPro.TextMeshProUGUI SerbtnText;
		[SerializeField]
		public UnityEngine.RectTransform SubListContainer;
		[SerializeField]
		public UnityEngine.UI.Button RecommendBtn;
		[SerializeField]
		public UnityEngine.UI.Button AllServerBtn;
		[SerializeField]
		public UnityEngine.UI.ScrollRect ServerScrollRight;
		
		private ServerPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ServerClose = null;
			ServerScrollLeft = null;
			ServerButton2 = null;
			SerbtnText = null;
			SubListContainer = null;
			RecommendBtn = null;
			AllServerBtn = null;
			ServerScrollRight = null;
			
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
