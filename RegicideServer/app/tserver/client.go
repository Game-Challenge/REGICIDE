package tserver

import (
	"net"

	GameProto "RegicideServer/Gameproto"

	"github.com/gogo/protobuf/proto"
	"github.com/wonderivan/logger"
)

type Actor struct {
	RoleId         uint32
	RoomInfo       *Room
	PosPack        *GameProto.PosPack
	AppearancePack *GameProto.AppearancePack
}

type Client struct {
	Addr     string
	Conn     net.Conn
	UDPConn  *net.UDPConn
	UDPAddr  *net.UDPAddr
	Username string
	RoomInfo *Room
	PosPack  *GameProto.PosPack
	Uniid    uint32
	RoleId   uint32
	Actor    *Actor
}

func InstanceClient(conn net.Conn, uniid uint32) *Client {

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
	return
	data, err := proto.Marshal(mainpack)
	if err != nil {
		logger.Emer("marshal error: ", err.Error())
		return
	}

	bodylen := len(data)

	ClientBufferHead[0] = byte(bodylen)

	logger.Info("bodylen: ", bodylen)

	logger.Info("byte(bodylen): ", byte(bodylen))

	buff := append(ClientBufferHead, data...)

	logger.Debug("send mainpack: ", mainpack)
	logger.Debug("send buff: ", buff)

	_, err2 := client.Conn.Write(buff)

	if err2 != nil {
		logger.Debug("marshal error: ", err2.Error())
	}
}
