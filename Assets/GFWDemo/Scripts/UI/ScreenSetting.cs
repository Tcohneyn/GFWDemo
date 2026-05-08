using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSetting : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private TMP_Dropdown mDropdown;
    [SerializeField] private Toggle tgFullScreen;    // 全屏 (Exclusive)
    [SerializeField] private Toggle tgBorderless;    // 窗口化全屏 (Borderless)
    [SerializeField] private Toggle tgWindowed;
    private IGFWDemoModel mModel;
    // Start is called before the first frame update
    private void Awake()
    {
        mModel = GFWDemo.Interface.GetModel<IGFWDemoModel>();
    }

void Start()
    {
        // --- 1. 分辨率下拉框初始化 ---
        InitResolutionDropdown();

        // --- 2. 初始化 Toggle 状态 ---
        InitToggleState();

        // --- 3. 绑定 Toggle 事件 ---
        tgFullScreen.onValueChanged.AddListener(isOn => {
            if (isOn) ChangeMode(FullScreenMode.ExclusiveFullScreen);
        });

        tgBorderless.onValueChanged.AddListener(isOn => {
            if (isOn) ChangeMode(FullScreenMode.FullScreenWindow);
        });

        tgWindowed.onValueChanged.AddListener(isOn => {
            if (isOn) ChangeMode(FullScreenMode.Windowed);
        });

        // --- 4. 绑定分辨率下拉框事件 ---
        mDropdown.onValueChanged.AddListener(index => {
            var res = mModel.AvailableResolutions[index];
            GFWDemo.Interface.SendCommand(new ChangeSettingsCommand(res.width, res.height, Screen.fullScreenMode));
        });
    }

    private void InitResolutionDropdown()
    {
        mDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < mModel.AvailableResolutions.Count; i++)
        {
            var res = mModel.AvailableResolutions[i];
            options.Add($"{res.width} x {res.height}");
            
            // 匹配当前屏幕宽高
            if (res.width == Screen.width && res.height == Screen.height)
                currentResIndex = i;
        }

        mDropdown.AddOptions(options);
        mDropdown.value = currentResIndex;
        mDropdown.RefreshShownValue();
        
        // 初始化时也检查一次置灰逻辑
        UpdateDropdownInteractable(Screen.fullScreenMode);
    }

    private void InitToggleState()
    {
        var mode = Screen.fullScreenMode;
        tgFullScreen.SetIsOnWithoutNotify(mode == FullScreenMode.ExclusiveFullScreen);
        tgBorderless.SetIsOnWithoutNotify(mode == FullScreenMode.FullScreenWindow);
        tgWindowed.SetIsOnWithoutNotify(mode == FullScreenMode.Windowed);
    }

    private void ChangeMode(FullScreenMode mode)
    {
        // 更新下拉框是否可用
        UpdateDropdownInteractable(mode);

        var res = mModel.AvailableResolutions[mDropdown.value];
        GFWDemo.Interface.SendCommand(new ChangeSettingsCommand(res.width, res.height, mode));
    }

    // 核心交互优化：窗口化全屏下禁用分辨率选择
    private void UpdateDropdownInteractable(FullScreenMode mode)
    {
        // FullScreenWindow 模式分辨率由系统桌面强行决定，修改无效，故禁用
        mDropdown.interactable = (mode != FullScreenMode.FullScreenWindow);
    }
}
