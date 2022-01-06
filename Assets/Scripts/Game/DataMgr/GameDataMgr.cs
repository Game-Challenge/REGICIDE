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
        if (m_action != null)
        {
            m_action();
            m_action = null;
        }
    }

    private Action m_action;
    public void RegisiterReq(string userId,string password,string name,Action action= null)
    {
        if (string.IsNullOrEmpty(name)|| string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            UISys.ShowTipMsg("不能有注册信息为空哦~");
            return;
        }

        m_action = action;
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
    public void LoginReq(string userId, string password, Action action = null)
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
        m_action = action;
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
        UISys.ShowTipMsg("登录成功，感谢游玩~");
        HadLogin = true;
        UISys.Mgr.CloseWindow<GameLoginUI>();
        CacheData(m_cacheUserId, mainPack.LoginPack.Password);
        if (m_action!= null)
        {
            m_action();
            m_action = null;
        }
    }
}