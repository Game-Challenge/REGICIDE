﻿using UnityEngine;
using UnityEngine.UI;

class InventoryUI : UIWindowWidget
{
    #region 脚本工具生成的代码
    private Transform m_tfInventoryList;
    private Button m_btnBack;
    private Button m_btnSetting;
    private Button m_btnRestart;
    private Button m_btnMenu;
    private Button m_btnChoiceLv;
    private Button m_btnRank;
    private Button m_btnLeft;
    private Button m_btnRight;
    protected override void ScriptGenerator()
    {
        m_tfInventoryList = FindChild("m_tfInventoryList");
        m_btnBack = FindChildComponent<Button>("m_tfInventoryList/m_btnBack");
        m_btnSetting = FindChildComponent<Button>("m_tfInventoryList/m_btnSetting");
        m_btnRestart = FindChildComponent<Button>("m_tfInventoryList/m_btnRestart");
        m_btnMenu = FindChildComponent<Button>("m_tfInventoryList/m_btnMenu");
        m_btnChoiceLv = FindChildComponent<Button>("m_tfInventoryList/m_btnChoiceLv");
        m_btnRank = FindChildComponent<Button>("m_tfInventoryList/m_btnRank");
        m_btnLeft = FindChildComponent<Button>("m_btnLeft");
        m_btnRight = FindChildComponent<Button>("m_btnRight");
        m_btnBack.onClick.AddListener(OnClickBackBtn);
        m_btnSetting.onClick.AddListener(OnClickSettingBtn);
        m_btnRestart.onClick.AddListener(OnClickRestartBtn);
        m_btnMenu.onClick.AddListener(OnClickMenuBtn);
        m_btnChoiceLv.onClick.AddListener(OnClickChoiceLvBtn);
        m_btnRank.onClick.AddListener(OnClickRankBtn);
        m_btnLeft.onClick.AddListener(OnClickLeftBtn);
        m_btnRight.onClick.AddListener(OnClickRightBtn);
    }
    #endregion

    #region 事件
    private void OnClickBackBtn()
    {
        Close();
        UISys.Mgr.CloseWindow<GameUI>();
        UISys.Mgr.ShowWindow<StartUI>();
    }
    private void OnClickSettingBtn()
    {
        UISys.Mgr.ShowWindow<SettingUI>();
    }
    private void OnClickRestartBtn()
    {
        GameMgr.Instance.RestartGame();
    }
    private void OnClickMenuBtn()
    {
        UISys.ShowTipMsg("敬请期待");
    }
    private void OnClickChoiceLvBtn()
    {
        UISys.Mgr.ShowWindow<LevelChoiceUI>();
    }
    private void OnClickRankBtn()
    {
        UISys.ShowTipMsg("敬请期待");
    }
    private void OnClickLeftBtn()
    {
    }
    private void OnClickRightBtn()
    {
    }
    #endregion

}
