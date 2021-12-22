using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameMgr : Singleton<GameMgr>
{
    public int PlayerNum;

    public int CurrentCardNum = 0;

    public const int TotalCardNum = 54;
    public const int MyMaxCardNum = 8;

    private BossActor m_bossActor;

    private List<CardData> m_totalList = new List<CardData>(TotalCardNum);  //总牌堆
    private List<CardData> m_curList = new List<CardData>(MyMaxCardNum);    //手卡
    private List<CardData> m_myList = new List<CardData>(TotalCardNum);     //可抽卡
    private List<CardData> m_useList = new List<CardData>(TotalCardNum);    //墓地
    private List<CardData> m_bossList = new List<CardData>();                       //boss堆
    private List<CardData> m_choiceList = new List<CardData>();                      
    public void Init()
    {
        RegiserEvent();
        PlayerNum = (int)GameApp.Instance.payerNum + 1;
        InitTotalCards();
        InitMyCards();
        GameMgr.Instance.RandomMyCards();
    }

    private void RegiserEvent()
    {
        MonoManager.Instance.AddUpdateListener(Update);
        EventCenter.Instance.AddEventListener("BossAttack", Hurt);
        EventCenter.Instance.AddEventListener<int>("AttackBoss", AttackBoss);
        EventCenter.Instance.AddEventListener<CardData>("Choice", Choice);
        EventCenter.Instance.AddEventListener<CardData>("DeChoice", DeChoice);
    }

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

    public void Attack()
    {
        var value = 0;
        for (int i = 0; i < m_choiceList.Count; i++)
        {
            var card = m_choiceList[i];

            value += card.CardValue;

            m_useList.Add(card);

            m_curList.Remove(card);
        }
        m_choiceList.Clear();
        EventCenter.Instance.EventTrigger("RefreshGameUI");
        AttackBoss(value);
    }

    public void InitBoss()
    {
        Random random = new Random((int)DateTime.Now.Ticks);
        var index = random.Next(0, m_bossList.Count - 1);
        var cardData = m_bossList[index];
        m_bossActor = ActorMgr.Instance.InstanceActor(cardData);
        EventCenter.Instance.EventTrigger<BossActor>("InitBoss", m_bossActor);
    }

    private void AttackBoss(int value)
    {
        if (m_bossActor != null)
        {
            m_bossActor.Hurt(value);
        }
    }

    private void Hurt()
    {
        Debug.Log("Hurt");
    }

    private void InitTotalCards()
    {
        m_totalList.Clear();
        for (int cardInt = 0; cardInt < TotalCardNum; cardInt++)
        {
            var cardData = CardMgr.Instance.InstanceData(cardInt);

            m_totalList.Add(cardData);
        }
    }

    private void InitMyCards()
    {
        for (int i = 0; i < m_totalList.Count; i++)
        {
            var cardData = m_totalList[i];
            if (cardData.IsBoss)
            {
                m_bossList.Add(cardData);
            }
            else
            {
                m_myList.Add(cardData);
            }
        }
    }

    public void RandomMyCards()
    {
        RandomSort(m_myList);
    }

    public void TurnCard()
    {
        List<CardData> temp = new List<CardData>();

        var turnCount = MyMaxCardNum - CurrentCardNum;
        turnCount = m_myList.Count > turnCount ? turnCount : m_myList.Count;
        for (int i = 0; i < turnCount; i++)
        {
            temp.Add(m_myList[i]);
            m_myList.RemoveAt(0);
        }
        m_curList.AddRange(temp);
    }

    public List<CardData> GetMyCard()
    {
        return m_curList;
    }

    public static void RandomSort<T>(List<T> list)
    {
        Random random = new Random((int)DateTime.Now.Ticks);

        int index = 0;
        for (int i = 0; i < list.Count; i++)
        {

            index = random.Next(0, list.Count - 1);
            if (index != i)
            {
                var temp = list[i];
                list[i] = list[index];
                list[index] = temp;
                //(list[i], list[index]) = (list[i], list[index]);
            }
        }
    }

    //private List<T> RandomSort<T>(List<T> list)
    //{
    //    var random = new Random((int)DateTime.Now.Ticks);
    //    var newList = new List<T>();
    //    foreach (var item in list)
    //    {
    //        newList.Insert(random.Next(newList.Count), item);
    //    }
    //    return newList;
    //}

    private void Update()
    {

    }

    public enum GameState
    {
        NONE,
        STATEONE,
        STATETWO,
        STATETHREE,
        STATEFOUR,
    }
}