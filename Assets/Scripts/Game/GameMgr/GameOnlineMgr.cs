using System;
using System.Collections;
using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;
using CardData = RegicideProtocol.CardData;

class GameOnlineMgr:DataCenterModule<GameOnlineMgr>
{
    private GAMESTATE gamestate;

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

    public void InitGame(MainPack mainPack)
    {
        var roomPack = mainPack.Roompack[0];

        PlayerNum = roomPack.ActorPack.Count;

        Gamestate = roomPack.Gamestate.State;

        //SetState(roomPack.Gamestate.State);

        CurrentGameIndex = roomPack.CurrentIndex;

        ActorPacks = roomPack.ActorPack.ToList();

        var bossActorPack = roomPack.BossActor;

        BossActor = ActorMgr.Instance.InstanceBossActor(bossActorPack.ActorId);

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
            RefreshCardDataByActorId(player.ActorId, player.CuttrntCards.ToList());
        }

        Gamestate = roomPack.Gamestate.State;
        BossActor.Refresh(roomPack.BossActor);
        EventCenter.Instance.EventTrigger("RefreshGameUI");

        UISys.ShowTipMsg(string.Format("当前{0}号玩家{1}准备攻击，请选牌！", CurrentGameIndex, playerPack[CurrentGameIndex-1].ActorName));
    }

    private void DamageRes(MainPack mainPack)
    {
        
    }

    private void SkillRes(MainPack mainPack)
    {
        
    }

    #region Attack
    public void AttackReq()
    {
        if (CurrentGameIndex!=MyGameIndex)
        {
            UISys.ShowTipMsg("当前阶段不是您出牌");
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
            //UISys.ShowTipMsg("请选择卡牌！");
            UISys.ShowTipMsg("当前阶段:" + GameMgr.Instance.GetCurrentStateStr(Gamestate));
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

        var roomPack = mainPack.Roompack[0];
        CurrentGameIndex = roomPack.CurrentIndex;
        var playerPack = mainPack.Roompack[0].ActorPack;
        foreach (var player in playerPack)
        {
            RefreshCardDataByActorId(player.ActorId,player.CuttrntCards.ToList());
        }

        Gamestate = roomPack.Gamestate.State;
        BossActor.Refresh(roomPack.BossActor);
        EventCenter.Instance.EventTrigger("RefreshGameUI");

        UISys.ShowTipMsg(string.Format("当前{0}号玩家{1}攻击结束，请弃牌！",CurrentGameIndex, playerPack[CurrentGameIndex-1].ActorName));

    }
    #endregion

    #endregion


    #region List
    public const int TotalCardNum = 54;
    private List<global::CardData> m_totalList = new List<global::CardData>(TotalCardNum);  //总牌堆
    private List<global::CardData> m_myList = new List<global::CardData>(TotalCardNum);     //可抽卡
    private List<global::CardData> m_useList = new List<global::CardData>(TotalCardNum);    //墓地
    private List<global::CardData> m_bossList = new List<global::CardData>();                       //boss堆

    public List<global::CardData> UseCardDatas
    {
        get
        {
            return m_useList;
        }
    }
    #endregion

    private List<PlayerActor> m_players = new List<PlayerActor>();

    public void Attack(int actorIndex)
    {
        var actor = m_players[actorIndex];

        ActorEventHelper.Send(actor,"Attack");
    }

    public void Hurt(int actorIndex)
    {
        var actor = m_players[actorIndex];

        ActorEventHelper.Send(actor, "Hurt");
    }

    public int GetMyIndex()
    {
        //我自己必须是index0

        return 0;
    }
}