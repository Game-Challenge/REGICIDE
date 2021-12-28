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
    protected override void ScriptGenerator()
    {
        m_btnConnect = FindChildComponent<Button>("m_btnConnect");
        m_textStartGame = FindChildComponent<Text>("m_textStartGame");
        m_btnStartGame = FindChildComponent<Button>("m_textStartGame/m_btnStartGame");
        m_btnMutiply = FindChildComponent<Button>("m_btnMutiply");
        m_btnConnect.onClick.AddListener(OnClickConnectBtn);
        m_btnStartGame.onClick.AddListener(OnClickStartGameBtn);
        m_btnMutiply.onClick.AddListener(OnClickMutiplyBtn);
    }
    #endregion

    #region 事件
    private void OnClickStartGameBtn()
    {
        Close();
        UISys.Mgr.ShowWindow<GameUI>();
    }
    private void OnClickConnectBtn()
    {
        if (GameClient.Instance.Status == GameClientStatus.StatusConnect)
        {
            UISys.ShowTipMsg("您已经连接到了服务器");
        }
        else
        {
            GameClient.Instance.Connect();
        }
    }

    private void OnClickMutiplyBtn()
    {
        if (GameClient.Instance.Status == GameClientStatus.StatusConnect)
        {
            UISys.Mgr.ShowWindow<RoomUI>();
        }
        else
        {
            UISys.ShowTipMsg("多人模式请先连接服务器哦~");
        }
    }
    #endregion
}
