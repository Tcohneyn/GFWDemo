using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

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
