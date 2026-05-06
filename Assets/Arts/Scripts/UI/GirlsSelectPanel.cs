using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
	public class GirlsSelectPanelData : UIPanelData
	{
        // 这个变量就是用来接住“点击的是哪个坑”的信息
        public int TargetIndex;
	}
	public partial class GirlsSelectPanel : UIPanel
	{
        private IGFWDemoModel mModel;
        private List<GameObject> itemList = new List<GameObject>();
        private ResLoader mResLoader = ResLoader.Allocate();
        private CanvasGroup mPanelCanvasGroup;
        private int mPendingPortraitLoads;
        private Coroutine mRevealCanvasRoutine;

        [Header("立绘全部加载完成后的 Canvas 显示")]
        [Tooltip("所有立绘异步都结束后再等待这么多秒，才把整块界面 CanvasGroup 显示出来（秒）。")]
        [SerializeField]
        [Min(0f)]
        private float canvasRevealDelayAfterLoads;
        /// <summary>供子物体 <see cref="GirlItem"/> 判断：根节点是否挂了 CanvasGroup（整块界面淡入）。</summary>
        public bool HasCanvasGroup => mPanelCanvasGroup != null;

        /// <summary>开始加载一张立绘时调用，与 <see cref="NotifyPortraitLoadDone"/> 成对。</summary>
        public void RegisterPortraitLoadPending()
        {
            if (mPanelCanvasGroup == null)
            {
                return;
            }

            mPendingPortraitLoads++;
            if (mPendingPortraitLoads == 1)
            {
                StopRevealRoutine();
                mPanelCanvasGroup.alpha = 0f;
                mPanelCanvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>单次立绘异步结束（含被刷新顶替的请求）时在回调里调用。</summary>
        public void NotifyPortraitLoadDone()
        {
            if (mPanelCanvasGroup == null)
            {
                return;
            }

            mPendingPortraitLoads = Mathf.Max(0, mPendingPortraitLoads - 1);
            if (mPendingPortraitLoads == 0)
            {
                StopRevealRoutine();
                if (canvasRevealDelayAfterLoads > 0f)
                {
                    mRevealCanvasRoutine = StartCoroutine(RevealCanvasAfterDelay());
                }
                else
                {
                    ShowPanelCanvasImmediate();
                }
            }
        }
        private void ShowPanelCanvasImmediate()
        {
            if (mPanelCanvasGroup == null)
            {
                return;
            }

            mPanelCanvasGroup.alpha = 1f;
            mPanelCanvasGroup.blocksRaycasts = true;
        }

        private void StopRevealRoutine()
        {
            if (mRevealCanvasRoutine == null)
            {
                return;
            }

            StopCoroutine(mRevealCanvasRoutine);
            mRevealCanvasRoutine = null;
        }

        private IEnumerator RevealCanvasAfterDelay()
        {
            try
            {
                yield return new WaitForSeconds(canvasRevealDelayAfterLoads);
                if (mPendingPortraitLoads != 0)
                {
                    yield break;
                }

                ShowPanelCanvasImmediate();
            }
            finally
            {
                mRevealCanvasRoutine = null;
            }
        }
        protected override void OnInit(IUIData uiData = null)
        {
            mPanelCanvasGroup = GetComponent<CanvasGroup>();
            mModel = GFWDemo.Interface.GetModel<IGFWDemoModel>();
            mData = uiData as GirlsSelectPanelData ?? new GirlsSelectPanelData();
            btnBack.onClick.AddListener(() =>
            {
                this.Back();
            });
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            UpdateList();
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            ShowPanelCanvasImmediate();
            // 清理资源加载器
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }

        protected void LoadGirlsItem(GirlsInfo info)
        {
            // 使用 ResKit 加载（假设你已经生成了资源代码或直接使用路径）
            var itemPrefab = mResLoader.LoadSync<GameObject>("GirlItem");

            GameObject item = Instantiate(itemPrefab, svGirls.content, false);


            var view = item.GetComponent<GirlItem>();
            if (view != null)
            {
                view.InitInfo(info,GirlsPanlType.GirlsSelectPanel, OnGirlSelected);
            }

            itemList.Add(item);
        }

        private void UpdateList()
        {
            foreach (var item in itemList)
            {
                Destroy(item);
            }

            itemList.Clear();
            var source = mModel.GirlsData.Value;
            if (source == null || source.Count == 0)
            {
                StopRevealRoutine();
                mPendingPortraitLoads = 0;
                ShowPanelCanvasImmediate();
                return;
            }
            var sorted = new List<GirlsInfo>(source);
            sorted.Sort(CompareGirlsForDisplayOrder);

            foreach (var info in sorted)
            {
                LoadGirlsItem(info);
            }
            
        }
        /// <summary>等级降序优先，同等级再按稀有度降序。</summary>
        private static int CompareGirlsForDisplayOrder(GirlsInfo a, GirlsInfo b)
        {
            var byLevel = b.level.CompareTo(a.level);
            if (byLevel != 0)
            {
                return byLevel;
            }

            return b.rarity.CompareTo(a.rarity);
        }
        // // 当玩家点击列表中的某个角色项时
        // private void OnGirlSelected(GirlsInfo info)
        // {
        //     // 1. 依然是发送全局命令更新数据
        //     // 因为主界面的 DisplaySlot 监听着 Model，所以数据一变，底层会自动更新
        //     GFWDemo.Interface.SendCommand(new SelectGirlCommand(mData.TargetIndex, info));
        //
        //     // 2. 返回上一层面板 (即回到 PlayInfoPanel)
        //     // QFramework 的 UIKit 支持 Back 逻辑
        //     this.Back(); 
        // }
        private void SaveGirlsIsSelected()
        {
            var storage = GFWDemo.Interface.GetUtility<IStorage>();
            storage.SaveGirlsInfo(mModel.GirlsData.Value);
        }

        private void OnGirlSelected(GirlsInfo info)
        {
            if (info.isSelected)
            {
                // 已经选中 → 取消选中，不退出面板
                info.isSelected = false;

                // 找到该角色占的 Slot 并清空
                for (int i = 0; i < mModel.SlotInfos.Length; i++)
                {
                    if (mModel.SlotInfos[i].Value == info)
                    {
                        mModel.SlotInfos[i].Value = null;
                        break;
                    }
                }

                SaveGirlsIsSelected();
                UpdateList();
            }
            else
            {
                // 未选中 → 选中并填入 Slot
                info.isSelected = true;

                GFWDemo.Interface.SendCommand(new SelectGirlCommand(mData.TargetIndex, info));

                SaveGirlsIsSelected();
                this.Back();
            }
        }
	}
}
