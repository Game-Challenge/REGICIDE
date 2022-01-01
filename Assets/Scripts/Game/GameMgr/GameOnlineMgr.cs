using System;
using System.Collections;
using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;
using CardData = RegicideProtocol.CardData;

class GameOnlineMgr:Singleton<GameOnlineMgr>
{
    public GAMESTATE Gamestate { private set; get; }//当前状态
    public int MyGameIndex { private set; get; }//我是几号,第几个出牌的
    public int PlayerNum { private set; get; }  //玩家数目
    public int GameId { private set; get; }     //游戏ID
    public int GameIndex { private set; get; }  //当前是玩家几号
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

        Debug.Log("初始化成功："+MyActorId);
    }

    #region 协议
    public void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.Attack, AttackRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Skill, SkillRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Damage, DamageRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Hurt, AbordRes);
    }

    public void InitGame(MainPack mainPack)
    {
        PlayerNum = mainPack.Roompack[0].ActorPack.Count;
        Gamestate = (GAMESTATE)mainPack.Roompack[0].State;
        GameIndex = mainPack.Roompack[0].CurrentIndex;


        ActorPacks = mainPack.Roompack[0].ActorPack.ToList();

        var bossActorPack = mainPack.Roompack[0].BossActor;

        BossActor = ActorMgr.Instance.InstanceBossActor(bossActorPack.ActorId);

        InitCards();
    }

    private void InitCards()
    {
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

            CardDictionary.Add(players[i].ActorId,temp);
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
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Game, ActionCode.Attack);

        List<global::CardData> list = GameMgr.Instance.m_choiceList;
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