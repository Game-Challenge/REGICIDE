using RegicideProtocol;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

class StartUI : UIWindow
{
    #region 脚本工具生成的代码
    private Button m_btnConnect;
    private Text m_textStartGame;
    private Button m_btnStartGame;
    private Button m_btnMutiply;
    private Button m_btnRule;
    protected override void ScriptGenerator()
    {
        m_btnConnect = FindChildComponent<Button>("m_btnConnect");
        m_textStartGame = FindChildComponent<Text>("m_textStartGame");
        m_btnStartGame = FindChildComponent<Button>("m_textStartGame/m_btnStartGame");
        m_btnMutiply = FindChildComponent<Button>("m_btnMutiply");
        m_btnRule = FindChildComponent<Button>("m_btnRule");
        m_btnConnect.onClick.AddListener(OnClickConnectBtn);
        m_btnStartGame.onClick.AddListener(OnClickStartGameBtn);
        m_btnMutiply.onClick.AddListener(OnClickMutiplyBtn);
        m_btnRule.onClick.AddListener(OnClickRuleBtn);
    }
    #endregion
    #region 事件

    private void OnClickRuleBtn()
    {
        UISys.ShowTipMsg("敬请期待~");
    }
    private void OnClickStartGameBtn()
    {
        Close();
        if (GameMgr.Instance.IsLandScape)
        {
            UISys.Mgr.ShowWindow<GameUI>();
            GameMgr.Instance.RestartGame();
        }
        else
        {
            UISys.Mgr.ShowWindow<GameUILand>();
        }
        
    }
    private void OnClickConnectBtn()
    {
        if (GameClient.Instance.Status != GameClientStatus.StatusConnect)
        {
            WebSocketMgr.Instance.Init((() =>
            {
                UISys.Mgr.ShowWindow<GameLoginUI>();
            }));
        }

        if (GameDataMgr.Instance.HadLogin)
        {
            UISys.ShowTipMsg("您已经登录了");
            return;
        }

        if (GameClient.Instance.Status == GameClientStatus.StatusConnect)
        {
            UISys.ShowTipMsg("您已经连接到了服务器");
        }
        else
        {
            UISys.ShowTipMsg("亲，这个还没有做，快加群来催我(╯▔皿▔)╯！");
        }
    }

    private void OnClickMutiplyBtn()
    {
        if (GameDataMgr.Instance.HadLogin)
        {
            UISys.Mgr.ShowWindow<RoomUI>();
        }
        else
        {
            UISys.ShowTipMsg("多人模式请登录或注册哦~");
        }
    }
    #endregion
}
