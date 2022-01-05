using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLoseUI : UIWindow
{

    #region 脚本工具生成的代码
    private Text m_textWinGame;
    private Button m_btnRestart;
    protected override void ScriptGenerator()
    {
        m_textWinGame = FindChildComponent<Text>("m_textWinGame");
        m_btnRestart = FindChildComponent<Button>("m_btnRestart");
        m_btnRestart.onClick.AddListener(OnClickRestartBtn);
    }
    #endregion

    public void InitUI(string msg)
    {
        m_textWinGame.text = msg;
        //MonoManager.Instance.GC();
    }

    #region 事件
    private void OnClickRestartBtn()
    {
        Close();
        GameMgr.Instance.RestartGame();
    }
    #endregion
}