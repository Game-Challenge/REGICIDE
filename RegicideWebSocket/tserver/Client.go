package tserver

import "github.com/gorilla/websocket"

type Client struct {
	Addr     string
	Username string
	Uniid    uint32
	RoleId   uint32
	Conn     *websocket.Conn
}
