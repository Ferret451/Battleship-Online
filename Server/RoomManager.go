package main

import (
	"crypto/rand"
	"encoding/base64"
	"fmt"
	"github.com/gorilla/websocket"
	"log"
	"net/http"
)

var upgrader = websocket.Upgrader{
	ReadBufferSize:  1024,
	WriteBufferSize: 1024,
}

const (
	roomCodeLength = 6
)

// Room represents a game room with a unique code and players.
type Room struct {
	Code    string
	Players []*websocket.Conn
}

var rooms map[string]*Room

func generateRoomCode() string {
	bytes := make([]byte, roomCodeLength)
	_, err := rand.Read(bytes)
	if err != nil {
		log.Fatal("Error generating random bytes:", err)
	}

	return base64.URLEncoding.EncodeToString(bytes)[:roomCodeLength]
}

func createRoomHandler(w http.ResponseWriter, r *http.Request) {
	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println(err)
		return
	}
	defer conn.Close()

	code := generateRoomCode()
	room := &Room{
		Code:    code,
		Players: []*websocket.Conn{conn},
	}
	rooms[code] = room

	fmt.Println("Room created with code:", code)

	conn.WriteMessage(websocket.TextMessage, []byte(code))
}

func joinRoomHandler(w http.ResponseWriter, r *http.Request) {
	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println(err)
		return
	}
	defer conn.Close()

	code := r.URL.Query().Get("code")
	room, ok := rooms[code]
	if !ok {
		log.Println("Room not found:", code)
		return
	}

	room.Players = append(room.Players, conn)
	fmt.Println("Player joined room with code:", code)
}
