using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:149bc342-221f-4adb-9432-b418ccd0c8ac
	public partial class PlayerInfoPanel
	{
		public const string Name = "PlayerInfoPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button btnback;
		
		private PlayerInfoPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			btnback = null;
			
			mData = null;
		}
		
		public PlayerInfoPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		PlayerInfoPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new PlayerInfoPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
