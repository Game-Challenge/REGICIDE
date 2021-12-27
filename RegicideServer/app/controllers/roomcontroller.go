package controllers

import (
	GameProto "RegicideServer/Gameproto"
	server "RegicideServer/app/tserver"
	"errors"
	"strconv"

	"github.com/wonderivan/logger"
)

func InitRoomController() {
	controller := server.InstanceController("Room", GameProto.RequestCode_Room)
	controller.Funcs = map[string]interface{}{}
	controller.Funcs, _ = controller.AddFunction("JoinRoom", JoinRoom)
	controller.Funcs, _ = controller.AddFunction("Chat", Chat)
	controller.Funcs, _ = controller.AddFunction("FindRoom", FindRoom)
	controller.Funcs, _ = controller.AddFunction("StartGame", StartGame)
	server.RegisterController(GameProto.RequestCode_Room, controller)
}

func Chat(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nill")
	}
	if client.RoomInfo == nil {
		return nil, errors.New("client roomInfo is nill")
	}
	mainpack.User = client.Username

	mainpack.Returncode = GameProto.ReturnCode_Success
	client.RoomInfo.BroadcastTCP(client, mainpack)
	return nil, nil
}

func CreateRoom(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	mainpack.Returncode = GameProto.ReturnCode_Success
	return mainpack, nil
}

func JoinRoom(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}

	int64_, err := strconv.ParseInt(mainpack.Str, 10, 64)
	if err != nil {
		return nil, err
	}
	int32_ := int32(int64_)

	if len(server.RoomList) == 0 {
		logger.Error("RoomList count is empty")
	} else {
		for i := 0; i < len(server.RoomList); i++ {

			room := server.RoomList[i]
			if room.RoomPack.RoomID == int32_ {
				room.Join(client)
				mainpack.Returncode = GameProto.ReturnCode_Success
				break
			}
		}
	}

	return mainpack, nil
}

func FindRoom(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}

	for i := 0; i < len(server.RoomList); i++ {
		room := server.RoomList[i]
		roompack := &GameProto.RoomPack{}
		roompack.Roomname = room.RoomPack.Roomname
		roompack.Maxnum = room.RoomPack.Maxnum
		roompack.Gamestate = room.RoomPack.Gamestate
		roompack.Curnum = room.RoomPack.Curnum
		mainpack.Roompack = append(mainpack.Roompack, roompack)
	}

	mainpack.Returncode = GameProto.ReturnCode_Success
	return mainpack, nil
}

func StartGame(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}

	count := len(server.RoomList)

	for i := 0; i < count; i++ {
		room := server.RoomList[i]
		if room.RoomPack.RoomID == mainpack.Roompack[0].RoomID {
			room.Starting(client)
		}
	}

	mainpack.Returncode = GameProto.ReturnCode_Success
	return mainpack, nil
}
