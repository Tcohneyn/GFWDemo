using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class GFWDemo : Architecture<GFWDemo>
{
    protected override void Init()
    {
        this.RegisterModel<IGFWDemoModel>(new GFWDemoModel());

        // 注册存储工具对象
        this.RegisterUtility<IStorage>(new Storage());
        
    }
}
