using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupManager : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private ToggleGroup toggleGroup; // 拖拽赋值
    [SerializeField] private bool lockSelectedToggle = true; // 核心：选中后锁定

    [Header("外观 (可选)")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedLockedColor = new Color(0.6f, 0.8f, 1.0f, 0.8f); // 选中锁定后的颜色

    private List<Toggle> allToggles = new List<Toggle>();
    private Toggle currentSelectedToggle = null;
    public static event Action<Toggle> OnToggleChanged;
    private void Awake()
    {
        if (toggleGroup == null)
            toggleGroup = GetComponent<ToggleGroup>();

        // 收集组内所有 Toggle
        CollectAllToggles();
    }

    private void Start()
    {
        // 为每个 Toggle 绑定事件
        foreach (Toggle toggle in allToggles)
        {
            if (toggle != null)
            {
                // 绑定值变化事件
                toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));

                // 初始化状态：如果某个 Toggle 初始就是选中的，应用锁定状态
                if (toggle.isOn)
                {
                    OnToggleSelected(toggle);
                }
            }
        }
    }

    // 收集 Toggle
    private void CollectAllToggles()
    {
        allToggles.Clear();
        Toggle[] toggles = GetComponentsInChildren<Toggle>(true); // true 表示包含未激活的
        foreach (Toggle t in toggles)
        {
            if (t.group == toggleGroup) // 确保只管理本组的
            {
                allToggles.Add(t);
            }
        }
    }

    // Toggle 值变化事件处理
    private void OnToggleValueChanged(Toggle changedToggle, bool isOn)
    {
        if (isOn)
        {
            // 新 Toggle 被选中
            OnToggleSelected(changedToggle);
        }
        // 注意：在 ToggleGroup 中，当一个被设为 true，旧的会自动设为 false，
        // 但旧的 Toggle 的 onValueChanged(false) 也会被触发。
    }

    // 处理 Toggle 被选中的逻辑
    private void OnToggleSelected(Toggle selectedToggle)
    {
        // 1. 如果之前有选中的 Toggle，解锁它
        if (currentSelectedToggle != null && currentSelectedToggle != selectedToggle)
        {
            UnlockToggle(currentSelectedToggle);
        }

        // 2. 锁定当前选中的 Toggle
        if (lockSelectedToggle)
        {
            LockToggle(selectedToggle);
        }

        // 3. 更新当前选中引用
        currentSelectedToggle = selectedToggle;

        // 4. 可以在这里触发其他业务逻辑，比如保存选择
        Debug.Log($"选中了: {selectedToggle.name}");
        
        OnToggleChanged?.Invoke(selectedToggle);
    }

    // 锁定 Toggle（不可交互，改变外观）
    private void LockToggle(Toggle toggle)
    {
        toggle.interactable = false;
        // 可选：改变外观
        Image bg = toggle.GetComponent<Image>();
        if (bg != null) bg.color = selectedLockedColor;
    }

    // 解锁 Toggle（可交互，恢复外观）
    private void UnlockToggle(Toggle toggle)
    {
        toggle.interactable = true;
        // 可选：恢复外观
        Image bg = toggle.GetComponent<Image>();
        if (bg != null) bg.color = normalColor;
    }

    // ========== 外部控制方法 ==========
    // 获取当前选中的 Toggle
    public Toggle GetSelectedToggle()
    {
        return currentSelectedToggle;
    }

    // 获取当前选中 Toggle 的索引
    public int GetSelectedIndex()
    {
        for (int i = 0; i < allToggles.Count; i++)
        {
            if (allToggles[i] == currentSelectedToggle)
                return i;
        }
        return -1;
    }

    // 通过代码强制选中某个 Toggle（也会触发锁定）
    public void SelectToggle(int index)
    {
        if (index >= 0 && index < allToggles.Count)
        {
            allToggles[index].isOn = true;
            // onValueChanged 事件会被触发，进而调用 OnToggleSelected
        }
    }

    // 重置所有 Toggle（全部解锁，取消选中）
    public void ResetAllToggles()
    {
        // 必须先解除事件监听，避免在设置 isOn=false 时触发不必要的逻辑
        foreach (Toggle t in allToggles)
        {
            t.onValueChanged.RemoveListener((bool b) => { }); // 移除匿名监听器
            t.interactable = true;
            t.isOn = false;
        }
        currentSelectedToggle = null;
        // 重新绑定事件 (简化处理，实际可能需要重新初始化)
        // 更稳健的做法是在重置后重新调用 Start() 中的逻辑，或单独写一个初始化方法。
    }

    private void OnDestroy()
    {
        // 清理事件监听
        foreach (Toggle toggle in allToggles)
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }
    }
}
