using System.Collections.Generic;
using RegicideProtocol;
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

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        EventCenter.Instance.AddEventListener<List<RoomPack>>("RoomPack", OnRefresh);
    }

    protected override void DeRegisterEvent()
    {
        base.DeRegisterEvent();
        EventCenter.Instance.RemoveEventListener<List<RoomPack>>("RoomPack", OnRefresh);
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        RoomDataMgr.Instance.FindRoomReq();
        AdjustIconNum(m_roomList,30,m_tfContent,m_itemRoom);
        m_itemRoom.gameObject.SetActive(false);
    }

    private void OnRefresh(List<RoomPack> roomPacks)
    {
        AdjustIconNum(m_roomList, roomPacks.Count, m_tfContent, m_itemRoom);
        m_itemRoom.gameObject.SetActive(false);

        for (int i = 0; i < m_roomList.Count; i++)
        {
            m_roomList[i].Init(roomPacks[i]);
        }
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
    private RoomPack m_roomPack;
    #region 脚本工具生成的代码
    private Button m_btnChoice;
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
        m_btnChoice = UnityUtil.GetComponent<Button>(gameObject);
        m_btnChoice.onClick.AddListener(OnChoice);
    }
    #endregion

    protected override void OnCreate()
    {
        base.OnCreate();
        var rect = m_tfContent as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void Init(RoomPack roomPack)
    {
        m_roomPack = roomPack;
        m_textRoomId.text = roomPack.RoomID.ToString();
        m_textRoomInfo.text = roomPack.Roomname;
        m_textNum.text = string.Format("{0}/{1}",roomPack.Curnum,roomPack.Maxnum);
    }

    private void OnChoice()
    {
        RoomDataMgr.Instance.JoinRoomReq(m_roomPack.RoomID);
    }

    #region 事件
    #endregion

}