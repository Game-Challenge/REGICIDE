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

    protected override void OnCreate()
    {
        base.OnCreate();
        if (GameDataMgr.Instance.HadLogin)
        {
            m_btnPushRank.gameObject.SetActive(false);
            var rankIndex = GameMgr.Instance.GameLevel;
            var userId = GameOnlineMgr.Instance.MyActorId;
            var completeType = -GameMgr.Instance.LeftJokerCount + 3;
            RankDataMgr.Instance.PushRankData(rankIndex, userId, completeType);
        }
    }

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
    private void OnClickPushRankBtn()
    {
        if (!GameDataMgr.Instance.HadLogin)
        {
            WebSocketMgr.Instance.Init((() =>
            {
                if (GameDataMgr.Instance.HadCacheLoginData())
                {
                    var userId = PlayerPrefs.GetString("userId");
                    var password = PlayerPrefs.GetString("password");
                    GameDataMgr.Instance.LoginReq(userId, password,(() => {Push();}));
                }
                else
                {
                    var ui = UISys.Mgr.ShowWindow<GameLoginUI>();
                    ui.CloseCallBack = () => { Push(); };
                }
            }));
        }
    }

    private void Push()
    {
        if (GameDataMgr.Instance.HadLogin)
        {
            m_btnPushRank.gameObject.SetActive(false);
            var rankIndex = GameMgr.Instance.GameLevel;
            var userId = GameOnlineMgr.Instance.MyActorId;
            var completeType = -GameMgr.Instance.LeftJokerCount + 3;
            RankDataMgr.Instance.PushRankData(rankIndex, userId, completeType);
        }
    }
    #endregion

}