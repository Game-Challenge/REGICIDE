package controllers

import (
	"Regicide/App/tserver"
	server "Regicide/App/tserver"
	GameProto "Regicide/GameProto"
	"errors"

	"github.com/wonderivan/logger"
)

func InitGameController() {
	controller := server.InstanceController("Game", GameProto.RequestCode_Game)
	controller.Funcs = map[string]interface{}{}
	controller.Funcs, _ = controller.AddFunction("ATTACK", Attack)
	controller.Funcs, _ = controller.AddFunction("SKILL", Skill)
	controller.Funcs, _ = controller.AddFunction("DAMAGE", Damage)
	controller.Funcs, _ = controller.AddFunction("HURT", Hurt)
	server.RegisterController(GameProto.RequestCode_Game, controller)
}

func Attack(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}
	if client.RoomInfo == nil {
		return nil, errors.New("roomInfo is nil")
	}
	if mainpack == nil {
		return nil, errors.New("mainpack is nil")
	}
	if mainpack.Roompack == nil || len(mainpack.Roompack) <= 0 {
		return nil, errors.New("mainpack.Roompack is nil")
	}
	if mainpack.Roompack[0].ActorPack == nil || len(mainpack.Roompack[0].ActorPack) <= 0 {
		return nil, errors.New("mainpack.Roompack[0].ActorPack[0] is nil")
	}

	choiceCards := mainpack.Roompack[0].ActorPack[0].CuttrntCards

	var bossDie bool
	choiceCardCount := len(choiceCards)
	if choiceCardCount > 0 {
		bossdie, err := client.AttackBoss(choiceCards)
		bossDie = bossdie
		if err != nil {
			logger.Error(err)
			return nil, err
		}

		if err != nil {
			logger.Error(err)
			return nil, err
		}

		for i := 0; i < choiceCardCount; i++ {
			client.Actor.CuttrntCards = tserver.RemoveCardData(client.Actor.CuttrntCards, choiceCards[i])
			// tserver.UsedCardList = append(tserver.UsedCardList, choiceCards[i])
		}
	}

	mainpack = &GameProto.MainPack{}
	mainpack.Actioncode = GameProto.ActionCode_ATTACK
	mainpack.Requestcode = GameProto.RequestCode_Game
	mainpack.Roompack = append(mainpack.Roompack, client.RoomInfo.RoomPack)
	mainpack.Returncode = GameProto.ReturnCode_Success
	if bossDie {
		client.RoomInfo.RoomPack.Gamestate.State = GameProto.GAMESTATE_STATE1
	} else {
		client.RoomInfo.RoomPack.Gamestate.State = GameProto.GAMESTATE_STATE4
	}

	client.RoomInfo.BroadcastTCP(client, mainpack)
	return mainpack, nil
}

func Skill(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {

	return nil, nil
}

func Damage(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {

	return nil, nil
}

func Hurt(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}
	if client.RoomInfo == nil {
		return nil, errors.New("roomInfo is nil")
	}
	if mainpack == nil {
		return nil, errors.New("mainpack is nil")
	}
	if mainpack.Roompack == nil || len(mainpack.Roompack) <= 0 {
		return nil, errors.New("mainpack.Roompack is nil")
	}
	if mainpack.Roompack[0].ActorPack == nil || len(mainpack.Roompack[0].ActorPack) <= 0 {
		return nil, errors.New("mainpack.Roompack[0].ActorPack[0] is nil")
	}
	choiceCards := mainpack.Roompack[0].ActorPack[0].CuttrntCards

	for i := 0; i < len(choiceCards); i++ {
		client.Actor.CuttrntCards = tserver.RemoveCardData(client.Actor.CuttrntCards, choiceCards[i])
	}

	mainpack = &GameProto.MainPack{}
	mainpack.Actioncode = GameProto.ActionCode_HURT
	mainpack.Requestcode = GameProto.RequestCode_Game
	mainpack.Roompack = append(mainpack.Roompack, client.RoomInfo.RoomPack)
	mainpack.Returncode = GameProto.ReturnCode_Success

	client.RoomInfo.RoomPack.Gamestate.State = GameProto.GAMESTATE_STATE1
	client.RoomInfo.GetNextIndex()

	client.RoomInfo.BroadcastTCP(client, mainpack)
	return mainpack, nil
}
