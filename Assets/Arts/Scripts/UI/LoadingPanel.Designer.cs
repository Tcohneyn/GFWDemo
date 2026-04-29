using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:d5eae3cf-6258-4814-a41b-534e6b72153c
	public partial class LoadingPanel
	{
		public const string Name = "LoadingPanel";
		
		[SerializeField]
		public UnityEngine.UI.RawImage bgloading;
		[SerializeField]
		public UnityEngine.UI.Slider LoadingBar;
		
		private LoadingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			bgloading = null;
			LoadingBar = null;
			
			mData = null;
		}
		
		public LoadingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LoadingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LoadingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
