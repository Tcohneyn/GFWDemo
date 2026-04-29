using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.GFW
{
    public class LoadingPanelData : UIPanelData
    {
    }

    public partial class LoadingPanel : UIPanel
    {
        private ResLoader mResLoader;
        private const string BG_TAG = "loading"; // 对应你给资源打的 AssetBundle Name

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as LoadingPanelData ?? new LoadingPanelData();
            mResLoader = ResLoader.Allocate();
            ShowRandomBackground();
        }

        private void ShowRandomBackground()
        {
            if (bgloading == null) return;

            // --- 第一步：获取文件夹内的所有资源信息 ---
            // 注意：Resources.LoadAll 会扫描路径，我们指定加载 Texture2D 类型
            // 这一步是为了拿到文件夹下所有图片的名字
            var allInfos = Resources.LoadAll<Texture2D>("Loading");

            if (allInfos != null && allInfos.Length > 0)
            {
                // --- 第二步：从这“一堆”资源里随机选一个名字 ---
                int randomIndex = Random.Range(0, allInfos.Length);
                string selectedAssetName = allInfos[randomIndex].name;

                // --- 第三步：使用你提供的 LoadSync<T> 模板进行加载 ---
                // 这里的 T 是 Texture2D，符合 "where T : Object" 的约束
                // 路径格式必须是 "resources://路径/文件名"
                string resPath = $"resources://Loading/{selectedAssetName}";
                
                Texture2D tex = mResLoader.LoadSync<Texture2D>(resPath);

                if (tex != null)
                {
                    // 赋值并展示
                    bgloading.texture = tex;
                    ApplyVisualEffects();
                }
            }
            else
            {
                Debug.LogError("Resources/Loading 文件夹下没有找到任何图片！");
            }
        }

        private void ApplyVisualEffects()
        {
            // 简单的淡入效果
            bgloading.color = new Color(1, 1, 1, 0);
            bgloading.DOKill();
            bgloading.DOFade(1f, 0.2f).SetUpdate(true);

            // 自动适配比例防止拉伸
            var fitter = bgloading.GetOrAddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            if (bgloading.texture != null)
            {
                fitter.aspectRatio = (float)bgloading.texture.width / bgloading.texture.height;
            }
        }

        protected override void OnClose()
        {
            mResLoader?.Recycle2Cache();
            mResLoader = null;
        }

        public void SetProgress(float progress)
        {
            // 增加这一行防御代码：如果进度条对象已经销毁，直接返回
            if (LoadingBar == null) return;

            LoadingBar.value = progress;
        }
    }
}