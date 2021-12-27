using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

class StartUI : UIWindow
{
    #region 脚本工具生成的代码
    private Text m_textStartGame;
    private Button m_btnStartGame;
    protected override void ScriptGenerator()
    {
        m_textStartGame = FindChildComponent<Text>("m_textStartGame");
        m_btnStartGame = FindChildComponent<Button>("m_textStartGame/m_btnStartGame");
        m_btnStartGame.onClick.AddListener(OnClickStartGameBtn);
    }
    #endregion

    #region 事件
    private void OnClickStartGameBtn()
    {
        Close();
        UISys.Mgr.ShowWindow<GameUI>();
    }
    #endregion

}
