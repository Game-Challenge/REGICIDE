package tserver

import (
	GameProto "Regicide/GameProto"
	"errors"
	"math/rand"
	"time"

	"github.com/wonderivan/logger"
)

const (
	// TOTAL_CARD_COUNT = 54
	TOTAL_BOSS_COUNT = 12
)

var TOTAL_CARD_COUNT int

var CURRENT_BOSS_INDEX int

var CURRENT_MAX_TURN_COUNT int

//InitCards 初始化所有卡牌
func (room *Room) InitCards() {
	// clientCount := len(room.ClientList)
	// if clientCount <= 2 {
	// 	TOTAL_CARD_COUNT = 52
	// } else if clientCount == 3 {
	// 	TOTAL_CARD_COUNT = 53
	// } else if clientCount == 4 {
	// 	TOTAL_CARD_COUNT = 54
	// }

	TOTAL_CARD_COUNT = 54
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
	room.RoomPack.LeftCardCount = int32(len(MyCardList))
}

func (room *Room) TurnCards(client *Client) {
	turnCount := int(9 - room.RoomPack.Curnum)
	CURRENT_MAX_TURN_COUNT = turnCount
	if turnCount == 0 {
		return
	}

	mycardCount := len(MyCardList)
	if turnCount > mycardCount {
		turnCount = mycardCount
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

		cardProtoData := &GameProto.CardData{CardInt: int32(cardData.CardInt), CardValue: int32(cardData.CardValue)}
		currentCards = append(currentCards, cardProtoData)
	}

	client.Actor.CuttrntCards = currentCards
	room.RoomPack.LeftCardCount = int32(len(MyCardList))
}

//红桃
func (client *Client) AddHp(number int) {
	count := len(UsedCardList)
	if count == 0 {
		return
	}
	if number > count {
		number = count
	}
	for i := 0; i < number; i++ {
		cardData := InstanceCardData(int(UsedCardList[i].CardInt))
		MyCardList = append(MyCardList, &cardData)
	}
	for i := 0; i < number; i++ {
		if len(UsedCardList) <= 0 {
			break
		}
		cardData := UsedCardList[0]
		UsedCardList = RemoveCardData(UsedCardList, cardData)
	}
	RandomSort(MyCardList)
	client.RoomInfo.RoomPack.LeftCardCount = int32(len(MyCardList))
}

// 方块抽卡
func (client *Client) TurnCardDiamond(number int) {
	logger.Debug("TurnCardDiamond number=>", number)
	room := client.RoomInfo
	turnCount := number
	if turnCount == 0 {
		return
	}

	mycardCount := len(MyCardList)
	if turnCount > mycardCount {
		turnCount = mycardCount
	}

	myIndex := 0
	clientCount := len(room.ClientList)
	for i := 0; i < len(room.ClientList); i++ {
		if room.ClientList[i] == client {
			myIndex = i
			break
		}
	}
	i := 0
	index := myIndex
	continueCount := 0
	for {
		// logger.Debug(i, index, continueCount)
		if i > turnCount || continueCount > turnCount {
			break
		}
		if index >= clientCount {
			index = 0
		}
		clientCardCount := len(room.ClientList[index].Actor.CuttrntCards)
		if clientCardCount >= CURRENT_MAX_TURN_COUNT {
			index++
			continueCount++
			continue
		}
		cardData := MyCardList[i]
		MyCardList = RemoveCard(MyCardList, cardData)
		cardProtoData := &GameProto.CardData{CardInt: int32(cardData.CardInt), CardValue: int32(cardData.CardValue)}
		room.ClientList[index].Actor.CuttrntCards = append(room.ClientList[index].Actor.CuttrntCards, cardProtoData)
		i++
		index++
		if i > turnCount {
			break
		}
	}
	client.RoomInfo.RoomPack.LeftCardCount = int32(len(MyCardList))
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
var UsedCardList []*GameProto.CardData          //弃牌堆
var CurrentAttackCardList []*GameProto.CardData //boss未死亡的牌堆
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
	CURRENT_BOSS_INDEX++

	count := len(BossList)

	var bossPower int
	if CURRENT_BOSS_INDEX <= 4 {
		bossPower = 11
	} else if CURRENT_BOSS_INDEX <= 8 {
		bossPower = 12
	} else if CURRENT_BOSS_INDEX <= 12 {
		bossPower = 13
	}

	var cacheList []*CardData
	for i := 0; i < count; i++ {
		if BossList[i].CardValue == bossPower {
			cacheList = append(cacheList, BossList[i])
		}
	}

	r := rand.New(rand.NewSource(time.Now().UnixNano()))
	index := r.Intn(len(cacheList))

	var atk int32
	var hp int32

	cardData := cacheList[index]
	BossList = RemoveCard(BossList, cardData)

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
		return false
	}

	bossType := bossActor.ActorJob

	coubldDouble := ((bossType != 1) || CurrentBossBeJokerAtk) && attackData.CouldDoubleAtk

	CurrentBossType = GameProto.CardType(bossType)

	if attackData.CouldDownBossAtk && ((CurrentBossType != GameProto.CardType_SPADE) || CurrentBossBeJokerAtk) {
		bossActor.ATK -= attackData.Damage
		if bossActor.ATK <= 0 {
			bossActor.ATK = 0
		}
	}
	if attackData.CouldAddHp && (CurrentBossType != GameProto.CardType_HEART) {
		client.AddHp(int(attackData.Damage))
	}

	if attackData.CouldTurnCard && (CurrentBossType != GameProto.CardType_DIAMOND) {
		client.TurnCardDiamond(int(attackData.Damage))
	}

	if coubldDouble {
		bossActor.Hp -= 2 * attackData.Damage
	} else {
		bossActor.Hp -= attackData.Damage
	}

	// if bossActor.Hp < 0 {
	// 	cardData := InstanceCardData(int(bossActor.ActorId))
	// 	MyCardList = append(MyCardList, &cardData)
	// } else if bossActor.Hp == 0 {
	// 	cardData := InstanceCardData(int(bossActor.ActorId))
	// 	MyCardList = append(MyCardList, &cardData)

	// 	cache := []*CardData{&cardData}
	// 	temp := append(cache, MyCardList...)
	// 	MyCardList = temp
	// }

	bossDie := bossActor.Hp <= 0
	if bossActor.Hp <= 0 {
		bossActor.Hp = 0
		for i := 0; i < len(CurrentAttackCardList); i++ {
			UsedCardList = append(UsedCardList, CurrentAttackCardList[i])
			client.RoomInfo.RoomPack.MuDiCards = UsedCardList
		}
		CurrentAttackCardList = CurrentAttackCardList[0:0]
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
			Damage += card.CardValue
		} else {
			if card.CardValue == 11 {
				Damage += 10
			} else if card.CardValue == 12 {
				Damage += 15
			} else if card.CardValue == 13 {
				Damage += 20
			} else {
				Damage += card.CardValue
			}
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
		} else if int(card.CardType) == 6 {
			HadJoker = true
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
