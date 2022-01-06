using System.Collections.Generic;
using Google.Protobuf.Collections;
using RegicideProtocol;
using UnityEngine;
using UnityEngine.UI;

class RoomUI : UIWindow
{
    private List<RoomItem> m_roomList = new List<RoomItem>();
    private Transform m_tfContent;
    private GameObject m_itemRoom;
    private Button m_btnClose;
    private GameObject m_goCreateRoom;
    private InputField m_inputRoomName;
    private Button m_btnCreateRoom;
    private Button m_btnRefreshRoom;
    private Button m_btnPlayLocalMode;
    private Text m_textCreateRoom;
    private Text m_textUserName;
    private Text m_textUserId;
    protected override void ScriptGenerator()
    {
        m_tfContent = FindChild("ScrollView/Viewport/m_tfContent");
        m_itemRoom = FindChild("m_itemRoom").gameObject;
        m_btnClose = FindChildComponent<Button>("m_btnClose");
        m_goCreateRoom = FindChild("m_goCreateRoom").gameObject;
        m_inputRoomName = FindChildComponent<InputField>("m_goCreateRoom/m_inputRoomName");
        m_btnCreateRoom = FindChildComponent<Button>("m_goCreateRoom/m_btnCreateRoom");
        m_btnRefreshRoom = FindChildComponent<Button>("m_btnRefreshRoom");
        m_textCreateRoom = FindChildComponent<Text>("m_goCreateRoom/m_btnCreateRoom/m_textCreateRoom");
        m_textUserName = FindChildComponent<Text>("m_accountInfo/m_textUserName");
        m_textUserId = FindChildComponent<Text>("m_accountInfo/m_textUserId");
        m_btnPlayLocalMode = FindChildComponent<Button>("m_btnPlayLocalMode");
        m_btnClose.onClick.AddListener(OnClickCloseBtn);
        m_btnCreateRoom.onClick.AddListener(OnClickCreateRoomBtn);
        m_btnRefreshRoom.onClick.AddListener(Refresh);
        m_btnPlayLocalMode.onClick.AddListener(() =>
        {
            Close();
            if (GameMgr.Instance.IsLandScape)
            {
                UISys.Mgr.ShowWindow<GameUI>();
                GameMgr.Instance.RestartGame();
            }
            else
            {
                UISys.Mgr.ShowWindow<GameUILand>();
                GameMgr.Instance.RestartGame();
            }
        });
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        EventCenter.Instance.AddEventListener<RepeatedField<RoomPack>>("RoomPack", OnRefresh);
    }

    protected override void DeRegisterEvent()
    {
        base.DeRegisterEvent();
        EventCenter.Instance.RemoveEventListener<RepeatedField<RoomPack>>("RoomPack", OnRefresh);
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        Refresh();
        AdjustIconNum(m_roomList,30,m_tfContent,m_itemRoom);
        m_itemRoom.gameObject.SetActive(false);
        m_textUserName.text = string.Format("用户名：{0}", GameOnlineMgr.Instance.MyName);
        m_textUserId.text = string.Format("用户ID：：{0}", GameOnlineMgr.Instance.MyActorId);
    }

    private void OnRefresh(RepeatedField<RoomPack> roomPacks)
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
    private void OnClickCreateRoomBtn()
    {
        var roomName = m_inputRoomName.text;
        if (string.IsNullOrEmpty(roomName))
        {
            UISys.ShowTipMsg("请输入房间名称哦~");
            return;
        }
        else if (roomName.Length > 7)
        {
            UISys.ShowTipMsg("房间名字太长啦~");
            return;
        }
      
        RoomDataMgr.Instance.CreateRoomReq(m_inputRoomName.text,4);
    }

    private void Refresh()
    {
        RoomDataMgr.Instance.FindRoomReq();
        UISys.ShowTipMsg("刷新房间");
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
        if (m_roomPack != null)
        {
            RoomDataMgr.Instance.JoinRoomReq(m_roomPack.RoomID);
        }
        else
        {
            UISys.ShowTipMsg("这个房间有点问题");
        }
    }

    #region 事件
    #endregion

}