package main

import (
	"Regicide/App/common"
	"Regicide/App/controllers"
	"Regicide/App/tserver"
	"os"

	"github.com/spf13/viper"
	"github.com/wonderivan/logger"
)

func main() {
	print("START REGICIDE SERVER")
	InitConfig()
	InitDatabase()
	InitControllers()
	InitLogger()

	tserver.StartServer(":8767")
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
	//defer db.Close()
	db := common.InitDB()
	print(db)
}

func InitLogger() {
	print("\n 	 _   _      _ _    __        __         _     _ \n	| | | | ___| | | __\\ \\      / /__  _ __| | __| |\n	| |_| |/ _ \\ | |/ _ \\ \\ /\\ / / _ \\| '__| |/ _` |\n	|  _  |  __/ | | (_) \\ V  V / (_) | |  | | (_| |\n	|_| |_|\\___|_|_|\\___/ \\_/\\_/ \\___/|_|  |_|\\__,_|")
	logger.SetLogger("config/log.json")
}
