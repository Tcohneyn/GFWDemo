using UnityEngine;
using UnityEngine.UI;
using QFramework;

/// <summary>
/// 文案长度变化后强制刷新布局（Horizontal/Vertical Layout Group、Content Size Fitter 等）。
/// </summary>
public static class UILayoutRefresh
{
    /// <summary>
    /// 重建 UIRoot 下各层级的 RectTransform 布局，并刷新 Canvas。
    /// </summary>
    public static void RefreshAll()
    {
        var uiRoot = UIRoot.Instance;
        if (uiRoot == null || uiRoot.Canvas == null)
            return;

        RebuildImmediate(uiRoot.Canvas.transform as RectTransform);
        RebuildImmediate(uiRoot.Bg);
        RebuildImmediate(uiRoot.Common);
        RebuildImmediate(uiRoot.PopUI);
        RebuildImmediate(uiRoot.CanvasPanel);

        Canvas.ForceUpdateCanvases();
    }

    private static void RebuildImmediate(RectTransform rt)
    {
        if (rt != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
