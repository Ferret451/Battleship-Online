package main

import (
	"crypto/rand"
	"encoding/base64"
	"github.com/gorilla/websocket"
	"log"
	"net/http"
	"time"
)

var upgrader = websocket.Upgrader{
	ReadBufferSize:  1024,
	WriteBufferSize: 1024,
}

const (
	roomCodeLength = 6
)

type Player struct {
	Connection *websocket.Conn
	IsReady    bool
}

// Room represents a game room with a unique code and players.
type Room struct {
	Code    string
	Players []Player
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
		conn.Close()
		return
	}

	code := generateRoomCode()
	room := &Room{
		Code:    code,
		Players: make([]Player, 0),
	}
	newPlayer := Player{conn, false}
	room.Players = append(room.Players, newPlayer)
	rooms[code] = room

	log.Println("Room created with code:", code)

	message := createMessage("code", "server", code)
	err = conn.WriteMessage(websocket.TextMessage, message)
	if err != nil {
		log.Println(err)
	}

	go room.handlePlayerMessages(conn)

	go room.cleanup(code)
}

func joinRoomHandler(w http.ResponseWriter, r *http.Request) {
	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println(err)

		conn.Close()
		return
	}

	code := r.URL.Query().Get("code")
	room, ok := rooms[code]
	if !ok {
		message := createMessage("error", "server", "No such code!")
		err = conn.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			log.Println(err)
		}

		conn.Close()
		return
	}

	if len(room.Players) == 2 {
		message := createMessage("error", "server", "Room is fulfill!")
		err = conn.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			log.Println(err)
		}

		conn.Close()
		return
	}

	message := createMessage("code", "server", code)
	err = conn.WriteMessage(websocket.TextMessage, message)
	if err != nil {
		log.Println(err)
	}

	newPlayer := Player{conn, false}
	room.Players = append(room.Players, newPlayer)
	log.Println("Player joined room with code:", code)

	go room.handlePlayerMessages(conn)

	go room.cleanup(code)
}

func (room *Room) cleanup(code string) {
	ticker := time.Tick(time.Minute)
	for range ticker {
		if len(room.Players) == 0 {
			log.Println("Cleaning up room with code:", code)
			delete(rooms, code)
			break
		}
	}
}
