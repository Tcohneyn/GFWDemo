using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using QFramework.GFW;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 服务器列表是否展开
/// </summary>
public class RegionExpandedCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<IGFWDemoModel>();
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
        var model = this.GetModel<IGFWDemoModel>();
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

public class bShowCommand : AbstractCommand
{
    private bool isShow;

    public bShowCommand(bool show)
    {
        isShow = show;
    }

    protected override void OnExecute()
    {
        var model = this.GetModel<IGFWDemoModel>();
        model.bShow.Value = isShow;
    }
}

public class ChangeLanguageCommand : AbstractCommand
{
    private Language currentLanguage;

    public ChangeLanguageCommand(Language language)
    {
        currentLanguage = language;
    }
    protected override void OnExecute()
    {
        var model = this.GetModel<IGFWDemoModel>();
        model.mCurrentLanguage.Value = currentLanguage;
    }
}
public class LoadSceneCommand : AbstractCommand
{
    private readonly string mSceneName;
    public LoadSceneCommand(string sceneName) => mSceneName = sceneName;

    private float mDisplayProgress = 0f;
    private IUnRegister mUpdateHandle;

    protected override void OnExecute()
    {
        var loadingPanel = UIKit.OpenPanel<LoadingPanel>();
        var canvasGroup = loadingPanel.GetOrAddComponent<CanvasGroup>();
        var mModel = this.GetModel<IGFWDemoModel>();
        
        mModel.bShow.Value = false;
        var resLoader = ResLoader.Allocate();
        
        resLoader.LoadSceneAsync(mSceneName, onStartLoading: (op) =>
        {
            // 关键 1：绝对不自动跳转
            op.allowSceneActivation = false;

            mUpdateHandle = ActionKit.OnUpdate.Register(() =>
            {
                if (op == null || loadingPanel == null) { CleanUp(resLoader); return; }

                float realProgress = op.progress / 0.9f; 

                // 进度逻辑：在 0-0.9 之间有节奏地爬行
                if (mDisplayProgress < 0.9f)
                {
                    // 即使 realProgress 很快，我们也让 display 慢慢追，制造“分段感”
                    float speed = realProgress > mDisplayProgress ? 0.8f : 0.3f;
                    mDisplayProgress = Mathf.MoveTowards(mDisplayProgress, realProgress * 0.9f, Time.deltaTime * speed);
                }
                
                loadingPanel.SetProgress(mDisplayProgress);

                // 关键 2：后台加载好了 (0.9)，且进度条也爬到了 0.9 附近
                if (op.progress >= 0.9f && mDisplayProgress >= 0.89f)
                {
                    // 此时不销毁 Update，而是进入“等待激活”状态
                    mUpdateHandle?.UnRegister();
                    
                    // 开启最后的冲刺逻辑
                    StartSprint(op, loadingPanel, canvasGroup, resLoader);
                }
            });
        });
    }

    private void StartSprint(AsyncOperation op, LoadingPanel panel, CanvasGroup group, ResLoader loader)
    {
        // 关键 3：先让进度条冲刺到 100%
        DOTween.To(() => mDisplayProgress, x => 
        {
            mDisplayProgress = x;
            panel.SetProgress(mDisplayProgress);
        }, 1f, 0.3f).SetEase(Ease.InCubic).OnComplete(() => 
        {
            // 进度条冲完后，才允许 Unity 切换场景
            op.allowSceneActivation = true;
            
            // 此时场景开始切换，我们需要等待新场景就绪信号
            WaitForNewScene(panel, group, loader);
        });
    }

    private void WaitForNewScene(LoadingPanel panel, CanvasGroup group, ResLoader loader)
    {
        var mModel = this.GetModel<IGFWDemoModel>();
        
        // 关键 4：监听新场景信号。此时 LoadingPanel 依然是 alpha=1，挡住可能的空场景
        mModel.bShow.Register(show =>
        {
            if (show && group != null)
            {
                group.DOKill();
                group.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    UIKit.ClosePanel<LoadingPanel>();
                    CleanUp(loader);
                    // 消耗掉信号，防止下次干扰
                    mModel.bShow.Value = false; 
                });
            }
        }).UnRegisterWhenGameObjectDestroyed(panel.gameObject);
    }

    private void CleanUp(ResLoader loader)
    {
        mUpdateHandle?.UnRegister();
        loader?.Recycle2Cache();
    }
}