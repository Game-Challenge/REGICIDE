package controllers

import (
	"Regicide/App/common"
	"Regicide/App/model"
	server "Regicide/App/tserver"
	GameProto "Regicide/GameProto"
	"errors"
	"fmt"

	"github.com/jinzhu/gorm"
	"golang.org/x/crypto/bcrypt"
)

func InitUserController() {
	controller := server.InstanceController("User", GameProto.RequestCode_User)
	controller.Funcs = map[string]interface{}{}
	controller.Funcs, _ = controller.AddFunction("Login", Login)
	controller.Funcs, _ = controller.AddFunction("Register", Register)
	server.RegisterController(GameProto.RequestCode_User, controller)
}

func Login(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}
	if CheckLogin(mainpack, client) {
		mainpack.Returncode = GameProto.ReturnCode_Success
		client.Username = mainpack.LoginPack.Username
	} else {
		mainpack.Returncode = GameProto.ReturnCode_Fail
	}
	return mainpack, nil
}

func Register(client *server.Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}
	userNameStr := mainpack.LoginPack.Username
	var RoleId int
	DB := common.GetDB()
	if isUserExist(DB, userNameStr) {
		mainpack.Str = "已经存在用户"
		mainpack.Returncode = GameProto.ReturnCode_Fail
		return mainpack, nil
	} else {
		//创建用户
		//加密密码
		roleid := InstanceRoleId(DB)
		RoleId = roleid
		hasedPassword, _ := bcrypt.GenerateFromPassword([]byte(mainpack.LoginPack.Password), bcrypt.DefaultCost)
		newUser := model.User{
			Name:     mainpack.Str,
			Username: userNameStr,
			Password: string(hasedPassword),
			Roleid:   roleid,
		}
		DB.Create(&newUser)
	}

	mainpack.LoginPack.Username = mainpack.Str
	mainpack.Str = fmt.Sprint(RoleId)
	mainpack.Returncode = GameProto.ReturnCode_Success

	client.ActorID = RoleId
	client.Actor.ActorId = int32(RoleId)
	return mainpack, nil
}

func CheckLogin(mainpack *GameProto.MainPack, client *server.Client) bool {
	//todo check mysqlLogin
	DB := common.GetDB()
	//判断账号号是否存在
	var user model.User
	DB.Where("username = ?", mainpack.LoginPack.Username).First(&user)
	if user.ID == 0 {
		mainpack.Str = "用户不存在"
		return false
	}

	//判断账号密码正确与否
	if err := bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(mainpack.LoginPack.Password)); err != nil {
		mainpack.Str = "密码错误"
		return false
	}

	for i := 0; i < len(server.ClientList); i++ {
		if user.Roleid == int(server.ClientList[i].RoleId) {
			mainpack.Str = "已经登录了该账号"
			return false
		}
	}

	mainpack.LoginPack.Username = user.Name
	client.RoleId = uint32(user.Roleid)
	mainpack.Str = fmt.Sprint(client.RoleId)
	client.ActorID = user.Roleid
	client.Actor.ActorId = int32(user.Roleid)
	return true
}

func isUserExist(db *gorm.DB, userName string) bool {
	var user model.User
	db.Where("username = ?", userName).First(&user)
	return user.ID != 0
}

func InstanceRoleId(db *gorm.DB) int {
	// 获取最后一条记录，按主键排序
	var user model.User
	db.Last(&user)
	Roleid := user.ID + 1000
	return int(Roleid)
}
