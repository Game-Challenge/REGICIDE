package controllers

import (
	"Regicide/App/tserver"
	server "Regicide/App/tserver"
	GameProto "Regicide/GameProto"
	"errors"
	"strconv"

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

	if mainpack.Str == "JKR" {
		idx, _ := strconv.Atoi(mainpack.User)
		choiceIndex := int32(idx)
		mainpack = &GameProto.MainPack{}
		mainpack.Actioncode = GameProto.ActionCode_ATTACK
		mainpack.Requestcode = GameProto.RequestCode_Game
		mainpack.Returncode = GameProto.ReturnCode_Success

		client.RoomInfo.SetPlayerIndex(choiceIndex)
		mainpack.Roompack = append(mainpack.Roompack, client.RoomInfo.RoomPack)
		mainpack.Str = "JKRCH"
		client.RoomInfo.Broadcast(mainpack)
		return nil, nil
	}

	choiceCards := mainpack.Roompack[0].ActorPack[0].CuttrntCards

	var bossDie bool
	var gameLose bool
	choiceCardCount := len(choiceCards)
	if choiceCardCount > 0 {
		bossdie, err := client.AttackBoss(choiceCards)

		if err != nil {
			logger.Error(err)
			return nil, err
		}

		if tserver.CurrentBossBeJokerAtk {
			for i := 0; i < choiceCardCount; i++ {
				client.Actor.CuttrntCards = tserver.RemoveCardData(client.Actor.CuttrntCards, choiceCards[i])
				//放进弃牌堆
				tserver.CurrentAttackCardList = append(tserver.UsedCardList, choiceCards[i])
				//放入协议通知客户端
				client.RoomInfo.RoomPack.CurrentUseCards = append(client.RoomInfo.RoomPack.CurrentUseCards, choiceCards[i])
			}

			mainpack = &GameProto.MainPack{}
			mainpack.Actioncode = GameProto.ActionCode_ATTACK
			mainpack.Requestcode = GameProto.RequestCode_Game
			mainpack.Returncode = GameProto.ReturnCode_Success
			mainpack.Str = "JKR"
			client.RoomInfo.Broadcast(mainpack)
			return nil, nil
		}
		bossDie = bossdie

		for i := 0; i < choiceCardCount; i++ {
			client.Actor.CuttrntCards = tserver.RemoveCardData(client.Actor.CuttrntCards, choiceCards[i])
			//放进弃牌堆
			tserver.CurrentAttackCardList = append(tserver.UsedCardList, choiceCards[i])
			//放入协议通知客户端
			client.RoomInfo.RoomPack.CurrentUseCards = append(client.RoomInfo.RoomPack.CurrentUseCards, choiceCards[i])
			// tserver.UsedCardList = append(tserver.UsedCardList, choiceCards[i])
		}
		var currentCardsValue int32
		for i := 0; i < len(client.Actor.CuttrntCards); i++ {
			currentCardsValue += client.Actor.CuttrntCards[i].CardValue
		}
		if currentCardsValue < client.RoomInfo.RoomPack.BossActor.ATK && !bossDie {
			gameLose = true
		}
	}

	mainpack = &GameProto.MainPack{}
	mainpack.Actioncode = GameProto.ActionCode_ATTACK
	mainpack.Requestcode = GameProto.RequestCode_Game
	mainpack.Roompack = append(mainpack.Roompack, client.RoomInfo.RoomPack)
	mainpack.Returncode = GameProto.ReturnCode_Success
	if gameLose {
		mainpack.Str = "GAMELOSE"
		client.RoomInfo.RoomPack.State = 0
		client.RoomInfo.RoomPack.Gamestate.State = GameProto.GAMESTATE_STATE1
		client.RoomInfo.Broadcast(mainpack)
		return nil, nil
	} else {
		if bossDie {
			client.RoomInfo.RoomPack.Gamestate.State = GameProto.GAMESTATE_STATE1
			mainpack.Roompack[0].Gamestate.State = GameProto.GAMESTATE_STATE1
		} else {
			client.RoomInfo.RoomPack.Gamestate.State = GameProto.GAMESTATE_STATE4
			mainpack.Roompack[0].Gamestate.State = GameProto.GAMESTATE_STATE4
		}
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
		//放进弃牌堆
		tserver.UsedCardList = append(tserver.UsedCardList, choiceCards[i])
		//放入协议通知客户端
		client.RoomInfo.RoomPack.CurrentUseCards = append(client.RoomInfo.RoomPack.CurrentUseCards, choiceCards[i])
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
