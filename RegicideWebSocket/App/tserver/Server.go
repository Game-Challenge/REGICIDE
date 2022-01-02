package tserver

import (
	"Regicide/App/middleware"
	"net/http"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/gorilla/websocket"
	"github.com/wonderivan/logger"
)

var ClientList = []*Client{}
var RoomList = []*Room{}
var ConnMap = make(map[uint32]*websocket.Conn)
var connid uint32 = 6000
var RoomID int32 = 1000

type Server struct {
	Port string
}

func StartServer(port string) Server {
	server := Server{Port: port}
	Start(server)
	return server
}

func Start(server Server) {
	// logfile, err := os.Create("./gin_http.log")
	// if err != nil {
	// 	logger.Debug("Could not create log file")
	// }
	// gin.SetMode(gin.DebugMode)
	// gin.DefaultWriter = io.MultiWriter(logfile)
	CreateRoom("断剑重铸之日")
	print("START REGICIDE SERVER")
	r := gin.Default()
	r.Use(middleware.CrosMiddleWare())
	r.GET("/ws", Ws)
	r.POST("/rankList", GetRankList)
	r.POST("/rankPush", PostRankData)
	r.Run(server.Port)
}

//设置websocket
//CheckOrigin防止跨站点的请求伪造
var upGrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool {
		return true
	},
}

//websocket实现
func Ws(c *gin.Context) {
	//升级get请求为webSocket协议
	ws, err := upGrader.Upgrade(c.Writer, c.Request, nil)
	if err != nil {
		return
	}

	connid++
	uniid := connid
	ConnMap[connid] = ws
	go handleClient(ws, uniid)
}

func handleClient(conn *websocket.Conn, uniid uint32) {
	defer conn.Close() //返回前关闭
	client := InstanceClient(conn, uniid)
	ClientList, _ = AddClient(client)
	// buf := make([]byte, 1024)
	for {
		//读取ws中的数据
		messagetype, message, err := conn.ReadMessage()
		if err != nil {
			logger.Debug(" conn.Read error", err)
			RemoveClient(client)
			break
		}
		//写入ws数据
		logger.Debug("messagetype:", messagetype, "   message:", message)
		buffCount := len(message)
		err2 := GetDataCenter().handBuffer(client, message[4:buffCount])
		if err2 != nil {
			logger.Debug("handBuffer error", err2)
			RemoveClient(client)
			break
		}
	}
}

func AddClient(client *Client) ([]*Client, error) {
	ClientList = append(ClientList, client)
	logger.Debug("Add client to Server =>", client.Addr, "  ClientCount =>", len(ClientList))

	if len(ClientList) > 3000 {
		os.Exit(1)
	}
	return ClientList, nil
}

func RemoveClient(client *Client) {
	room := client.RoomInfo
	room.RemoveClient(client)
	ClientList = RemoveC(ClientList, client)
	_, ok := ConnMap[client.Uniid]
	if ok {
		delete(ConnMap, client.Uniid)
	}
	logger.Debug("Rmv client from Server =>", client.Addr, "Uniid :=>", client.Uniid, "  ClientCount =>", len(ClientList))
}
