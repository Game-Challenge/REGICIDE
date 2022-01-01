using System;
using System.Collections;
using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;

class GameOnlineMgr:Singleton<GameOnlineMgr>
{
    public GAMESTATE Gamestate { private set; get; }//当前状态
    public int MyGameIndex { private set; get; }//我是几号
    public int PlayerNum { private set; get; }  //玩家数目
    public int GameId { private set; get; }     //游戏ID
    public int GameIndex { private set; get; }  //当前是玩家几号

    public List<ActorPack> ActorPacks = new List<ActorPack>();
    public BossActor BossActor { private set; get; }
    #region 协议
    public void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.Attack, AttackRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Skill, SkillRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Damage, DamageRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Hurt, HurtRes);
    }

    public void InitGame(MainPack mainPack)
    {
        PlayerNum = mainPack.Roompack[0].ActorPack.Count;
        Gamestate = (GAMESTATE)mainPack.Roompack[0].State;
        GameIndex = mainPack.Roompack[0].CurrentIndex;


        ActorPacks = mainPack.Roompack[0].ActorPack.ToList();

        var bossActorPack = mainPack.Roompack[0].BossActor;

        BossActor = ActorMgr.Instance.InstanceBossActor(bossActorPack.ActorId);
    }

    private void HurtRes(MainPack mainPack)
    {
        
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

        GameClient.Instance.SendCSMsg(mainPack);
    }

    private void AttackRes(MainPack mainPack)
    {
        
    }
    #endregion

    #endregion


    #region List
    public const int TotalCardNum = 54;
    private List<CardData> m_totalList = new List<CardData>(TotalCardNum);  //总牌堆
    private List<CardData> m_myList = new List<CardData>(TotalCardNum);     //可抽卡
    private List<CardData> m_useList = new List<CardData>(TotalCardNum);    //墓地
    private List<CardData> m_bossList = new List<CardData>();                       //boss堆

    public List<CardData> UseCardDatas
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
}