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

type Player struct {
	Connection *websocket.Conn
	IsReady    bool
	Field      [][]int
}

// Room represents a game room with a unique code and players.
type Room struct {
	Code    string
	Players []Player
}

var rooms map[string]*Room

func GenerateRoomCode() string {
	bytes := make([]byte, roomCodeLength)
	_, err := rand.Read(bytes)
	if err != nil {
		log.Fatal("Error generating random bytes:", err)
	}

	return base64.URLEncoding.EncodeToString(bytes)[:roomCodeLength]
}

func CreateRoomHandler(w http.ResponseWriter, r *http.Request) {
	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println(err)

		err := conn.Close()
		if err != nil {
			log.Println(err)
		}

		return
	}

	code := GenerateRoomCode()
	room := &Room{
		Code:    code,
		Players: make([]Player, 0),
	}
	newPlayer := Player{conn, false, make([][]int, 0)}
	room.Players = append(room.Players, newPlayer)
	rooms[code] = room

	log.Println("Room created with code:", code)

	message := EncodeMessage("code", "server", code)
	err = conn.WriteMessage(websocket.TextMessage, message)
	if err != nil {
		log.Println(err)
	}

	go room.HandlePlayerMessages(conn)

	go room.Cleanup()
}

func JoinRoomHandler(w http.ResponseWriter, r *http.Request) {
	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println(err)

		err := conn.Close()
		if err != nil {
			log.Println(err)
		}

		return
	}

	code := r.URL.Query().Get("code")
	room, ok := rooms[code]
	if !ok {
		message := EncodeMessage("error", "server", "No such code!")
		err = conn.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			log.Println(err)
		}

		err := conn.Close()
		if err != nil {
			log.Println(err)
		}

		return
	}

	if len(room.Players) >= 2 {
		message := EncodeMessage("error", "server", "Room is full!")
		err = conn.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			log.Println(err)
		}

		err := conn.Close()
		if err != nil {
			log.Println(err)
		}

		return
	}

	message := EncodeMessage("code", "server", code)
	err = conn.WriteMessage(websocket.TextMessage, message)
	if err != nil {
		log.Println(err)
	}

	newPlayer := Player{conn, false, make([][]int, 0)}
	room.Players = append(room.Players, newPlayer)
	log.Println("Player joined room with code:", code)

	go room.HandlePlayerMessages(conn)
}

func (room *Room) Cleanup() {
	ticker := time.Tick(time.Second)
	for range ticker {
		var remainingPlayers []Player

		for _, player := range room.Players {
			if player.Connection != nil {
				remainingPlayers = append(remainingPlayers, player)
			} else {
				log.Println("Player disconnected and removed from room:", room.Code)
			}
		}

		room.Players = remainingPlayers

		if len(room.Players) == 0 {
			log.Println("Cleaning up room with code:", room.Code)
			delete(rooms, room.Code)
			break
		}
	}
}
