using System;
using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;
using UnityEngine.UI;

class RoomWaitUI : UIWindow
{
    private List<RoomPlayerItem> m_items = new List<RoomPlayerItem>();
    #region 脚本工具生成的代码
    private Text m_textWaiting;
    private GameObject m_goRoomInfo;
    private GameObject m_goRoom;
    private Text m_textRoomId;
    private Text m_textRoomName;
    private Button m_btnStartGame;
    private Button m_btnExit;
    private Transform m_tfPlayerContent;
    private GameObject m_itemPlayer;
    private InputField m_inputChat;
    private Button m_btnSend;
    protected override void ScriptGenerator()
    {
        m_textWaiting = FindChildComponent<Text>("m_textWaiting");
        m_goRoomInfo = FindChild("m_goRoomInfo").gameObject;
        m_goRoom = FindChild("m_goRoomInfo/m_goRoom").gameObject;
        m_textRoomId = FindChildComponent<Text>("m_goRoomInfo/m_goRoom/m_textRoomId");
        m_textRoomName = FindChildComponent<Text>("m_goRoomInfo/m_goRoom/m_textRoomName");
        m_btnStartGame = FindChildComponent<Button>("m_goRoomInfo/m_btnStartGame");
        m_btnExit = FindChildComponent<Button>("m_goRoomInfo/m_btnExit");
        m_tfPlayerContent = FindChild("m_tfPlayerContent");
        m_itemPlayer = FindChild("m_tfPlayerContent/m_itemPlayer").gameObject;
        m_inputChat = FindChildComponent<InputField>("m_inputChat");
        m_btnSend = FindChildComponent<Button>("m_btnSend");
        m_btnStartGame.onClick.AddListener(OnClickStartGameBtn);
        m_btnExit.onClick.AddListener(OnClickExitBtn);
        m_btnSend.onClick.AddListener((() =>
        {
            if (!m_inputChat.text.Equals(string.Empty))
            {
                RoomDataMgr.Instance.ChatReq(m_inputChat.text);
                m_inputChat.text = string.Empty;
            }
        }));
    }

    private void OnClickExitBtn()
    {
        RoomDataMgr.Instance.ExitRoomReq();
    }
    #endregion

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
    }

    protected override void OnCreate()
    {
        base.OnCreate();

    }

    public void Refresh(MainPack mainPack)
    {
        var roomPack = mainPack.Roompack[0];

        var actorPack = roomPack.ActorPack;

        m_textRoomName.text = "房间名:" + roomPack.Roomname;
        m_textRoomId.text = "房间号:" + roomPack.RoomID.ToString();

        AdjustIconNum(m_items, actorPack.Count, m_tfPlayerContent,m_itemPlayer);
        for (int i = 0; i < actorPack.Count; i++)
        {
            m_items[i].Init(actorPack[i]);
        }
        m_itemPlayer.gameObject.SetActive(false);
    }

    #region 事件
    private void OnClickStartGameBtn()
    {
        RoomDataMgr.Instance.StartGameReq();
    }
    #endregion

}

class RoomPlayerItem : UIWindowWidget
{
    #region 脚本工具生成的代码

    private Image m_imgIsMe;
    private Text m_textName;
    private Text m_textID;
    private Image m_imgHeadIcon;
    private Button m_btnNick;
    protected override void ScriptGenerator()
    {
        m_textName = FindChildComponent<Text>("m_textName");
        m_textID = FindChildComponent<Text>("m_textID");
        m_imgHeadIcon = FindChildComponent<Image>("m_imgHeadIcon");
        m_btnNick = FindChildComponent<Button>("m_btnNick");
        m_imgIsMe = FindChildComponent<Image>("m_imgIsMe");
        m_btnNick.onClick.AddListener(OnClickNickBtn);
    }
    #endregion

    public void Init(PlayerPack playerPack)
    {
        if (playerPack == null)
        {
            return;
        }
        m_textName.text = playerPack.Playername;
        m_textID.text = playerPack.PlayerID;

        m_btnNick.gameObject.SetActive(false);
    }

    public void Init(ActorPack actorPack)
    {
        if (actorPack == null)
        {
            return;
        }

        m_textName.text = actorPack.ActorName;
        m_textID.text = actorPack.ActorId.ToString();

        m_btnNick.gameObject.SetActive(false);

        if (actorPack.ActorId == GameOnlineMgr.Instance.MyActorId)
        {
            m_imgIsMe.gameObject.SetActive(true);
        }
    }

    #region 事件
    private void OnClickNickBtn()
    {

    }
    #endregion

}
