using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.GFW;
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
}
public class GFWDemoModel : AbstractModel,IGFWDemoModel
{
    public BindableProperty<List<GFWServerInfo>> serverData { get; private set;}= new BindableProperty<List<GFWServerInfo>>();
    public BindableProperty<GFWServerInfo> CurrentSelectedServer { get; } = new BindableProperty<GFWServerInfo>(null);
    public BindableProperty<bool> IsRegionExpanded { get; } = new BindableProperty<bool>(false); // 初始未展开
    public BindableProperty<ServerType> selectedServerType { get; } = new BindableProperty<ServerType>(ServerType.Pioneer);
    
    public BindableProperty<LeftTabType> CurrentTab { get; } = new BindableProperty<LeftTabType>(LeftTabType.Recommend);
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
    }
}
public class GFWDemo : Architecture<GFWDemo>
{
    protected override void Init()
    {
        this.RegisterModel<IGFWDemoModel>(new GFWDemoModel());

        // 注册存储工具对象
        this.RegisterUtility<IStorage>(new Storage());
        
    }
}
public interface IStorage : IUtility
{
    List<GFWServerInfo> LoadServerInfo();
}

public class Storage : IStorage
{
    public List<GFWServerInfo> LoadServerInfo()
    {
        return JsonMgr.Instance.LoadData<List<GFWServerInfo>>("ServerInfo");
    }
}
// 引入 Command
public class RegionExpandedCommand : AbstractCommand 
{
    protected override void OnExecute()
    {
        var model =this.GetModel<IGFWDemoModel>();
        model.IsRegionExpanded.Value = !model.IsRegionExpanded.Value;
    }
}

public class selectedServerCommand : AbstractCommand
{
    private GFWServerInfo selectedInfo;

    public selectedServerCommand(GFWServerInfo info)
    {
        selectedInfo = info;
    }
    protected override void OnExecute()
    {
        var model =this.GetModel<IGFWDemoModel>();
        model.CurrentSelectedServer.Value = selectedInfo;
    }
}

public class selectedServerTypeCommand : AbstractCommand 
{
    private ServerType selectedType;

    public selectedServerTypeCommand(ServerType Type)
    {
        selectedType = Type;
    }

    protected override void OnExecute()
    {
        var model = this.GetModel<IGFWDemoModel>();
        model.selectedServerType.Value = selectedType;
    }
}

public class LoginManager : MonoBehaviour, IController
{
    private IGFWDemoModel mModel;
    
    /// <summary>
    /// 页面的名字
    /// </summary>
    public string PanelName;

    /// <summary>
    /// 层级名字
    /// </summary>
    public UILevel Level;

    [SerializeField] public List<UIPanelTesterInfo> mOtherPanels;
    private void Awake()
    {
        ResKit.Init();
    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        LoginPanel loginPanel= UIKit.OpenPanel(PanelName, Level) as LoginPanel;
        //mOtherPanels.ForEach(panelTesterInfo => { UIKit.OpenPanel(panelTesterInfo.PanelName, panelTesterInfo.Level); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IArchitecture GetArchitecture()
    {
        return GFWDemo.Interface;
    }
    private void OnDestroy()
    {
        // 8. 将 Model 设置为空
        mModel = null;
    }
}
