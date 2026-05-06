
using System.ComponentModel;
using UnityEngine;

public class GirlsInfo
{
    //女孩名
    public string name;
    //服装
    public string dress;
    //稀有度
    public int rarity;
    //等级
    public int level;
    //属性
    public AttributeType attribute;
    //武器
    public WeaponType weapon;
    //锁定
    public bool isLock = false;
    //图片
    public string girlImage;
    //展示是否选定
    public bool isSelected;
    /// <summary>
    /// 立绘在卡片内的矩形偏移（像素，相对预制体里 Image 的锚点位置）。
    /// 可在 StreamingAssets 的 GrilsInfo.json 里按角色单独配置。
    /// </summary>
    public float portraitOffsetX;
    public float portraitOffsetY;

    /// <summary>立绘整体缩放；JSON 未填或为 0 时按 1 处理。</summary>
    public float portraitScale;
}

public enum AttributeType
{
    [Description("防疫")]
    Immune,
    [Description("侵蚀")]
    Erosive,
    [Description("生物")]
    Biological,
    [Description("机械")]
    Mechanical,
    [Description("幽能")]
    Psionic
}

public enum WeaponType
{
    [Description("手枪")]
    HG,
    [Description("榴弹枪")]
    FT,
    [Description("狙击枪")]
    SR,
    [Description("霰弹枪")]
    SG,
    [Description("自动步枪")]
    AR
}

public enum GirlsPanlType
{
    [Description("面板")]
    GirlsPanel,
    [Description("选择面板")]
    GirlsSelectPanel,
    [Description("展示")]
    Display
}