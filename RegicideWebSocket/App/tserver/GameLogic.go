package tserver

import (
	GameProto "Regicide/GameProto"
	"errors"
	"math/rand"
	"time"

	"github.com/wonderivan/logger"
)

const (
	TOTAL_CARD_COUNT = 54
	TOTAL_BOSS_COUNT = 12
)

var CURRENT_BOSS_INDEX int

//InitCards 初始化所有卡牌
func (room *Room) InitCards() {
	for i := 0; i < TOTAL_CARD_COUNT; i++ {
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
	}

	turnCardList := []*CardData{}

	for i := 0; i < turnCount; i++ {
		cardData := MyCardList[i]
		turnCardList = append(turnCardList, cardData)
	}

	currentCards := []*GameProto.CardData{}

	for i := 0; i < turnCount; i++ {
		cardData := turnCardList[i]
		MyCardList = RemoveCard(MyCardList, cardData)

		cardProtoData := &GameProto.CardData{CardInt: int32(turnCardList[i].CardInt)}
		currentCards = append(currentCards, cardProtoData)
	}

	client.Actor.CuttrntCards = currentCards
}

//todo
func (client *Client) TurnCardDiamond(number int) {
	room := client.RoomInfo
	turnCount := number
	if turnCount == 0 {
		return
	}

	if turnCount > int(9-room.RoomPack.Curnum) {
		return
	}

	if turnCount > len(MyCardList) {
		turnCount = len(MyCardList)
	}

	return
	// // todo
	// index := 0
	// for i := 0; i < len(client.RoomInfo.ClientList); i++ {
	// 	if client.RoomInfo.ClientList[i] == client {
	// 		index = i
	// 	}
	// }

	turnCardList := []*CardData{}

	for i := 0; i < turnCount; i++ {
		cardData := MyCardList[i]
		turnCardList = append(turnCardList, cardData)
	}

	currentCards := []*GameProto.CardData{}

	for i := 0; i < turnCount; i++ {
		cardData := turnCardList[i]
		MyCardList = RemoveCard(MyCardList, cardData)

		cardProtoData := &GameProto.CardData{CardInt: int32(turnCardList[i].CardInt)}
		currentCards = append(currentCards, cardProtoData)
	}
	// client.Actor.CuttrntCards = currentCards
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
var UsedCardList []*CardData
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

//初始化这张卡的卡片数据
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

//InitBoss
func (room *Room) InitBoss() *GameProto.ActorPack {
	r := rand.New(rand.NewSource(time.Now().UnixNano()))
	count := len(BossList)
	index := r.Intn(count - 1)

	var atk int32
	var hp int32

	cardData := BossList[index]
	if cardData.CardValue == 11 {
		atk = 10
		hp = 20
	} else if cardData.CardValue == 12 {
		atk = 15
		hp = 25
	} else if cardData.CardValue == 13 {
		atk = 20
		hp = 30
	} else {
		atk = 30
		hp = 50
	}
	room.RoomPack.Gamestate.State = GameProto.GAMESTATE_STATE1
	CURRENT_BOSS_INDEX++
	CurrentBossBeJokerAtk = false
	bossActor := &GameProto.ActorPack{ATK: atk, Hp: hp, ActorId: int32(cardData.CardInt), Index: int32(CURRENT_BOSS_INDEX), ActorJob: int32(((cardData.CardInt) / 13) + 1)}
	room.RoomPack.BossActor = bossActor
	return bossActor
}

func (client *Client) AttackBoss(cardData []*GameProto.CardData) (bool, error) {
	if client == nil {
		return false, errors.New("client is nil")
	}
	room := client.RoomInfo
	if room == nil {
		return false, errors.New("room is nil")
	}
	if room.RoomPack == nil {
		return false, errors.New("room.RoomPack is nil")
	}
	if room.RoomPack.BossActor == nil {
		return false, errors.New("room.RoomPack.BossActor is nil")
	}
	if room.RoomPack.Gamestate.State != GameProto.GAMESTATE_STATE1 {
		return false, errors.New("is not GameProto.GAMESTATE_STATE1")
	}

	bossActor := room.RoomPack.BossActor

	attackData := GenAttackData(cardData)

	logger.Debug("attackData:", attackData)

	return ImpactSkill(client, bossActor, attackData), nil
}

var CurrentBossType GameProto.CardType

var CurrentBossBeJokerAtk bool

func ImpactSkill(client *Client, bossActor *GameProto.ActorPack, attackData AttackData) bool {
	if attackData.HadJoker {
		CurrentBossBeJokerAtk = true
	}

	bossType := bossActor.ActorJob

	coubldDouble := ((bossType != 1) || CurrentBossBeJokerAtk) && attackData.CouldDoubleAtk

	CurrentBossType = GameProto.CardType(bossType)

	if attackData.CouldDownBossAtk && ((CurrentBossType != GameProto.CardType_SPADE) || CurrentBossBeJokerAtk) {
		bossActor.ATK -= attackData.Damage
	}
	if attackData.CouldTurnCard && (CurrentBossType != GameProto.CardType_DIAMOND) {
		client.TurnCardDiamond(int(attackData.Damage))
	}
	if attackData.CouldAddHp && (CurrentBossType != GameProto.CardType_HEART) {

	}

	if coubldDouble {
		bossActor.Hp -= 2 * attackData.Damage
	} else {
		bossActor.Hp -= attackData.Damage
	}
	bossDie := bossActor.Hp <= 0
	if bossActor.Hp <= 0 {
		bossActor.Hp = 0
		client.RoomInfo.InitBoss()
	}
	logger.Debug("bossActor:", bossActor)
	return bossDie
}

func GenAttackData(cardData []*GameProto.CardData) AttackData {
	var CouldTurnCard bool
	var CouldDoubleAtk bool
	var CouldDownBossAtk bool
	var CouldAddHp bool
	var Damage int32
	var HadPet bool
	var HadJoker bool
	for i := 0; i < len(cardData); i++ {
		card := cardData[i]
		//is pet
		if card.CardValue == 1 {
			HadPet = true
		} else {
			Damage += card.CardValue
		}

		if card.CardType == GameProto.CardType_DIAMOND {
			CouldTurnCard = true
		} else if card.CardType == GameProto.CardType_CLUB {
			CouldDoubleAtk = true
		} else if card.CardType == GameProto.CardType_SPADE {
			CouldDownBossAtk = true
		} else if card.CardType == GameProto.CardType_HEART {
			CouldAddHp = true
		} else if card.CardType == GameProto.CardType_JOKER {
			HadJoker = true
		}

		if HadPet {
			Damage += 1
		}
	}

	attackData := AttackData{
		CouldTurnCard:    CouldTurnCard,
		CouldDoubleAtk:   CouldDoubleAtk,
		CouldDownBossAtk: CouldDownBossAtk,
		CouldAddHp:       CouldAddHp, Damage: Damage,
		HadPet: HadPet, HadJoker: HadJoker}
	return attackData
}

type AttackData struct {
	CouldTurnCard    bool
	CouldDoubleAtk   bool
	CouldDownBossAtk bool
	CouldAddHp       bool
	Damage           int32
	HadPet           bool
	HadJoker         bool
}
