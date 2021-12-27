using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameOnlineMgr:Singleton<GameOnlineMgr>
{
    public int MyGameIndex { private set; get; }
    public int PlayerNum { private set; get; }
    public uint GameId { private set; get; }
    public uint GameIndex { private set; get; }

    #region List
    public const int TotalCardNum = 54;
    private List<CardData> m_totalList = new List<CardData>(TotalCardNum);  //总牌堆
    private List<CardData> m_myList = new List<CardData>(TotalCardNum);     //可抽卡
    private List<CardData> m_useList = new List<CardData>(TotalCardNum);    //墓地
    private List<CardData> m_bossList = new List<CardData>();                       //boss堆

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