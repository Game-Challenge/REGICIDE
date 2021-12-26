using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class RoomUI : UIWindow
{
    private List<RoomItem> m_roomList = new List<RoomItem>();
    #region 脚本工具生成的代码
    private Transform m_tfContent;
    private GameObject m_itemRoom;
    private Button m_btnClose;
    protected override void ScriptGenerator()
    {
        m_tfContent = FindChild("ScrollView/Viewport/m_tfContent");
        m_itemRoom = FindChild("m_itemRoom").gameObject;
        m_btnClose = FindChildComponent<Button>("m_btnClose");
        m_btnClose.onClick.AddListener(OnClickCloseBtn);
    }
    #endregion

    protected override void OnCreate()
    {
        base.OnCreate();
        AdjustIconNum(m_roomList,30,m_tfContent,m_itemRoom);
        m_itemRoom.gameObject.SetActive(false);
    }

    #region 事件
    private void OnClickCloseBtn()
    {
        Close();
    }
    #endregion

}
class RoomItem : UIWindowWidget
{
    #region 脚本工具生成的代码
    private Transform m_tfContent;
    private Image m_imgIcon;
    private Text m_textRoomInfo;
    private Text m_textNum;
    private Text m_textRoomId;
    protected override void ScriptGenerator()
    {
        m_tfContent = FindChild("m_tfContent");
        m_imgIcon = FindChildComponent<Image>("m_tfContent/m_imgIcon");
        m_textRoomInfo = FindChildComponent<Text>("m_tfContent/m_textRoomInfo");
        m_textNum = FindChildComponent<Text>("m_tfContent/m_textNum");
        m_textRoomId = FindChildComponent<Text>("m_textRoomId");
    }
    #endregion

    protected override void OnCreate()
    {
        base.OnCreate();
        var rect = m_tfContent as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    #region 事件
    #endregion

}