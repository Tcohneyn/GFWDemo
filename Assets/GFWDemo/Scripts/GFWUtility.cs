using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public interface IStorage : IUtility
{
    List<GFWServerInfo> LoadServerInfo();
    List<GirlsInfo> LoadGirlsInfo();
}

public class Storage : IStorage
{
    public List<GFWServerInfo> LoadServerInfo()
    {
        return JsonMgr.Instance.LoadData<List<GFWServerInfo>>("ServerInfo");
    }

    public List<GirlsInfo> LoadGirlsInfo()
    {
        return JsonMgr.Instance.LoadData<List<GirlsInfo>>("GrilsInfo");
    }
}
