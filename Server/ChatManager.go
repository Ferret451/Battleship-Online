package main

import (
	"github.com/gorilla/websocket"
	"log"
)

func (room *Room) sendToAll(_type string, sendFrom string, info string) {
	for _, v := range room.Players {
		message := createMessage(_type, sendFrom, info)

		err := v.Connection.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			log.Println(err)
		}
		//} else {
		//	log.Println("Sent message from", sendFrom, "with message |", info, "|")
		//}
	}
}
