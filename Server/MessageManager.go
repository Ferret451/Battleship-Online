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

func (room *Room) handlePlayerMessages(conn *websocket.Conn) {
	defer conn.Close()
	for {
		_, messageByte, err := conn.ReadMessage()
		if err != nil {
			log.Println(err)
			break
		}

		message, err := handleMessage(messageByte)
		if err == nil {
			//log.Println(message.SentFrom, "sent message of type |", message.Type, "| with message |", message.Info, "|")

			switch message.Type {
			case "close":
				log.Println("Player", message.SentFrom, "disconnected from room", message.Info)

				//Deleting player
				var result []Player
				for _, v := range room.Players {
					if v.Connection != conn {
						result = append(result, v)
					}
				}
				room.Players = result

				break
			case "shot":
				log.Println("Shot fired by:", message.SentFrom)

			case "chat":
				room.sendToAll(message.Type, message.SentFrom, message.Info)

			default:
				log.Println("Unknown message type:", message.Type)
			}
		}
	}
}

func handleMessage(jsonData []byte) (Message, error) {
	var message Message
	var err error = nil
	if err = json.Unmarshal(jsonData, &message); err != nil {
		log.Println("Error decoding JSON:", err)
		return message, err
	}

	return message, err
}

func createMessage(_type string, sendFrom string, info string) []byte {
	var message Message = Message{_type, sendFrom, info}

	jsonData, err := json.Marshal(message)
	if err != nil {
		log.Println("Error encoding JSON:", err)
	}

	return jsonData
}
