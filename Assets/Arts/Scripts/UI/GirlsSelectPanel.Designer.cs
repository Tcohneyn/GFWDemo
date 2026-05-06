using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	// Generate Id:5fac7d37-b38b-4966-845d-658b7d82e945
	public partial class GirlsSelectPanel
	{
		public const string Name = "GirlsSelectPanel";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect svGirls;
		[SerializeField]
		public UnityEngine.UI.Button btnBack;
		
		private GirlsSelectPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			svGirls = null;
			btnBack = null;
			
			mData = null;
		}
		
		public GirlsSelectPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		GirlsSelectPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new GirlsSelectPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
