package main

import (
	"log"
	"net/http"
)

const (
	roomCodeLength  = 6
	fieldRowsAmount = 10
	fieldColsAmount = 10
)

func main() {
	rooms = make(map[string]*Room)

	http.HandleFunc("/createRoom", CreateRoomHandler)
	http.HandleFunc("/joinRoom", JoinRoomHandler)
	log.Fatal(http.ListenAndServe(":8080", nil))
}
