package controllers

import (
	"Regicide/App/tserver"
	server "Regicide/App/tserver"
	GameProto "Regicide/GameProto"
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
	controller.Funcs, _ = controller.AddFunction("CreateRoom", CreateRoom)
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

var roomID int32 = 1000

func CreateRoom(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}
	roomID = roomID + 1

	roompack := &GameProto.RoomPack{}
	roompack.Roomname = mainpack.Roompack[0].Roomname
	roompack.Maxnum = mainpack.Roompack[0].Maxnum
	roompack.RoomID = roomID
	room := tserver.InstanceRoom(roompack)
	tserver.RoomList = append(tserver.RoomList, &room)
	mainpack.Returncode = GameProto.ReturnCode_Success

	room.RoomPack.RoomID = roomID
	room.Join(client)
	// playerpack := &GameProto.PlayerPack{}
	// playerpack.Playername = strconv.Itoa(int(client.Uniid)) //todo_client.Username
	// playerpack.PlayerID = strconv.Itoa(int(client.Uniid))
	// mainpack.Playerpack = append(mainpack.Playerpack, playerpack)

	mainpack.Roompack = append(mainpack.Roompack, room.RoomPack)

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
				if room.RoomPack.State == 1 {
					mainpack.Returncode = GameProto.ReturnCode_Fail
					mainpack.Str = "房间已经开始游戏了~"
					return mainpack, nil
				}

				if room.RoomPack.Curnum >= room.RoomPack.Maxnum {
					mainpack.Returncode = GameProto.ReturnCode_Fail
					mainpack.Str = "房间人数已满"
					return mainpack, nil
				}

				room.Join(client)

				for i := 0; i < len(room.ClientList); i++ {
					// _client := room.ClientList[i]
					// playerpack := &GameProto.PlayerPack{}
					// playerpack.Playername = strconv.Itoa(int(_client.Uniid)) //todo_client.Username
					// playerpack.PlayerID = strconv.Itoa(int(_client.Uniid))
					// mainpack.Playerpack = append(mainpack.Playerpack, playerpack)

					mainpack.Roompack = append(mainpack.Roompack, room.RoomPack)
				}

				mainpack.Returncode = GameProto.ReturnCode_Success
				room.BroadcastTCP(client, mainpack)
				return mainpack, nil
			}
		}
	}

	mainpack.Returncode = GameProto.ReturnCode_Fail
	mainpack.Str = "没有该房间"
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
		roompack.RoomID = room.RoomPack.RoomID
		mainpack.Roompack = append(mainpack.Roompack, roompack)
	}

	mainpack.Returncode = GameProto.ReturnCode_Success
	return mainpack, nil
}

//开始游戏
func StartGame(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}
	count := len(server.RoomList)

	for i := 0; i < count; i++ {
		room := server.RoomList[i]
		if room.RoomPack.RoomID == mainpack.Roompack[0].RoomID {
			room.RoomPack.State = 1
			room.StartGame(client)
			// mainpack.Returncode = GameProto.ReturnCode_Success
			return nil, nil
		}
	}

	mainpack.Returncode = GameProto.ReturnCode_Fail
	mainpack.Str = "没有该房间"
	return mainpack, nil
}
