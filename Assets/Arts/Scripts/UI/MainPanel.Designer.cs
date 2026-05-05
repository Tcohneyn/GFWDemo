using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:c91a01f4-72ae-4eff-8dfe-3197c1bf2538
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
