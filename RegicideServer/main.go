package main

import (
	"RegicideServer/app/common"
	"RegicideServer/app/controllers"
	server "RegicideServer/app/tserver"
	config "RegicideServer/config"
	manager "RegicideServer/manager"
	"fmt"
	"os"

	_ "github.com/go-sql-driver/mysql"
	"github.com/spf13/viper"
)

func main() {
	InitConfig()
	InitDatabase()
	// manager.Testlog()
	manager.SetLogger()
	InitControllers()
	go server.StartServer(config.TCPport)
	// go server.ListenCMD()
	manager.Plot()
}

// All Controllers Will Init On Here
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
	fmt.Println(db)
	defer db.Close()
}
