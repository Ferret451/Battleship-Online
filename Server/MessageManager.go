package main

import (
	"encoding/json"
	"github.com/gorilla/websocket"
	"log"
)

type Message struct {
	Type     string `json:"Type"`
	SentFrom string `json:"SentFrom"`
	Info     string `json:"Info"`
}

func (room *Room) HandlePlayerMessages(conn *websocket.Conn) {
	for {
		_, messageByte, err := conn.ReadMessage()
		if err != nil {
			log.Println(err)

			var index = room.FindPlayer(conn)
			room.Players[index].Connection = nil

			err := conn.Close()
			if err != nil {
				log.Println(err)
			}

			break
		}

		message, err := DecodeMessage(messageByte)
		if err == nil {
			//log.Println(message.SentFrom, "sent message of type |", message.Type, "| with message |", message.Info, "|")

			switch message.Type {
			case "ready":
				var index = room.FindPlayer(conn)
				if index >= 0 {
					room.Players[index].IsReady = true
					room.Players[index].Field = ReadField(message.Info)
					room.TryToStart()
				}

			case "notReady":
				var index = room.FindPlayer(conn)
				if index > 0 {
					room.Players[index].IsReady = false
					room.Players[index].Field = make([][]int, 0)
				}

			case "chat":
				room.SendToAll(message.Type, message.SentFrom, message.Info)

			case "shot":
				room.HandleShoot(room.FindPlayer(conn), message.Info)

			default:
				log.Println("Unknown message type:", message.Type)
			}
		}
	}
}

func DecodeMessage(jsonData []byte) (Message, error) {
	var message Message
	var err error = nil
	if err = json.Unmarshal(jsonData, &message); err != nil {
		log.Println("Error decoding JSON:", err)
		return message, err
	}

	return message, err
}

func EncodeMessage(_type string, sendFrom string, info string) []byte {
	var message = Message{_type, sendFrom, info}

	jsonData, err := json.Marshal(message)
	if err != nil {
		log.Println("Error encoding JSON:", err)
	}

	return jsonData
}

func (room *Room) SendToAll(_type string, sendFrom string, info string) {
	for i := range room.Players {
		room.SendToPlayer(i, _type, sendFrom, info)
	}
}

func (room *Room) SendToPlayer(index int, _type string, sendFrom string, info string) {
	message := EncodeMessage(_type, sendFrom, info)

	err := room.Players[index].Connection.WriteMessage(websocket.BinaryMessage, message)
	if err != nil {
		log.Println(err)
	}
}
