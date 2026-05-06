using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

// 1. 定义一个 Model 对象
public interface IGFWDemoModel : IModel
{
    //所有服务器数据
    BindableProperty<List<GFWServerInfo>> ServerData { get; }
    //女孩们信息数据
    BindableProperty<List<GirlsInfo>> GirlsData { get; }
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
    
    // 长度为 3 的数组，存储选中的角色信息
    BindableProperty<GirlsInfo>[] SlotInfos { get; }
    
}
public class GFWDemoModel : AbstractModel,IGFWDemoModel
{
    public BindableProperty<List<GFWServerInfo>> ServerData { get; private set;}= new BindableProperty<List<GFWServerInfo>>();

    public BindableProperty<List<GirlsInfo>> GirlsData { get; } = new BindableProperty<List<GirlsInfo>>();
    public BindableProperty<GFWServerInfo> CurrentSelectedServer { get; } = new BindableProperty<GFWServerInfo>(null);
    public BindableProperty<bool> IsRegionExpanded { get; } = new BindableProperty<bool>(false); // 初始未展开
    public BindableProperty<ServerType> selectedServerType { get; } = new BindableProperty<ServerType>(ServerType.Pioneer);
    
    public BindableProperty<LeftTabType> CurrentTab { get; } = new BindableProperty<LeftTabType>(LeftTabType.Recommend);
    public BindableProperty<bool> bShow { get; } = new BindableProperty<bool>(false);
    
    public BindableProperty<Language> mCurrentLanguage { get; }= new BindableProperty<Language>(Language.ChineseSimplified);
    public BindableProperty<GirlsInfo>[] SlotInfos { get; } = new[]
    {
        new BindableProperty<GirlsInfo>(),
        new BindableProperty<GirlsInfo>(),
        new BindableProperty<GirlsInfo>()
    };
    

    protected override void OnInit()
    {
        var storage = this.GetUtility<IStorage>();
        List<GFWServerInfo> loadedData = storage.LoadServerInfo();
        List<GirlsInfo> GirlsloadedData = storage.LoadGirlsInfo();
        // 设置初始值（不触发事件）
        ServerData.SetValueWithoutEvent(loadedData);
        GirlsData.SetValueWithoutEvent(GirlsloadedData);
        
        // 从持久化数据中恢复已选中的角色到对应 Slot
        if (GirlsloadedData != null)
        {
            int slotIdx = 0;
            foreach (var girl in GirlsloadedData)
            {
                if (girl.isSelected && slotIdx < SlotInfos.Length)
                {
                    SlotInfos[slotIdx].SetValueWithoutEvent(girl);
                    slotIdx++;
                }
            }
        }
        
        // 可选：默认选中第一个服务器
        if (ServerData.Value != null && ServerData.Value.Count > 0)
        {
            CurrentSelectedServer.Value = ServerData.Value[0];
        }
        // 语言唯一基准：本 Model。LocaleKit 仅同步用于内置本地化组件（LocaleTMP 等）。
        mCurrentLanguage.RegisterWithInitValue(lang =>
        {
            LocaleKit.ChangeLanguage(lang);
            UILayoutRefresh.RefreshAll();
        });
    }
}
