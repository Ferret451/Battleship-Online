package main

import (
	"log"
	"net/http"
)

func main() {
	rooms = make(map[string]*Room)
	http.HandleFunc("/createRoom", createRoomHandler)
	http.HandleFunc("/joinRoom", joinRoomHandler)
	log.Fatal(http.ListenAndServe(":8080", nil))
}
