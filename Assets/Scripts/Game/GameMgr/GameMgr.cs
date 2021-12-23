using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameMgr : Singleton<GameMgr>
{
    #region 属性
    public int PlayerNum;

    public int CurrentCardsNum
    {
        get
        {
            return m_curList.Count;
        }
    }

    /// <summary>
    /// 当前的手牌
    /// </summary>
    public List<CardData> CurrentCards
    {
        get
        {
            return m_curList;
        }
    }
    public const int TotalCardNum = 54;
    public const int MyMaxCardNum = 8;
    private BossActor m_bossActor;
    private List<CardData> m_totalList = new List<CardData>(TotalCardNum);  //总牌堆
    private List<CardData> m_curList = new List<CardData>(MyMaxCardNum);    //手卡
    private List<CardData> m_myList = new List<CardData>(TotalCardNum);     //可抽卡
    private List<CardData> m_useList = new List<CardData>(TotalCardNum);    //墓地
    private List<CardData> m_bossList = new List<CardData>();                       //boss堆
    private List<CardData> m_choiceList = new List<CardData>();
    #endregion

    #region 生命周期
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
        EventCenter.Instance.AddEventListener("AbordCard", AbordCard);
        EventCenter.Instance.AddEventListener<int>("BossAttack", Hurt);
        EventCenter.Instance.AddEventListener<int>("AttackBoss", AttackBoss);
        EventCenter.Instance.AddEventListener<CardData>("Choice", Choice);
        EventCenter.Instance.AddEventListener<CardData>("DeChoice", DeChoice);
        EventCenter.Instance.AddEventListener("BossDie", BossDie);
    }

    private void Update()
    {

    }
    #endregion

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

    private void AbordCard()
    {
        int myValue = 0;

        foreach (var card in m_choiceList)
        {
            myValue += card.CardValue;
        }

        if (myValue < m_needAbordValue)
        {
            Debug.Log("您需遗弃的牌点数不足");
            return;
        }

        for (int i = 0; i < m_choiceList.Count; i++)
        {
            var card = m_choiceList[i];

            m_useList.Add(card);

            m_curList.Remove(card);
        }

        m_choiceList.Clear();

        m_needAbordValue = 0;

        SetState(GameState.STATEONE);

        EventCenter.Instance.EventTrigger("RefreshGameUI");
    }
    #endregion

    #region Boss 接口
    public void InitBoss()
    {
        Random random = new Random((int)DateTime.Now.Ticks);
        var index = random.Next(0, m_bossList.Count - 1);
        var cardData = m_bossList[index];
        m_bossActor = ActorMgr.Instance.InstanceActor(cardData);
        EventCenter.Instance.EventTrigger<BossActor>("RefreshBoss", m_bossActor);
        SetState(GameState.STATEONE);
    }

    private void AttackBoss(int value)
    {
        if (m_bossActor != null)
        {
            m_bossActor.Hurt(value);
        }
        SetState(GameState.STATETHREE);
    }

    private void BossDie()
    {
        InitBoss();
    }
    #endregion

    #region 战斗相关

    public bool CheckCardSkill(List<CardData> choiceList)
    {
        bool doubleAtk = false;

        foreach (var card in choiceList)
        {
            if (card.cardType == CardType.DIAMOND)
            {
                TurnCard(card.CardValue);
            }
            else if (card.cardType == CardType.CLUB)
            {
                doubleAtk = true;
            }
            else if (card.cardType == CardType.SPADE)
            {
                EventCenter.Instance.EventTrigger<int>("DownAtk", card.CardValue);
            }
            else if (card.cardType == CardType.HEART)
            {

            }
        }
        SetState(GameState.STATETWO);
        return doubleAtk;
    }


    /// <summary>
    ///  验证卡是否合法
    /// </summary>
    /// <param name="choiceList"></param>
    /// <returns></returns>
    public bool CheckCardInvild(List<CardData> choiceList)
    {
        var count = choiceList.Count;

        if (count == 1)
        {
            return true;
        }
        else if (count == 2)
        {
            if (choiceList[0].CardValue == choiceList[1].CardValue)
            {
                return true;
            }
            else if (choiceList[0].CardValue == 1 || choiceList[1].CardValue == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (count == 0)
            {
                return false;
            }
            else
            {
                var value = choiceList[0].CardValue;
                for (int i = 0; i < count; i++)
                {
                    if (choiceList[i].CardValue != value)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void Attack()
    {
        if (gameState != GameState.STATEONE)
        {
            Debug.Log(string.Format("当前阶段是：{0}，无法攻击",gameState));
            return;
        }
        if (!CheckCardInvild(m_choiceList))
        {
            Debug.Log("您选择的卡片不符合规定");
            return;
        }
        var value = 0;
        for (int i = 0; i < m_choiceList.Count; i++)
        {
            var card = m_choiceList[i];

            value += card.CardValue;

            m_useList.Add(card);

            m_curList.Remove(card);
        }
        if (CheckCardSkill(m_choiceList))
        {
            value = value * 2;
        }
        m_choiceList.Clear();
        EventCenter.Instance.EventTrigger("RefreshGameUI");
        AttackBoss(value);
    }

    private int m_needAbordValue = 0;
    private void Hurt(int value)
    {
        SetState(GameState.STATEFOUR);
        Debug.Log("Hurt:" + value);
        m_needAbordValue = value;
    }
    #endregion

    #region Card操作
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

        var turnCount = MyMaxCardNum - CurrentCardsNum;
        turnCount = m_myList.Count > turnCount ? turnCount : m_myList.Count;
        for (int i = 0; i < turnCount; i++)
        {
            temp.Add(m_myList[i]);
            m_myList.RemoveAt(0);
        }
        m_curList.AddRange(temp);
        //EventCenter.Instance.EventTrigger("RefreshGameUI");
    }

    public void TurnCard(int turnCount)
    {
        List<CardData> temp = new List<CardData>();

        var couldTurnCount = MyMaxCardNum - CurrentCardsNum;

        if (couldTurnCount == 0)
        {
            return;
        }
        turnCount = m_myList.Count > turnCount ? turnCount : m_myList.Count;

        turnCount = couldTurnCount > turnCount ? turnCount : couldTurnCount;
        for (int i = 0; i < turnCount; i++)
        {
            temp.Add(m_myList[i]);
            m_myList.RemoveAt(0);
        }
        m_curList.AddRange(temp);
        //EventCenter.Instance.EventTrigger("RefreshGameUI");
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
    #endregion

    #region 游戏阶段
    private int m_stateIndex;
    public GameState gameState { private set; get; }

    public enum GameState
    {
        NONE,
        STATEONE,   //阶段一，打出手牌
        STATETWO,   //阶段二，激活技能
        STATETHREE, //阶段三，造成伤害
        STATEFOUR,  //阶段四，承受伤害
    }

    public void NextState()
    {
        m_stateIndex++;
        if (m_stateIndex > 4)
        {
            m_stateIndex = 1;
        }
        gameState = (GameState)m_stateIndex;
    }

    public void SetState(GameState state)
    {
        gameState = state;
        m_stateIndex = (int)state;
    }

    public void EndGame()
    {
        SetState(GameState.NONE);
    }
    #endregion
}