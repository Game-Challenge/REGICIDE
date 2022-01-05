using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using RegicideProtocol;
using UnityEngine;

class RoomDataMgr : DataCenterModule<RoomDataMgr>
{
    private int RoomID = 0;
    private RepeatedField<RoomPack> m_roomPacks = new RepeatedField<RoomPack>();
    public override void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.FindRoom, FindRoomRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.CreateRoom, CreateRoomRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.JoinRoom, JoinRoomRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Exit, ExitRoomRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.StartGame, StartGameRes);
    }
    public void StartGameReq()
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.StartGame);
        RoomPack roomPack = new RoomPack();
        roomPack.RoomID = RoomID;
        mainPack.Roompack.Add(roomPack);
        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void StartGameRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }
        Debug.Log(mainPack);

        GameOnlineMgr.Instance.InitGame(mainPack);

        UISys.Mgr.CloseWindow<RoomWaitUI>();

        var ui = UISys.Mgr.ShowWindow<GameOnlineUI>();

        ui.Init(mainPack);
    }

    public void FindRoomReq()
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.FindRoom);

        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void FindRoomRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }
        Debug.Log(mainPack);

        m_roomPacks = mainPack.Roompack;

        EventCenter.Instance.EventTrigger("RoomPack", m_roomPacks);
    }

    public void CreateRoomReq(string roomName, int maxNum)
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.CreateRoom);
        RoomPack room = new RoomPack();
        room.Roomname = roomName;
        room.Maxnum = maxNum;
        mainPack.Roompack.Add(room);
        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void CreateRoomRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }

        Debug.Log(mainPack);
        RoomID = mainPack.Roompack[0].RoomID;

        UISys.Mgr.CloseWindow<GameUI>();
        UISys.Mgr.CloseWindow<GameUILand>();
        UISys.Mgr.CloseWindow<RoomUI>();

        Debug.Log(mainPack);
        RoomID = mainPack.Roompack[0].RoomID;
        var ui = UISys.Mgr.ShowWindow<RoomWaitUI>();
        ui.Refresh(mainPack);
    }

    public void JoinRoomReq(int roomID,string password = "")
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.JoinRoom);
        mainPack.Str = roomID.ToString();
        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void JoinRoomRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            if (mainPack!=null&&string.IsNullOrEmpty(mainPack.Str))
            {
                UISys.ShowTipMsg(mainPack.Str);
            }
            return;
        }
        UISys.Mgr.CloseWindow<GameUI>();
        UISys.Mgr.CloseWindow<GameUILand>();
        UISys.Mgr.CloseWindow<RoomUI>();

        Debug.Log(mainPack);
        RoomID = mainPack.Roompack[0].RoomID;
        var ui = UISys.Mgr.ShowWindow<RoomWaitUI>();
        ui.Refresh(mainPack);
    }

    public void ExitRoomReq()
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.Exit);
        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void ExitRoomRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }

        Debug.Log(mainPack);
    }
}