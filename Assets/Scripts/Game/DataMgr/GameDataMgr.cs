using System;
using System.Collections;
using RegicideProtocol;
using UnityEngine;

public class GameDataMgr : DataCenterModule<GameDataMgr>
{
    public bool HadLogin = false;
    public override void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.Login, LoginRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Register, RegisiterRes);
    }

    public bool HadCacheLoginData()
    {
        var userId = PlayerPrefs.GetString("userId");

        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        return true;
    }

    private void CacheData(string userId,string password)
    {
        PlayerPrefs.SetString("userId",userId);
        PlayerPrefs.SetString("password",password);
    }

    private void RegisiterRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }
        GameOnlineMgr.Instance.InitMyData(mainPack);
        HadLogin = true;
        UISys.Mgr.CloseWindow<GameLoginUI>();

        CacheData(m_cacheUserId, mainPack.LoginPack.Password);
    }

    public void RegisiterReq(string userId,string password,string name)
    {
        if (string.IsNullOrEmpty(name)|| string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            UISys.ShowTipMsg("不能有注册信息为空哦~");
            return;
        }

        m_cacheUserId = userId;
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.User, ActionCode.Register);
        LoginPack loginPack = new LoginPack();
        loginPack.Username = userId;
        loginPack.Password = password;
        mainPack.Str = name;
        mainPack.LoginPack = loginPack;
        GameClient.Instance.SendCSMsg(mainPack);
    }

    private string m_cacheUserId;
    public void LoginReq(string userId, string password)
    {
        if (HadLogin)
        {
            UISys.ShowTipMsg("您已经登录了~");
            return;
        }
        if ( string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            UISys.ShowTipMsg("不能有登录信息为空哦~");
            return;
        }

        m_cacheUserId = userId;
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.User, ActionCode.Login);
        LoginPack loginPack = new LoginPack();
        loginPack.Username = userId;
        loginPack.Password = password;
        mainPack.LoginPack = loginPack;
        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void LoginRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }
        GameOnlineMgr.Instance.InitMyData(mainPack);

        HadLogin = true;
        UISys.Mgr.CloseWindow<GameLoginUI>();
        CacheData(m_cacheUserId, mainPack.LoginPack.Password);
    }
}