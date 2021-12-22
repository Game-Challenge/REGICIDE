using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameMgr : Singleton<GameMgr>
{
    public const int TotalCardNum = 54;
    public const int MyMaxCardNum = 8;

    private List<CardData> m_totalList = new List<CardData>(TotalCardNum);  //总牌堆
    private List<CardData> m_curList = new List<CardData>(MyMaxCardNum);    //手卡
    private List<CardData> m_myList = new List<CardData>(TotalCardNum);     //可抽卡
    private List<CardData> m_useList = new List<CardData>(TotalCardNum);    //墓地
    private List<CardData> m_bossList = new List<CardData>();                       //boss堆

    public void Init()
    {
        InitTotalCards();
        InitMyCards();
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

    public static void RandomSort(List<CardData> list)
    {
        Random random = new Random((int)DateTime.Now.Ticks);

        list.Sort((x, y) => { return random.Next(-1, 1); });
    }
}