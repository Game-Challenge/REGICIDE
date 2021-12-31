package tserver

import (
	GameProto "Regicide/GameProto"

	"github.com/gorilla/websocket"
)

type Client struct {
	Addr     string
	Username string
	Uniid    uint32
	RoleId   uint32
	Conn     *websocket.Conn
	RoomInfo *Room
}

func InstanceClient(conn *websocket.Conn, uniid uint32) *Client {

	rAddr := conn.RemoteAddr()

	client := Client{Addr: rAddr.String(), Conn: conn, Uniid: uniid}

	return &client
}

var ClientBufferHead []byte = []byte{0, 0, 0, 0}

func (client *Client) SendTCP(mainpack *GameProto.MainPack) {
	if client == nil {
		return
	}
	if client.Conn == nil {
		return
	}

	GetDataCenter().sendBuffer(client, mainpack)
}
