﻿using RegicideProtocol;
using UnityEngine;

class RoomDataMgr : DataCenterModule<RoomDataMgr>
{
    public override void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.FindRoom, FindRoomRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.CreateRoom, CreateRoomRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.JoinRoom, JoinRoomRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Exit, ExitRoomRes);
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
    }

    public void JoinRoomReq(string roomName)
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.JoinRoom);
        mainPack.Str = roomName;
        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void JoinRoomRes(MainPack mainPack)
    {
        if (Utils.CheckHaveError(mainPack))
        {
            return;
        }

        Debug.Log(mainPack);
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