using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening; 
using QFramework;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Events;

namespace QFramework.GFW
{
    public class BannerScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [Header("UI 引用")]
        public ScrollRect scrollRect;
        public RectTransform content;
        public RectTransform indicatorGrp;
        
        [Header("预制体")]
        public GameObject bannerItemPrefab;
        public GameObject dotPrefab;

        [Header("参数配置")]
        public float autoPlayInterval = 3f;
        public float scrollDuration = 0.5f;
        public string resourcePath = "Banner";

        private ResLoader mResLoader;
        private List<Image> mDots = new List<Image>();
        private int mTotalCount = 0;
        private int mCurrentIndex = 0;
        
        private float mTimer = 0f;
        private bool mIsDragging = false;

        private void Awake()
        {
            mResLoader = ResLoader.Allocate();
            if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
            
            // 关键：强制锁定轴向，防止“只能上下移动”
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
        }

        private void Start()
        {
            InitBanners();
        }

        private void InitBanners()
        {
            var assets = Resources.LoadAll<Texture2D>(resourcePath);
            mTotalCount = assets.Length;

            if (mTotalCount == 0) return;

            for (int i = 0; i < mTotalCount; i++)
            {
                // 生成 Banner
                var item = Instantiate(bannerItemPrefab, content);
                var rawImg = item.GetComponentInChildren<RawImage>();
                if (rawImg != null)
                {
                    rawImg.texture = mResLoader.LoadSync<Texture2D>($"resources://{resourcePath}/{assets[i].name}");
                }

                // 生成指示器
                var dot = Instantiate(dotPrefab, indicatorGrp);
                var dotImg = dot.GetComponentInChildren<Image>(); // 兼容子物体挂载 Image 的情况
                if (dotImg != null) mDots.Add(dotImg);
            }

            // 初始显示第一页
            RefreshUI(0);
        }

        private void Update()
        {
            if (mIsDragging || mTotalCount <= 1) return;

            mTimer += Time.deltaTime;
            if (mTimer >= autoPlayInterval)
            {
                mTimer = 0f;
                ScrollToNext();
            }
        }

        public void ScrollToNext()
        {
            int nextIndex = (mCurrentIndex + 1) % mTotalCount;
            MoveToPage(nextIndex);
        }

        public void MoveToPage(int index)
        {
            mCurrentIndex = index;
            // 参考博客思路：计算归一化位置
            float targetPos = mTotalCount > 1 ? (float)mCurrentIndex / (mTotalCount - 1) : 0;
            
            scrollRect.DOKill(); // 停止当前所有动画
            scrollRect.DOHorizontalNormalizedPos(targetPos, scrollDuration)
                .SetEase(Ease.OutCubic)
                .OnUpdate(() => SyncDotsByPos());
        }

        private void SyncDotsByPos()
        {
            // 根据实时位置同步指示器状态
            float pos = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition);
            int index = Mathf.RoundToInt(pos * (mTotalCount - 1));
            RefreshUI(index);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            mIsDragging = true;
            scrollRect.DOKill();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            mIsDragging = false;
            mTimer = 0f;

            // 吸附逻辑：松手时滑动到最近的一页
            float pos = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition);
            int nearestIndex = Mathf.RoundToInt(pos * (mTotalCount - 1));
            MoveToPage(nearestIndex);
        }

        private void RefreshUI(int index)
        {
            for (int i = 0; i < mDots.Count; i++)
            {
                mDots[i].color = (i == index) ? Color.white : new Color(0, 0, 0, 0.6f);
            }
        }

        private void OnDestroy()
        {
            mResLoader?.Recycle2Cache();
            mResLoader = null;
        }
    }
}