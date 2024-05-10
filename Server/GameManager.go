package main

import (
	"fmt"
	"github.com/gorilla/websocket"
	"log"
	"strconv"
	"strings"
)

func (room *Room) FindPlayer(conn *websocket.Conn) int {
	for i, v := range room.Players {
		if v.Connection == conn {
			return i
		}
	}

	return -1
}

func ReadField(strField string) [][]int {
	field := make([][]int, fieldRowsAmount)
	for i := range field {
		field[i] = make([]int, fieldColsAmount)
	}

	strIndex := 0
	for i := range field {
		for j := range field[i] {
			if strIndex >= len(strField) {
				break
			}
			field[i][j] = int(strField[strIndex]) - 48
			strIndex++
		}
	}

	return field
}

func WriteField(field [][]int) string {
	str := ""
	for i := range field {
		for j := range field[i] {
			str += strconv.Itoa(field[i][j])
		}
	}

	return str
}

func TransformField(field [][]int) [][]int {
	newField := make([][]int, fieldRowsAmount)
	for i := range field {
		newField[i] = make([]int, fieldColsAmount)
	}

	for i := range field {
		for j := range field[i] {
			if field[i][j] == 3 || field[i][j] == 4 || field[i][j] == 5 {
				newField[i][j] = field[i][j]
			} else {
				newField[i][j] = 0
			}
		}
	}

	return newField
}

func (room *Room) TryToStart() {
	if len(room.Players) == 2 {
		for _, v := range room.Players {
			if v.IsReady == false {
				return
			}
		}

		room.StartGame()
	}
}

func (room *Room) StartGame() {
	room.SendToAll("start", "server", "")
	room.ChangeTurnTo(0)
}

func (room *Room) ChangeTurnTo(index int) {
	room.SendToPlayer(index, "goShoot", "server", "")
	room.SendToPlayer((index+1)%2, "wait", "server", "")
}

func (room *Room) HandleShoot(shootFromIndex int, coords string) {
	var shootToIndex = (shootFromIndex + 1) % 2

	x, y, err := ExtractNumbers(coords)
	if err != nil {
		log.Println("Error:", err)
		return
	}

	if room.Players[shootToIndex].Field[x][y] == 1 {
		room.Players[shootToIndex].Field[x][y] = 4

		if isDestroyed(room.Players[shootToIndex].Field, x, y) {
			Recolor(&room.Players[shootToIndex].Field, x, y)
		}
	} else {
		room.Players[shootToIndex].Field[x][y] = 3
	}

	room.SendToPlayer(shootToIndex, "shotToYou", "server", WriteField(room.Players[shootToIndex].Field))

	transformedField := TransformField(room.Players[shootToIndex].Field)
	room.SendToPlayer(shootFromIndex, "shotFromYou", "server", WriteField(transformedField))

	room.ChangeTurnTo(shootToIndex)
}

func ExtractNumbers(str string) (int, int, error) {
	numbersStr := strings.Split(str, " ")
	if len(numbersStr) != 2 {
		return 0, 0, fmt.Errorf("wrong string format, expected two numbers divided by space")
	}

	num1, err := strconv.Atoi(numbersStr[0])
	if err != nil {
		return 0, 0, fmt.Errorf("error when converting first number: %v", err)
	}

	num2, err := strconv.Atoi(numbersStr[1])
	if err != nil {
		return 0, 0, fmt.Errorf("error when converting second number: %v", err)
	}

	return num1, num2, nil
}

func isDestroyed(field [][]int, x int, y int) bool {
	if checkDirection(field, x, y, -1, 0) &&
		checkDirection(field, x, y, 1, 0) &&
		checkDirection(field, x, y, 0, -1) &&
		checkDirection(field, x, y, 0, 1) {
		return true
	}

	return false
}

func checkDirection(field [][]int, x int, y int, addX int, addY int) bool {
	for x > 0 && x < fieldRowsAmount && y > 0 && y < fieldColsAmount && field[x][y] == 4 {
		x += addX
		y += addY
	}

	if x > 0 && x < fieldRowsAmount && y > 0 && y < fieldColsAmount && field[x][y] == 1 {
		return false
	}

	return true
}

func Recolor(fieldPtr *[][]int, x int, y int) {
	RecolorDirection(fieldPtr, x, y, -1, 0)
	RecolorDirection(fieldPtr, x, y, 1, 0)
	RecolorDirection(fieldPtr, x, y, 0, -1)
	RecolorDirection(fieldPtr, x, y, 0, 1)
}

func RecolorDirection(fieldPtr *[][]int, x int, y int, addX int, addY int) {
	var field = *fieldPtr

	for x > 0 && x < fieldRowsAmount && y > 0 && y < fieldColsAmount && field[x][y] == 4 {
		field[x][y] = 5

		x += addX
		y += addY
	}
}
