using System;
using UnityEngine;
using UnityEngine.UI;

class RuleFirstUI : UIWindow
{
    private TypeWriter m_typeWriter;
    #region 脚本工具生成的代码
    private Button m_btnClose;
    private Text m_textSpecial;
    private Text m_text;
    private GameObject m_goTypeWriter;
    protected override void ScriptGenerator()
    {
        m_btnClose = FindChildComponent<Button>("m_btnClose");
        m_textSpecial = FindChildComponent<Text>("m_textSpecial");
        m_text = FindChildComponent<Text>("ScrollView/Viewport/Content/m_text");
        m_goTypeWriter = FindChild("ScrollView/Viewport/Content/m_text/m_goTypeWriter").gameObject;
        m_btnClose.onClick.AddListener(OnClickCloseBtn);
        m_typeWriter = m_goTypeWriter.GetComponent<TypeWriter>();
    }
    #endregion

    public void Init(string tag, string text)
    {
        if (!text.Equals(String.Empty))
        {
            m_typeWriter.text = text;
        }
        PlayerPrefs.SetInt(tag, 1);
    }

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    #region 事件
    private void OnClickCloseBtn()
    {
        Close();
    }
    #endregion

}