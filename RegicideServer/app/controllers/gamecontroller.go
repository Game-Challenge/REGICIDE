package controllers

import (
	GameProto "RegicideServer/Gameproto"
	server "RegicideServer/app/tserver"
)

func InitGameController() {
	controller := server.InstanceController("Game", GameProto.RequestCode_Game)
	controller.Funcs = map[string]interface{}{}
	controller.Funcs, _ = controller.AddFunction("UpPos", UpPos)
	controller.Funcs, _ = controller.AddFunction("ATTACK", Attack)
	controller.Funcs, _ = controller.AddFunction("SKILL", UpPos)
	controller.Funcs, _ = controller.AddFunction("DAMAGE", Damage)
	controller.Funcs, _ = controller.AddFunction("HURT", Hurt)
	server.RegisterController(GameProto.RequestCode_Game, controller)
}

func UpPos(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	// if client == nil {
	// 	return nil, errors.New("client is nill")
	// }
	// if client.RoomInfo == nil {
	// 	return nil, errors.New("client roomInfo is nill")
	// }
	// client.RoomInfo.BroadcastUDP(client, mainpack)
	// client.UpPos(mainpack)
	return nil, nil
}

func Attack(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {

	return nil, nil
}

func Skill(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {

	return nil, nil
}

func Damage(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {

	return nil, nil
}

func Hurt(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {

	return nil, nil
}
