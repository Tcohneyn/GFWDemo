using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

// 1. 定义一个 Model 对象
public interface IGFWDemoModel : IModel
{
    //所有服务器数据
    BindableProperty<List<GFWServerInfo>> serverData { get; }
    // 读写：当前选中的服务器（需要订阅变化）
    BindableProperty<GFWServerInfo> CurrentSelectedServer { get; }
    
    // 读写：服务器列表是否展开（需要订阅变化）
    BindableProperty<bool> IsRegionExpanded { get; }
    //所选大区
    BindableProperty<ServerType> selectedServerType { get; }
    //推荐or所有？
    BindableProperty<LeftTabType> CurrentTab { get; }
    
    BindableProperty<bool> bShow { get; }
    
    BindableProperty<Language> mCurrentLanguage { get; }
}
public class GFWDemoModel : AbstractModel,IGFWDemoModel
{
    public BindableProperty<List<GFWServerInfo>> serverData { get; private set;}= new BindableProperty<List<GFWServerInfo>>();
    public BindableProperty<GFWServerInfo> CurrentSelectedServer { get; } = new BindableProperty<GFWServerInfo>(null);
    public BindableProperty<bool> IsRegionExpanded { get; } = new BindableProperty<bool>(false); // 初始未展开
    public BindableProperty<ServerType> selectedServerType { get; } = new BindableProperty<ServerType>(ServerType.Pioneer);
    
    public BindableProperty<LeftTabType> CurrentTab { get; } = new BindableProperty<LeftTabType>(LeftTabType.Recommend);
    public BindableProperty<bool> bShow { get; } = new BindableProperty<bool>(false);
    
    public BindableProperty<Language> mCurrentLanguage { get; }= new BindableProperty<Language>(Language.ChineseSimplified);

    protected override void OnInit()
    {
        var storage = this.GetUtility<IStorage>();
        List<GFWServerInfo> loadedData = storage.LoadServerInfo();
        // 设置初始值（不触发事件）
        serverData.SetValueWithoutEvent(loadedData);
        // 可选：默认选中第一个服务器
        if (serverData.Value != null && serverData.Value.Count > 0)
        {
            CurrentSelectedServer.Value = serverData.Value[0];
        }
        // 语言唯一基准：本 Model。LocaleKit 仅同步用于内置本地化组件（LocaleTMP 等）。
        mCurrentLanguage.RegisterWithInitValue(lang =>
        {
            LocaleKit.ChangeLanguage(lang);
            UILayoutRefresh.RefreshAll();
        });
    }
}
