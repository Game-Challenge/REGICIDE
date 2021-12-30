package tserver

import (
	GameProto "Regecide/GameProto"
	"errors"
	"reflect"
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

	logger.Crit(conn.RemoteAddr(), "send buff: ", buff)

	err2 := conn.WriteMessage(len(buff), buff)

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

type Controller struct {
	ControllerName string
	RequestCode    GameProto.RequestCode
	Funcs          map[string]interface{}
	Action         func(client Client, mainpack *GameProto.MainPack, isUdp bool) (*GameProto.MainPack, error)
}

var controllerMap map[GameProto.RequestCode]Controller = map[GameProto.RequestCode]Controller{}

func handleReq(client *Client, mainpack *GameProto.MainPack, isUdp bool) error {
	conn := client.Conn
	if conn == nil {
		return errors.New("conn is nil")
	}
	if mainpack == nil {
		return errors.New("mainpack is nil")
	}

	requestType := mainpack.Requestcode

	ctr, ok := controllerMap[requestType]
	if ok {
		if isUdp {

		} else {
			valuePack, err := ctr.Call(mainpack.Actioncode.String(), client, mainpack, isUdp)
			if err != nil {
				return err
			}
			var sendpack *GameProto.MainPack = valuePack[0].Interface().(*GameProto.MainPack)

			// err = valuePack[1].Interface().(error)
			if sendpack == nil {
				return nil
			}
			if err != nil {
				return err
			}
			logger.Debug(sendpack)

			GetDataCenter().sendBuffer(client, sendpack)
		}
	} else {
		logger.Error("Unknown request", ctr, requestType)
	}

	return nil
}

func (controller *Controller) Call(name string, params ...interface{}) (result []reflect.Value, err error) {
	_, hadValue := controller.Funcs[name]
	if !hadValue {
		logger.Emer("Func called %d, is not registered", name)
		return nil, errors.New("name = nil")
	}

	f := reflect.ValueOf(controller.Funcs[name])
	if f.Type() == nil {
		return nil, errors.New("f = nil")
	}

	if len(params) != f.Type().NumIn() {
		return nil, errors.New("The number of params is not adapted.")
	}

	in := make([]reflect.Value, len(params))

	for key, param := range params {
		in[key] = reflect.ValueOf(param)
	}

	result = f.Call(in)
	return result, err
}
