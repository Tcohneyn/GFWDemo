using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.GFW
{
    public class ServerPanelData : UIPanelData
    {
        // 状态变量
        public bool isSelected = false;

        public Dictionary<string, bool> Serverselect = new Dictionary<string, bool>();

        // 存储三个按钮的状态
        public ButtonState[] buttonStates = new ButtonState[3];

        private int currentSelectedIndex = -1; // -1表示没有选中任何按钮

        // 其他配置数据
        public int defaultSelectedIndex = -1;
        public float moveDistance = 80f;
        public float animationDuration = 0.1f;
        public Color selectedColor = Color.white;

        public struct ButtonState
        {
            public Button button;
            public TMP_Text text;
            [HideInInspector] public Vector2 originalPosition; // 这个字段不会显示在Inspector中
            [HideInInspector] public Color originalColor; // 这个字段也不会显示
            [HideInInspector] public bool isSelected;
        }
    }

    public partial class ServerPanel : UIPanel
    {
        private IGFWDemoModel mModel;
        public Sprite normalImage;
        public Sprite selectedImage;
        private int currentSelectedIndex = -1;
        private List<ServerLeftItem> mItems = new List<ServerLeftItem>();
        private ResLoader mResLoader = ResLoader.Allocate();
        //用于存储右侧按钮们
        private List<GameObject> itemList = new List<GameObject>();

        protected override void OnInit(IUIData uiData = null)
        {
            // 缓存 Model 引用
            mData = uiData as ServerPanelData ?? new ServerPanelData();
            // please add init code here
            // 核心：获取 Model 引用
            mModel = GFWDemo.Interface.GetModel<IGFWDemoModel>();
            SubListContainer.Hide();
            //初始化存储按钮数据
            InitializeButtonStates();
            // 绑定点击事件
            BindButtonEvents();
            // 监听状态变化
            mModel.IsRegionExpanded.Register(isExpanded => { ExpandedLeftList(isExpanded); })
                .UnRegisterWhenGameObjectDestroyed(gameObject); // 别忘了自动注销

            mModel.selectedServerType.RegisterWithInitValue(type =>
                {
                    UpdateLeftList(type); 
                    UpdateRightList(type,mModel.CurrentTab.Value);
                })
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mModel.CurrentTab.RegisterWithInitValue(tab => 
            {
                // 1. 处理按钮的可点击性 (interactable)
                // 选中的按钮设为不可点击，没选中的设为可点击
                RecommendBtn.interactable = (tab != LeftTabType.Recommend);
                AllServerBtn.interactable = (tab != LeftTabType.All);
                SwitchCurrentTab(tab);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            ServerButton2.onClick.AddListener(() =>
            {
                //mModel.IsRegionExpanded.Value = !mModel.IsRegionExpanded.Value;
                GFWDemo.Interface.SendCommand(new RegionExpandedCommand());
            });

            ServerClose.onClick.AddListener(() => { UIKit.ClosePanel("ServerPanel"); });
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        /// <summary>
        /// 加载左侧按钮
        /// </summary>
        /// <param name="mResLoader"></param>
        public void LoadLeftItem(ResLoader mResLoader, ServerType type, bool isSelected, bool isLast)
        {
            // 使用 ResKit 加载（假设你已经生成了资源代码或直接使用路径）
            var itemPrefab = mResLoader.LoadSync<GameObject>("ServerLeftItem");
            GameObject item = Instantiate(itemPrefab, SubListContainer, false);
            // // 获取当前点击按钮的层级索引
            // int buttonIndex = ServerButton2.transform.GetSiblingIndex();
            //
            // // 将新生成的 Item 插在按钮的正下方
            // item.transform.SetSiblingIndex(buttonIndex + 1);

            var view = item.GetComponent<ServerLeftItem>();
            if (view != null)
            {
                view.InitInfo(type, isLast, mModel);
            }

            mItems.Add(view);
        }

        /// <summary>
        /// 加载右侧按钮
        /// </summary>
        /// <param name="mResLoader"></param>
        /// <param name="info"></param>
        public void LoadRightItem(ResLoader mResLoader, GFWServerInfo info,LeftTabType Tabtype)
        {
            // 使用 ResKit 加载（假设你已经生成了资源代码或直接使用路径）
            var itemPrefab = mResLoader.LoadSync<GameObject>("ServerRightItem");
            if (Tabtype == LeftTabType.Recommend)
            {
                if (info.ms <= 30)
                {
                    GameObject item = Instantiate(itemPrefab, ServerScrollRight.content, false);


                    var view = item.GetComponent<ServerRightItem>();
                    if (view != null)
                    {
                        view.InitInfo(info);
                    }

                    itemList.Add(item);
                }
            }
            else
            {
                GameObject item = Instantiate(itemPrefab, ServerScrollRight.content, false);


                var view = item.GetComponent<ServerRightItem>();
                if (view != null)
                {
                    view.InitInfo(info);
                }

                itemList.Add(item);
            }
        }

        /// <summary>
        /// 初始化所有按钮状态
        /// </summary>
        private void InitializeButtonStates()
        {
            // 初始化第一个按钮
            if (RecommendBtn != null)
            {
                TMP_Text text1 = RecommendBtn.transform.Find("SerbtnText")?.GetComponent<TMP_Text>();
                if (text1 != null)
                {
                    mData.buttonStates[0] = new ServerPanelData.ButtonState
                    {
                        button = RecommendBtn,
                        text = text1,
                        originalPosition = text1.rectTransform.anchoredPosition,
                        originalColor = text1.color,
                        isSelected = false
                    };
                }
            }

            // 初始化第二个按钮
            if (AllServerBtn != null)
            {
                TMP_Text text2 = AllServerBtn.transform.Find("SerbtnText")?.GetComponent<TMP_Text>();
                if (text2 != null)
                {
                    mData.buttonStates[1] = new ServerPanelData.ButtonState
                    {
                        button = AllServerBtn,
                        text = text2,
                        originalPosition = text2.rectTransform.anchoredPosition,
                        originalColor = text2.color,
                        isSelected = false
                    };
                }
            }

            // 初始化第三个按钮
            if (ServerButton2 != null)
            {
                TMP_Text text3 = ServerButton2.transform.Find("SerbtnText")?.GetComponent<TMP_Text>();
                if (text3 != null)
                {
                    mData.buttonStates[2] = new ServerPanelData.ButtonState
                    {
                        button = ServerButton2,
                        text = text3,
                        originalPosition = text3.rectTransform.anchoredPosition,
                        originalColor = text3.color,
                        isSelected = false
                    };
                }
            }
        }

        /// <summary>
        /// 绑定按钮点击事件
        /// </summary>
        private void BindButtonEvents()
        {
            for (int i = 0; i < mData.buttonStates.Length; i++)
            {
                int index = i; // 捕获索引
                if (mData.buttonStates[i].button != null)
                {
                    if (mData.buttonStates[i].button == RecommendBtn)
                    {
                        mData.buttonStates[i].button.onClick.AddListener(() =>
                        {
                            // 如果当前已经是推荐，就不重复执行逻辑
                            if (mModel.CurrentTab.Value == LeftTabType.Recommend) return;
                            mModel.CurrentTab.Value = LeftTabType.Recommend;
                            OnServerButtonClicked(index);
                        });
                    }
                    else if(mData.buttonStates[i].button == AllServerBtn)
                    {
                        mData.buttonStates[i].button.onClick.AddListener(() =>
                        {
                            if (mModel.CurrentTab.Value == LeftTabType.All) return;
                            mModel.CurrentTab.Value = LeftTabType.All;
                            OnServerButtonClicked(index);
                        });
                    }
                    else
                    {
                        mData.buttonStates[i].button.onClick.AddListener(() => OnServerButtonClicked(index));
                    }
                }
            }
        }

        /// <summary>
        /// 服务器按钮点击事件处理
        /// </summary>
        private void OnServerButtonClicked(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= mData.buttonStates.Length) return;

            // 如果点击的是已选中的按钮，取消选中
            if (currentSelectedIndex == buttonIndex)
            {
                DeselectButton(buttonIndex);
                currentSelectedIndex = -1;
                return;
            }

            // 取消之前选中的按钮
            if (currentSelectedIndex >= 0)
            {
                DeselectButton(currentSelectedIndex);
            }

            // 选中新按钮
            SelectButton(buttonIndex);
            currentSelectedIndex = buttonIndex;
        }

        /// <summary>
        /// 选中按钮
        /// </summary>
        private void SelectButton(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= mData.buttonStates.Length) return;

            var state = mData.buttonStates[buttonIndex];
            if (state.text == null) return;

            // 使用 mData 中的配置
            float targetX = state.originalPosition.x + mData.moveDistance;
            state.text.rectTransform.DOAnchorPosX(targetX, mData.animationDuration);
            state.text.DOColor(mData.selectedColor, mData.animationDuration);
            state.button.image.sprite = selectedImage;
            // 更新状态
            mData.buttonStates[buttonIndex].isSelected = true;
        }

        /// <summary>
        /// 取消选中按钮
        /// </summary>
        private void DeselectButton(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= mData.buttonStates.Length) return;

            var state = mData.buttonStates[buttonIndex];
            if (state.text == null) return;

            state.text.rectTransform.DOAnchorPos(state.originalPosition, mData.animationDuration);
            state.text.DOColor(state.originalColor, mData.animationDuration);
            state.button.image.sprite = normalImage;
            // 更新状态
            mData.buttonStates[buttonIndex].isSelected = false;
        }

        private void ExpandedLeftList(bool isExpanded)
        {
            if (isExpanded)
            {
                SubListContainer.Show();
                // 1. 获取枚举的所有值并转为数组，方便拿到长度
                var serverTypes = Enum.GetValues(typeof(ServerType));
                int totalCount = serverTypes.Length;
                int currentIndex = 0;
                // 状态变为展开：生成所有大区条目
                foreach (ServerType type in serverTypes)
                {
                    currentIndex++;
                    // 2. 判断是否是最后一个：当前的索引等于总长度
                    bool isLast = (currentIndex == totalCount);
                    // 调用你之前的加载方法
                    LoadLeftItem(mResLoader, type, false, isLast);
                }
            }
            else
            {
                // 状态变为收起：清理掉所有生成的条目
                // 建议给生成的 Item 加上 Tag 或存到 List 里方便删除
                ClearGeneratedItems();
            }

            // 3. 核心：强制立即重新计算布局（双重保险）
            // 第一步：强制让系统意识到层级变化
            Canvas.ForceUpdateCanvases();

            // 第二步：强制刷新子容器（如果有）
            LayoutRebuilder.ForceRebuildLayoutImmediate(SubListContainer.GetComponent<RectTransform>());

            // 第三步：强制刷新父级 Content
            LayoutRebuilder.ForceRebuildLayoutImmediate(ServerScrollLeft.content as RectTransform);
        }

        private void ClearGeneratedItems()
        {
            //假设 SubListContainer 是你存放那些二级菜单 Item 的父物体
            SubListContainer.transform.DestroyChildren();
            mItems.Clear();
            SubListContainer.Hide();
        }

        private void UpdateLeftList(ServerType type)
        {
            // 假设这些 View 已经在你生成时存进了 mViews
            foreach (var Item in mItems)
            {
                // 这里的逻辑就是：如果是匹配的类型，nowselectText 就会 SetActive(true)
                // 否则会自动 SetActive(false)
                // 注意：你需要给 view 增加一个存 type 的变量，或者通过名字判断
                bool isSelected = (Item.MyType == type);
                Item.SetSelectState(isSelected);
            }
        }

        private void UpdateRightList(ServerType type,LeftTabType Tabtype)
        {
            foreach (var item in itemList)
            {
                Destroy(item);
            }
            itemList.Clear();
            foreach (var Info in mModel.serverData.Value)
            {
                if (Info.Type == type)
                {
                    LoadRightItem(mResLoader, Info,Tabtype);
                }
                else
                {
                    continue;
                }
            }
        }

        private void SwitchCurrentTab(LeftTabType type)
        {
            ServerType info = mModel.selectedServerType.Value;
            // 2. 处理折叠逻辑
            if (type == LeftTabType.All)
            {
                UpdateRightList(info,type);
            }
            else
            {
                UpdateRightList(info,type);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 1. 彻底杀掉所有动画，防止跳转场景报错
            transform.DOKill();

            // 2. 回收资源
            if (mResLoader != null)
            {
                mResLoader.Recycle2Cache();
                mResLoader = null;
            }
        }
    }
}