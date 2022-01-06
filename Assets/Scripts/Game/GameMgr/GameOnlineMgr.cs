﻿using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using RegicideProtocol;
using UnityEngine;
using CardData = RegicideProtocol.CardData;

class GameOnlineMgr:DataCenterModule<GameOnlineMgr>
{
    private GAMESTATE gamestate;

    public void SetGameSate(GAMESTATE state)
    {
        Gamestate = state;
    }
    public GAMESTATE Gamestate
    {
        private set
        {
            gamestate = value;
            UISys.ShowTipMsg("当前阶段:" + GameMgr.Instance.GetCurrentStateStr(gamestate));
            EventCenter.Instance.EventTrigger("UpdateGameState");
        }
        get
        {
            return gamestate;
        }
    } //当前状态

    public int LeftCardCount;
    public int MyGameIndex { private set; get; }//我是几号,第几个出牌的
    public int PlayerNum { private set; get; }  //玩家数目
    public int GameId { private set; get; }     //游戏ID
    public int CurrentGameIndex { private set; get; }  //当前是玩家几号
    public string MyName { private set; get; }  //我的用户名
    public int MyActorId { private set; get; }  //我的唯一编号

    public bool IsOnlineGameIng;

    public List<ActorPack> ActorPacks = new List<ActorPack>();
    public BossActor BossActor { private set; get; }

    public Dictionary<int, List<global::CardData>> CardDictionary = new Dictionary<int, List<global::CardData>>();

    public List<RegicideProtocol.CardData> CurrentUsedCardDatas = new List<RegicideProtocol.CardData>();

    public List<RegicideProtocol.CardData> MuDiCardDatas = new List<RegicideProtocol.CardData>();

    public void InitMyData(MainPack mainPack)
    {
        int result;

        int.TryParse(mainPack.Str, out result);

        MyActorId = result;

        MyName = mainPack.LoginPack.Username;

        Debug.Log("初始化成功："+MyActorId);
    }

    #region 协议
    public override void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.Attack, AttackRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Skill, SkillRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Damage, DamageRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Hurt, AbordRes);
    }

    public int CurrentBossIndex = 0;
    public void InitGame(MainPack mainPack)
    {
        CurrentBossIndex = 0;

        var roomPack = mainPack.Roompack[0];

        PlayerNum = roomPack.ActorPack.Count;

        Gamestate = roomPack.Gamestate.State;

        //SetState(roomPack.Gamestate.State);

        CurrentGameIndex = roomPack.CurrentIndex;

        ActorPacks = roomPack.ActorPack.ToList();

        var bossActorPack = roomPack.BossActor;

        BossActor = ActorMgr.Instance.InstanceBossActor(bossActorPack.ActorId);

        LeftCardCount = roomPack.LeftCardCount;

        InitCards();
    }

    private void InitCards()
    {
        CardDictionary.Clear();

        var players = ActorPacks;

        if (players.Count > 4)
        {
            return;
        }


        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ActorId == GameOnlineMgr.Instance.MyActorId)
            {
                MyGameIndex = i;//第几个出牌的
            }

            var temp = new List<global::CardData>();

            foreach (var card in players[i].CuttrntCards)
            {
                var cardData = CardMgr.Instance.InstanceData(card.CardInt);
                temp.Add(cardData);
            }

            if (!CardDictionary.ContainsKey(players[i].ActorId))
            {
                CardDictionary.Add(players[i].ActorId,temp);
            }
        }
    }

    public void RefreshCardDataByActorId(int actorId, RepeatedField<RegicideProtocol.CardData> cardDatas)
    {
        if (CardDictionary.ContainsKey(actorId))
        {
            var list = CardDictionary[actorId];
            list.Clear();
            foreach (var card in cardDatas)
            {
                var cardData = CardMgr.Instance.InstanceData(card.CardInt);
                list.Add(cardData);
            }
        }
    }

    public void RefreshCardDataByActorId(int actorId, List<RegicideProtocol.CardData> cardDatas)
    {
        if (CardDictionary.ContainsKey(actorId))
        {
            var list = CardDictionary[actorId];
            list.Clear();
            for (int i = 0; i < cardDatas.Count; i++)
            {
                var cardData = CardMgr.Instance.InstanceData(cardDatas[i].CardInt);
                list.Add(cardData);
            }
        }
    }

    public List<global::CardData> GetCardDataByActorId(int actorId)
    {
        List<global::CardData> list;

        if (CardDictionary.TryGetValue(actorId, out list))
        {
            return list;
        }
        return list;
    }

    public List<global::CardData> GetMyCardData()
    {
        var actorId = MyActorId;

        return GetCardDataByActorId(actorId);
    }

    public void AbordReq()
    {
        if (CurrentGameIndex != MyGameIndex)
        {
            UISys.ShowTipMsg("当前阶段不是您出牌");
            return;
        }
        if (Gamestate != GAMESTATE.State4)
        {
            UISys.ShowTipMsg("当前阶段:" + GameMgr.Instance.GetCurrentStateStr(Gamestate));
            return;
        }

        List<global::CardData> list = GameMgr.Instance.m_choiceList;

        var point = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var card = list[i];
            if (card.cardType == CardType.BLACK_JOKER || card.cardType == CardType.RED_JOKER)
            {
                continue;
            }

            if (card.CardValue == 11)
            {
                point += 10;
            }else if (card.CardValue == 12)
            {
                point += 15;
            }
            else if (card.CardValue == 13)
            {
                point += 20;
            }
            else
            {
                point += list[i].CardValue;
            }
        }

        if (point < BossActor.Atk)
        {
            UISys.ShowTipMsg(string.Format("弃牌点数还差{0}点",BossActor.Atk - point));
            return;
        }

        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Game, ActionCode.Hurt);
        RoomPack roomPack = new RoomPack();
        ActorPack actorPack = new ActorPack();

        for (int i = 0; i < list.Count; i++)
        {
            RegicideProtocol.CardData cardData = InstanceCardData(list[i]);
            actorPack.CuttrntCards.Add(cardData);
        }
        roomPack.ActorPack.Add(actorPack);
        mainPack.Roompack.Add(roomPack);

        GameClient.Instance.SendCSMsg(mainPack);
    }

    public RegicideProtocol.CardData InstanceCardData(global::CardData cardData)
    {
        RegicideProtocol.CardData protoCcardData = new RegicideProtocol.CardData();
        protoCcardData.CardInt = cardData.CardInt;
        protoCcardData.CardType = (RegicideProtocol.CardType)cardData.cardType;
        protoCcardData.CardValue = cardData.CardValue;

        return protoCcardData;
    }

    private void AbordRes(MainPack mainPack)
    {
        GameMgr.Instance.m_choiceList.Clear();

        var roomPack = mainPack.Roompack[0];
        CurrentGameIndex = roomPack.CurrentIndex;

        var playerPack = mainPack.Roompack[0].ActorPack;
        foreach (var player in playerPack)
        {
            RefreshCardDataByActorId(player.ActorId, player.CuttrntCards);
        }

        LeftCardCount = roomPack.LeftCardCount;
        SetMuDiUsedCards(roomPack.MuDiCards);
        //SetCurrentUsedCards(roomPack.CurrentUseCards);
        UISys.Mgr.ShowWindow<GameOnlineTips>(UI_Layer.Top).ShowTip("玩家"+ playerPack[CurrentGameIndex].ActorName+"遗弃卡牌", roomPack.CurrentUseCards);

        Gamestate = roomPack.Gamestate.State;
        BossActor.Refresh(roomPack.BossActor);
        EventCenter.Instance.EventTrigger("RefreshGameUI");

        UISys.ShowTipMsg(string.Format("当前{0}号玩家{1}准备攻击，请选牌！", CurrentGameIndex, playerPack[CurrentGameIndex].ActorName));
    }

    private void DamageRes(MainPack mainPack)
    {
        
    }

    private void SkillRes(MainPack mainPack)
    {
        
    }

    #region Attack
    public void AttackReq(bool choiceIndex = false,int index = 0)
    {
        if (choiceIndex)
        {
            MainPack mainPack_ = ProtoUtil.BuildMainPack(RequestCode.Game, ActionCode.Attack);
            mainPack_.Str = "JKR";
            mainPack_.User = index.ToString();
            GameClient.Instance.SendCSMsg(mainPack_);
        }

        if (CurrentGameIndex!=MyGameIndex)
        {
            UISys.ShowTipMsg("当前阶段不是您出牌");
            Debug.Log(CurrentGameIndex);
            Debug.Log(MyGameIndex.ToString());
            return;
        }
        if (Gamestate != GAMESTATE.State1)
        {
            UISys.ShowTipMsg("当前阶段:" + GameMgr.Instance.GetCurrentStateStr(Gamestate));
            return;
        }
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Game, ActionCode.Attack);

        List<global::CardData> list = GameMgr.Instance.m_choiceList;

        if (list.Count == 0)
        {
            UISys.ShowTipMsg(string.Format("玩家{0}跳过出牌",ActorPacks[CurrentGameIndex].ActorName));
        }

        if (!GameMgr.Instance.CheckCardInvild(list))
        {
            UISys.ShowTipMsg("您选择的卡片不符合规定");
            return;
        }

        RoomPack roomPack = new RoomPack();
        ActorPack actorPack = new ActorPack();

        for (int i = 0; i < list.Count; i++)
        {
            RegicideProtocol.CardData cardData = InstanceCardData(list[i]);
            actorPack.CuttrntCards.Add(cardData);
        }
        roomPack.ActorPack.Add(actorPack);
        mainPack.Roompack.Add(roomPack);

        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void AttackRes(MainPack mainPack)
    {
        GameMgr.Instance.m_choiceList.Clear();

        if (ChekJoker(mainPack))
        {
            return;
        }

        var roomPack = mainPack.Roompack[0];

        LeftCardCount = roomPack.LeftCardCount;

        var playerPack = mainPack.Roompack[0].ActorPack;

        //SetCurrentUsedCards(roomPack.CurrentUseCards);
        UISys.Mgr.ShowWindow<GameOnlineTips>(UI_Layer.Top).ShowTip("玩家" + playerPack[CurrentGameIndex].ActorName + "打出卡牌", roomPack.CurrentUseCards);

        CurrentGameIndex = roomPack.CurrentIndex;


        if (mainPack.Str.Equals("GAMELOSE"))
        {
            UISys.Mgr.ShowWindow<GameLoseUI>().InitUI("游戏失败，"+ playerPack[CurrentGameIndex].ActorName + "的牌不足以抵挡本次攻击！！");
        }
       
        foreach (var player in playerPack)
        {
            RefreshCardDataByActorId(player.ActorId,player.CuttrntCards);
        }

        Gamestate = roomPack.Gamestate.State;
        bool bossDie = BossActor.Refresh(roomPack.BossActor);
        EventCenter.Instance.EventTrigger("RefreshGameUI");
        if (bossDie&&CurrentBossIndex>=13)
        {
            UISys.Mgr.ShowWindow<GameWinUI>();
            return;
        }
        if (bossDie)
        {
            CurrentBossBeJokerAtk = false;
            UISys.ShowTipMsg(string.Format("当前{0}号玩家{1}击败boss，直接进入阶段一", CurrentGameIndex, playerPack[CurrentGameIndex].ActorName));
        }
        else
        {
            UISys.ShowTipMsg(string.Format("当前{0}号玩家{1}攻击结束，请弃点数{2}的牌！", CurrentGameIndex, playerPack[CurrentGameIndex].ActorName, BossActor.Atk));
        }
    }

    public bool CurrentBossBeJokerAtk;
    private bool ChekJoker(MainPack mainPack)
    {
        if (mainPack.Str.Equals("JKR")) //打出了Joker
        {
            var playerPack = mainPack.Roompack[0].ActorPack;

            UISys.ShowTipMsg(string.Format("当前{0}号玩家{1}打出了Joker，BOSS技能失效，请选择下一个出牌的玩家！", CurrentGameIndex, playerPack[CurrentGameIndex].ActorName));

            CurrentBossBeJokerAtk = true;

            Gamestate = GAMESTATE.State1;
            if (CurrentGameIndex == MyGameIndex)
            {
                GameMgr.Instance.m_choiceList.Clear();
                UISys.Mgr.ShowWindow<JokerChoiceUI>();
                return true;
            }
        }
        else if (mainPack.Str.Equals("JKRCH")) 
        {
            var roomPack = mainPack.Roompack[0];

            var playerPack = roomPack.ActorPack;

            UISys.ShowTipMsg(string.Format("当前{0}号玩家{1}选择了{2}号玩家{3}出牌！", CurrentGameIndex, playerPack[CurrentGameIndex].ActorName, roomPack.CurrentIndex, playerPack[roomPack.CurrentIndex].ActorName));

            CurrentGameIndex = roomPack.CurrentIndex;

            Gamestate = GAMESTATE.State1;

            return true;
        }

        return false;
    }
    #endregion

    #endregion

    #region CurrentCards

    public void SetCurrentUsedCards(RepeatedField<RegicideProtocol.CardData> list)
    {
        CurrentUsedCardDatas.Clear();

        if (list == null ||list.Count<0)
        {
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            CurrentUsedCardDatas.Add(list[i]);
        }
    }

    public void SetMuDiUsedCards(RepeatedField<RegicideProtocol.CardData> list)
    {
        MuDiCardDatas.Clear();

        if (list == null || list.Count < 0)
        {
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            MuDiCardDatas.Add(list[i]);
        }
    }


    #endregion
}