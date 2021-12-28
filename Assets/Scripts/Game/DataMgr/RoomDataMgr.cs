using System;
using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;

class RoomDataMgr : DataCenterModule<RoomDataMgr>
{
    private int RoomID = 0;
    private List<RoomPack> m_roomPacks = new List<RoomPack>();
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

        var ui = UISys.Mgr.ShowWindow<GameOnlineUI>();
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

        m_roomPacks.Clear();

        m_roomPacks = mainPack.Roompack.ToList();

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
            return;
        }
        UISys.Mgr.CloseWindow<GameUI>();
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