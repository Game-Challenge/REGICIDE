package main

import (
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/gorilla/websocket"
)

//设置websocket
//CheckOrigin防止跨站点的请求伪造
var upGrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool {
		return true
	},
}

var ConnMap = make(map[uint32]*Conn)

var ConnIdMap = make(map[uint32]*websocket.Conn)

var RoomList = []*Room{}

var connid uint32

type Conn struct {
	connid    uint32
	websocket *websocket.Conn
}

type Room struct {
	roomId       uint32
	userNum      uint32
	roomName     string
	roomUsePW    bool
	roomPassword string
	conn         *Conn
}

//websocket实现
func handleWebSocket(c *gin.Context) {
	//升级get请求为webSocket协议

	ws, err := upGrader.Upgrade(c.Writer, c.Request, nil)
	if err != nil {
		return
	}
	connid++
	ConnIdMap[connid] = ws
	conn := &Conn{connid: connid, websocket: ws}
	ConnMap[connid] = conn

	defer ws.Close() //返回前关闭
	for {
		//读取ws中的数据
		mt, message, err := ws.ReadMessage()
		if err != nil {
			break
		}
		//写入ws数据
		err = ws.WriteMessage(mt, message)
		if err != nil {
			break
		}
	}
}

func main() {
	print("START REGICIDE SERVER")
	r := gin.Default()
	r.GET("/ws", handleWebSocket)
	r.Run("127.0.0.1:8080")
}
