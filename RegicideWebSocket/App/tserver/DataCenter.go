package tserver

import (
	GameProto "Regicide/GameProto"
	"errors"
	"sync"

	"github.com/gogo/protobuf/proto"
	"github.com/wonderivan/logger"
)

type DataCenter struct {
	BufferHead []byte
}

var dataCenter *DataCenter
var syncOne sync.Once

func GetDataCenter() *DataCenter {
	syncOne.Do(func() {
		dataCenter = new(DataCenter)
		dataCenter.BufferHead = []byte{0, 0, 0, 0}
	})
	return dataCenter
}

func (dataCenter *DataCenter) handBuffer(client *Client, buf []byte) error {
	mainPack := &GameProto.MainPack{}

	protoData := buf

	err := proto.Unmarshal(protoData, mainPack)
	if err != nil {
		logger.Emer("marshal error: ", err.Error())
		panic(err)
	}

	logger.Crit("Receive from client:", client.Conn.RemoteAddr(), mainPack)

	err2 := handleReq(client, mainPack, false)

	if err2 != nil {
		return err2
	}
	return nil
}

func (dataCenter *DataCenter) sendBuffer(client *Client, mainpack *GameProto.MainPack) (*GameProto.MainPack, error) {
	if client == nil {
		return nil, errors.New("client is nil")
	}
	if mainpack == nil {
		return nil, errors.New("mainpack is nil")
	}

	conn := client.Conn

	if conn == nil {
		return mainpack, errors.New("conn is nil")
	}

	data, err := proto.Marshal(mainpack)

	if err != nil {
		logger.Emer("marshal error: ", err.Error())
		return nil, err
	}

	bodylen := len(data)

	dataCenter.BufferHead[0] = byte(bodylen)

	buff := append(dataCenter.BufferHead, data...)

	logger.Crit(conn.RemoteAddr(), "send mainpack: ", mainpack)

	// logger.Crit(conn.RemoteAddr(), "send buff: ", buff)

	err2 := conn.WriteMessage(2, buff)

	if err2 != nil {
		logger.Emer(err2)
		return nil, err2
	}
	return mainpack, err2
}

func (dataCenter *DataCenter) BuildProto(reqCode GameProto.RequestCode, action GameProto.ActionCode, retCode GameProto.ReturnCode) (*GameProto.MainPack, error) {
	mainPack := &GameProto.MainPack{}
	mainPack.Actioncode = action
	mainPack.Returncode = retCode
	return mainPack, nil
}
