using UnityEngine;
using UnityEngine.UI;

class GameLoginUI : UIWindow
{
    #region 脚本工具生成的代码
    private Button m_btnClose;
    private GameObject m_goLogin;
    private InputField m_inputUserID;
    private InputField m_inputUserPassword;
    private Button m_btnRegister;
    private Button m_btnLogin;
    private InputField m_inputUserName;
    protected override void ScriptGenerator()
    {
        m_btnClose = FindChildComponent<Button>("m_btnClose");
        m_goLogin = FindChild("m_goLogin").gameObject;
        m_inputUserID = FindChildComponent<InputField>("m_goLogin/m_inputUserID");
        m_inputUserPassword = FindChildComponent<InputField>("m_goLogin/m_inputUserPassword");
        m_btnRegister = FindChildComponent<Button>("m_goLogin/m_btnRegister");
        m_btnLogin = FindChildComponent<Button>("m_goLogin/m_btnLogin");
        m_inputUserName = FindChildComponent<InputField>("m_goLogin/m_inputUserName");
        m_btnClose.onClick.AddListener(OnClickCloseBtn);
        m_btnRegister.onClick.AddListener(OnClickRegisterBtn);
        m_btnLogin.onClick.AddListener(OnClickLoginBtn);
    }
    #endregion

    protected override void OnCreate()
    {
        base.OnCreate();
        if (GameClient.Instance.Status != GameClientStatus.StatusConnect)
        {
            WebSocketMgr.Instance.Init();
        }

        if (GameDataMgr.Instance.HadCacheLoginData())
        {
            var userId = PlayerPrefs.GetString("userId");
            var password = PlayerPrefs.GetString("password");
            GameDataMgr.Instance.LoginReq(userId, password);
            Close();
        }
    }

    #region 事件
    private void OnClickCloseBtn()
    {
        Close();
    }
    private void OnClickRegisterBtn()
    {
        var userId = m_inputUserID.text;
        var password = m_inputUserPassword.text;
        var username = m_inputUserName.text;
        if (username.Length > 15 || userId.Length > 20 || password.Length > 20)
        {
            UISys.ShowTipMsg("字段太长了，请短一些哦~");
            return;
        }
        GameDataMgr.Instance.RegisiterReq(userId,password,username);
    }
    private void OnClickLoginBtn()
    {
        var userId = m_inputUserID.text;
        var password = m_inputUserPassword.text;
        var username = m_inputUserName.text;
        GameDataMgr.Instance.LoginReq(userId, password);
    }
    #endregion

}