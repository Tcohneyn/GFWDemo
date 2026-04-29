using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:608c6621-fc94-41e9-b820-82747cabf1e8
	public partial class MainPanel
	{
		public const string Name = "MainPanel";
		
		[SerializeField]
		public RectTransform MainUI;
		[SerializeField]
		public UnityEngine.UI.Button btnfolder;
		
		private MainPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			MainUI = null;
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
