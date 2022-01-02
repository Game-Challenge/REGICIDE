using UnityEngine;
using UnityEngine.UI;

class GameWinUI : UIWindow
{
    #region 脚本工具生成的代码
    private Text m_textWinGame;
    private Button m_btnRestart;
    private Button m_btnPushRank;
    protected override void ScriptGenerator()
    {
        m_textWinGame = FindChildComponent<Text>("m_textWinGame");
        m_btnRestart = FindChildComponent<Button>("m_btnRestart");
        m_btnPushRank = FindChildComponent<Button>("m_btnPushRank");
        m_btnRestart.onClick.AddListener(OnClickRestartBtn);
        m_btnPushRank.onClick.AddListener(OnClickPushRankBtn);
    }
    #endregion

    public void InitUI(string msg)
    {
        m_textWinGame.text = msg;
    }

    #region 事件
    private void OnClickRestartBtn()
    {
        Close();
        GameMgr.Instance.RestartGame();
    }
    private void OnClickPushRankBtn()
    {
        RankDataMgr.Instance.PushRankData();
    }
    #endregion

}