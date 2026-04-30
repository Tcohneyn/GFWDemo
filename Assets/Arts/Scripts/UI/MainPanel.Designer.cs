using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:c87ead6f-02a6-4609-ba4b-bb581e968b76
	public partial class MainPanel
	{
		public const string Name = "MainPanel";
		
		[SerializeField]
		public RectTransform MainUI;
		[SerializeField]
		public UnityEngine.UI.Button playerinfo;
		[SerializeField]
		public UnityEngine.UI.Button btnfolder;
		
		private MainPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			MainUI = null;
			playerinfo = null;
			btnfolder = null;
			
			mData = null;
		}
		
		public MainPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MainPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MainPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
