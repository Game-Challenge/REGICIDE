package tserver

import (
	GameProto "Regicide/GameProto"
	"errors"
	"strconv"

	"github.com/wonderivan/logger"
)

type Room struct {
	ClientList []*Client
	RoomPack   *GameProto.RoomPack
}

type BossActor struct {
	MaxHp     int32
	Hp        int32
	Atk       int32
	JokerAtk  bool
	CardInt   int32
	CardValue int32
	CardType  int32
}

func InstanceRoom(roomPack *GameProto.RoomPack) Room {
	room := Room{RoomPack: roomPack}
	return room
}

func CreateRoom(roomName string) Room {
	RoomID = RoomID + 1

	mainPack := &GameProto.MainPack{}
	mainPack.Requestcode = GameProto.RequestCode_Room
	roompack := &GameProto.RoomPack{}
	roompack.Roomname = roomName
	roompack.Maxnum = 4
	roompack.RoomID = RoomID
	room := InstanceRoom(roompack)
	RoomList = append(RoomList, &room)
	return room
}

func (room *Room) RemoveClient(client *Client) error {
	if room == nil {
		return errors.New("room is nil")
	}

	mainPack := &GameProto.MainPack{}
	mainPack.Requestcode = GameProto.RequestCode_Room
	mainPack.Actioncode = GameProto.ActionCode_UpCharacterList

	mainPack.Str = client.Username

	room.Broadcast(mainPack)

	room.ClientList = RemoveC(ClientList, client)
	room.RoomPack.Curnum = room.RoomPack.Curnum - 1

	for i := 0; i < len(room.RoomPack.ActorPack); i++ {

	}
	room.RoomPack.ActorPack = RemoveActor(room.RoomPack.ActorPack, client.Actor)

	logger.Debug("Rmv client from Room =>", client.Addr, "Uniid :=>", client.Uniid, "  ClientCount =>", len(room.ClientList))
	return nil
}

func (room *Room) Join(client *Client) {
	client.RoomInfo = room

	for i := 0; i < len(room.ClientList); i++ {
		if room.ClientList[i] == client {
			return
		}
	}
	room.ClientList = append(room.ClientList, client)
	room.RoomPack.Curnum = room.RoomPack.Curnum + 1
	room.RoomPack.ActorPack = append(room.RoomPack.ActorPack, client.Actor)
}

func (room *Room) StartGame(client *Client) {
	gameState := &GameProto.GameStatePack{}
	gameState.State = GameProto.GAMESTATE_STATE1
	room.RoomPack.Gamestate = gameState

	room.InitCards()
	room.InitMyCards()
	room.InitBoss()

	mainPack := &GameProto.MainPack{}
	mainPack.Requestcode = GameProto.RequestCode_Room
	mainPack.Actioncode = GameProto.ActionCode_StartGame

	for i := 0; i < len(room.ClientList); i++ {
		_client := room.ClientList[i]
		room.TurnCards(_client)
		playerpack := &GameProto.PlayerPack{}
		playerpack.Playername = _client.Username //todo _client.Username
		playerpack.PlayerID = strconv.Itoa(int(_client.ActorID))
		mainPack.Playerpack = append(mainPack.Playerpack, playerpack)
	}
	mainPack.Roompack = append(mainPack.Roompack, room.RoomPack)
	room.Broadcast(mainPack)
}

//遗弃
func (room *Room) Starting(client *Client) {
	gameState := &GameProto.GameStatePack{}
	gameState.State = GameProto.GAMESTATE_STATE1
	room.RoomPack.Gamestate = gameState

	mainPack := &GameProto.MainPack{}
	mainPack.Requestcode = GameProto.RequestCode_Room
	mainPack.Actioncode = GameProto.ActionCode_Starting

	for i := 0; i < len(room.ClientList); i++ {
		_client := room.ClientList[i]
		playerpack := &GameProto.PlayerPack{}
		playerpack.Playername = _client.Username
		playerpack.PlayerID = strconv.Itoa(int(_client.RoleId))

		mainPack.Playerpack = append(mainPack.Playerpack, playerpack)
	}
	room.Broadcast(mainPack)
}

func (room *Room) Broadcast(mainPack *GameProto.MainPack) {
	for i := 0; i < len(room.ClientList); i++ {
		room.ClientList[i].Send(mainPack)
	}
}

func (room *Room) BroadcastTCP(client *Client, mainPack *GameProto.MainPack) {
	if room == nil {
		logger.Emer("Broadcast failed room is nil!!!")
		return
	}
	for i := 0; i < len(room.ClientList); i++ {
		if room.ClientList[i] == client {
			continue
		}
		if room.ClientList[i] == nil {
			continue
		}
		room.ClientList[i].Send(mainPack)
	}
}
