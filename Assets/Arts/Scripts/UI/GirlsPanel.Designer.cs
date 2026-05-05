using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:266fbe35-618a-4620-a632-ab2807c5d832
	public partial class GirlsPanel
	{
		public const string Name = "GirlsPanel";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect svGirls;
		[SerializeField]
		public UnityEngine.UI.Button btnBack;
		[SerializeField]
		public TMPro.TextMeshProUGUI txtnumGirls;
		
		private GirlsPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			svGirls = null;
			btnBack = null;
			txtnumGirls = null;
			
			mData = null;
		}
		
		public GirlsPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		GirlsPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new GirlsPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
