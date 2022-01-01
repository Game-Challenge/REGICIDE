package tserver

import (
	"Regicide/GameProto"
	"math/rand"
	"time"
)

//Cards
func (room *Room) InitCards() {
	for i := 0; i < 54; i++ {
		cardData := InstanceCardData(i)
		ToTalCardList = append(ToTalCardList, &cardData)
	}
}

func (room *Room) InitMyCards() {
	for i := 0; i < len(ToTalCardList); i++ {
		cardData := ToTalCardList[i]
		if cardData.IsBoss {
			BossList = append(BossList, cardData)
		} else {
			MyCardList = append(MyCardList, cardData)
		}
	}
	RandomSort(MyCardList)
}

func (room *Room) TurnCards(client *Client) {
	turnCount := int(9 - room.RoomPack.Curnum)
	if turnCount == 0 {
		return
	}
	if turnCount > len(MyCardList) {
		turnCount = len(MyCardList)
		return
	}

	turnCardList := []*CardData{}

	for i := 0; i < turnCount; i++ {
		cardData := MyCardList[i]
		turnCardList = append(turnCardList, cardData)
		// for j := 0; j < len(MyCardList); j++ {
		// 	if MyCardList[j] == cardData {
		// 		continue
		// 	}
		// 	v := MyCardList[j]
		// 	MyCardList = append(MyCardList, v)
		// }
	}

	currentCards := []*GameProto.CardData{}
	for i := 0; i < len(turnCardList); i++ {
		cardData := &GameProto.CardData{CardInt: int32(turnCardList[i].CardInt)}
		currentCards = append(currentCards, cardData)
	}

	client.Actor.CuttrntCards = currentCards
}

func RandomSort(cardDatas []*CardData) {
	r := rand.New(rand.NewSource(time.Now().UnixNano()))
	count := len(cardDatas)
	for i := 0; i < count; i++ {
		index := r.Intn(count - 1)
		temp := cardDatas[i]
		cardDatas[i] = cardDatas[index]
		cardDatas[index] = temp
	}
}

var ToTalCardList []*CardData
var MyCardList []*CardData
var BossList []*CardData

type CardData struct {
	CardInt   int
	CardValue int
	CardType  int
	IsBoss    bool
	IsJoker   bool
	IsPet     bool
}

func InstanceCardData(cardInt int) CardData {

	cardValue := 0
	if cardInt == 52 || cardInt == 53 {
		cardValue = 0
	} else {
		cardValue = (cardInt % 13) + 1
	}
	cardType := 0
	if cardValue == 0 {
		cardType = 5
	} else {
		cardType = ((cardInt) / 13) + 1
	}

	isJoker := cardType == 5
	isBoss := cardValue > 10 && !isJoker
	isPet := cardValue == 1

	cardData := CardData{CardValue: cardValue, CardInt: cardInt, CardType: cardType, IsJoker: isJoker, IsBoss: isBoss, IsPet: isPet}
	return cardData
}
