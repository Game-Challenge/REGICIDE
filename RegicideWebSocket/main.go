package main

import (
	"Regicide/App/common"
	"Regicide/App/controllers"
	"Regicide/App/tserver"
	"os"

	"github.com/spf13/viper"
)

func main() {
	print("START REGICIDE SERVER")
	// InitConfig()
	InitControllers()

	tserver.StartServer(":12345")
	select {}
}

func InitControllers() {
	controllers.InitUserController()
	controllers.InitRoomController()
	controllers.InitGameController()
}

func InitConfig() {
	workDir, _ := os.Getwd()
	viper.SetConfigName("config")
	viper.SetConfigType("yml")
	viper.AddConfigPath(workDir + "/config")

	err := viper.ReadInConfig()
	if err != nil {
		panic(err)
	}
}

func InitDatabase() {
	db := common.InitDB()
	defer db.Close()
}
