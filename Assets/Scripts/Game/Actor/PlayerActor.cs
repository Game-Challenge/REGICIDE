using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor:GameActor
{
    private ActorEventMgr eventMgr;

    public ActorEventMgr Event
    {
        get
        {
            return eventMgr;
        }
    }
    public int Index = 0;

    private List<CardData> m_curList = new List<CardData>(GameMgr.Instance.MyMaxCardNum);    //手卡
    private List<CardData> m_choiceList = new List<CardData>();                             //当前回合选择的卡

    #region 战斗中卡排操作
    private void Choice(CardData cardData)
    {
        m_choiceList.Add(cardData);
    }

    private void DeChoice(CardData cardData)
    {
        for (int i = 0; i < m_choiceList.Count; i++)
        {
            m_choiceList.Remove(cardData);
        }
    }

    private void AbordCard(int needAbordValue)
    {
        if (GameMgr.Instance.gameState != GameMgr.GameState.STATEFOUR)
        {
            UISys.ShowTipMsg(string.Format("当前阶段是：{0}，无法遗弃卡牌", GameMgr.Instance.stateMsgDic[GameMgr.Instance.gameState]));
            return;
        }

        int myValue = 0;

        foreach (var card in m_choiceList)
        {
            myValue += card.CardValue;
        }

        if (myValue < needAbordValue)
        {
            UISys.ShowTipMsg("您需遗弃的牌点数不足");
            return;
        }

        for (int i = 0; i < m_choiceList.Count; i++)
        {
            var card = m_choiceList[i];

            GameMgr.Instance.m_useList.Add(card);

            m_curList.Remove(card);
        }

        m_choiceList.Clear();

        GameMgr.Instance.SetState(GameMgr.GameState.STATEONE);

        EventCenter.Instance.EventTrigger("RefreshGameUI");
    }

    private void AddHp(int num)
    {
        //Debug.Log("num " + num);
        //RandomSort(m_useList);
        //var count = m_useList.Count;

        //count = count < num ? count : num;

        //Debug.Log("Count " + count);
        //for (int i = 0; i < count; i++)
        //{
        //    var card = m_useList[i];

        //    m_myList.Add(card);

        //    m_useList.Remove(card);
        //}
    }
    #endregion

    public void Attack()
    {
        if (GameMgr.Instance.gameState != GameMgr.GameState.STATEONE)
        {
            UISys.ShowTipMsg(string.Format("当前阶段是：{0}，无法攻击", GameMgr.Instance.stateMsgDic[GameMgr.Instance.gameState]));
            return;
        }
        if (!GameMgr.Instance.CheckCardInvild(m_choiceList))
        {
            UISys.ShowTipMsg("您选择的卡片不符合规定");
            return;
        }

        var attackData = BattleMgr.Instance.GenAttackData(m_choiceList);

        for (int i = 0; i < m_choiceList.Count; i++)
        {
            var card = m_choiceList[i];

            GameMgr.Instance.m_useList.Add(card);

            m_curList.Remove(card);
        }

        m_choiceList.Clear();

        BattleMgr.Instance.ImpactSkill(attackData, GameMgr.Instance.BossActor);

        EventCenter.Instance.EventTrigger("RefreshGameUI");
    }

    public void Hurt(int value)
    {
        UISys.ShowTipMsg("受到君主的伤害:" + value);
        Debug.Log("Hurt:" + value);
        AbordCard(value);
    }
}