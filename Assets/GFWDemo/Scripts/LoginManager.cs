using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.GFW;
using UnityEngine;


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
