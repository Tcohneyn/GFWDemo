using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class GFWServerInfo
{
    //区号 ID
    public int id;
    //服务器名
    public string name;
    //服务器状态 0~4就是5种状态
    public ServerType Type;
    //是否是新服
    public bool isNew;
    //服务器延迟
    public int ms;
}

public enum ServerType
{
    [Description("先行")]
    Pioneer,
    
    [Description("东南亚")]
    SoutheastAsia,
    
    [Description("美洲")]
    America,
    
    [Description("亚洲")]
    Asia,
    
    [Description("欧洲")]
    Europe
}
public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        // 1. 获取类型信息
        Type type = value.GetType();
        // 2. 获取成员信息
        MemberInfo[] memInfo = type.GetMember(value.ToString());
        if (memInfo.Length > 0)
        {
            // 3. 查找 DescriptionAttribute
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        return value.ToString(); // 如果没写 Description，回退到原名
    }
}

public enum LeftTabType
{
    Recommend,     // 推荐服务器
    All            // 所有服务器（也就是那个可以折叠的）
}

