using System;
using System.Collections;
using RegicideProtocol;
using UnityEngine;

public class GameDataMgr : DataCenterModule<GameDataMgr>
{
    public override void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.Login, LoginRes);

    }

    private void LoginRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }
        GameOnlineMgr.Instance.InitMyData(mainPack);
    }
}