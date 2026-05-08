using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:ad6253b4-1f60-4bca-aa8e-3b0ed60141f3
	public partial class MainPanel
	{
		public const string Name = "MainPanel";
		
		[SerializeField]
		public RectTransform MainUI;
		[SerializeField]
		public UnityEngine.UI.Button playerinfo;
		[SerializeField]
		public UnityEngine.UI.Button btnGirls;
		[SerializeField]
		public UnityEngine.UI.Button btnfolder;
		
		private MainPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			MainUI = null;
			playerinfo = null;
			btnGirls = null;
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
