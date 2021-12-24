using UnityEngine;
using UnityEngine.UI;

class InventoryUI : UIWindowWidget
{
    #region 脚本工具生成的代码
    private Transform m_tfInventoryList;
    private Button m_btnMenu;
    private Button m_btnSetting;
    private Button m_btnRestart;
    private Button m_btnLeft;
    private Button m_btnRight;
    protected override void ScriptGenerator()
    {
        m_tfInventoryList = FindChild("m_tfInventoryList");
        m_btnMenu = FindChildComponent<Button>("m_tfInventoryList/m_btnMenu");
        m_btnSetting = FindChildComponent<Button>("m_tfInventoryList/m_btnSetting");
        m_btnRestart = FindChildComponent<Button>("m_tfInventoryList/m_btnRestart");
        m_btnLeft = FindChildComponent<Button>("m_btnLeft");
        m_btnRight = FindChildComponent<Button>("m_btnRight");
        m_btnMenu.onClick.AddListener(OnClickMenuBtn);
        m_btnSetting.onClick.AddListener(OnClickSettingBtn);
        m_btnLeft.onClick.AddListener(OnClickLeftBtn);
        m_btnRight.onClick.AddListener(OnClickRightBtn);
        m_btnRestart.onClick.AddListener((() => {GameMgr.Instance.RestartGame(); }));
    }
    #endregion

    #region 事件
    private void OnClickMenuBtn()
    {
        UISys.Mgr.ShowWindow<SettingUI>();
    }
    private void OnClickSettingBtn()
    {
    }
    private void OnClickLeftBtn()
    {
    }
    private void OnClickRightBtn()
    {
    }
    #endregion

}