﻿using System.Collections.Generic;

class DataCenterSys : BaseLogicSys<DataCenterSys>
{
    private List<IDataCenterModule> m_listModule = new List<IDataCenterModule>();

    public override bool OnInit()
    {
        RegCmdHandle();
        InitModule();
        return true;
    }

    private void RegCmdHandle()
    {
        var client = GameClient.Instance;
    }

    void InitModule()
    {
        InitModule(GameOnlineMgr.Instance);
        InitModule(RoomDataMgr.Instance);
        InitModule(GameDataMgr.Instance);
        //InitModule(ChatDataMgr.Instance);
        //InitModule(ActorDataMgr.Instance);
    }

    public void InitModule(IDataCenterModule module)
    {
        if (!m_listModule.Contains(module))
        {
            module.Init();
            m_listModule.Add(module);
        }
    }
}
