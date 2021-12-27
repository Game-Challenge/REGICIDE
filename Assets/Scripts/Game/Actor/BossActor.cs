﻿using System.Collections;
using UnityEngine;

public class BossActor: GameActor
{
    public CardData cardData { private set; get; }

    public int MaxHp { private set; get; }

    public int Hp { private set; get; }

    public int Atk { private set; get; }

    public bool JokerAtk { private set; get; }

    public CardType cardType
    {
        get
        {
            if (JokerAtk)
            {
                return CardType.NONE;
            }
            return cardData.cardType;
        }
    }

    public BossActor(CardData cardData)
    {
        this.cardData = cardData;
        RegisterEvent();
        Init();
    }

    ~BossActor()
    {
        DeRegisterEvent();
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener("Attack", Attack);
        EventCenter.Instance.AddEventListener<int>("Hurt", Hurt);
        EventCenter.Instance.AddEventListener<int>("DownAtk", DownAtk);
        EventCenter.Instance.AddEventListener("BeJokerAtk", BeJokerAtk);
    }

    private void DeRegisterEvent()
    {
        EventCenter.Instance.RemoveEventListener("Attack", Attack);
        EventCenter.Instance.RemoveEventListener<int>("Hurt", Hurt);
        EventCenter.Instance.RemoveEventListener<int>("DownAtk", DownAtk);
        EventCenter.Instance.RemoveEventListener("BeJokerAtk", BeJokerAtk);
    }

    public void ReInit(CardData cardData)
    {
        this.cardData = cardData;
        JokerAtk = false;
        m_cacheAtk = 0;
        Init();
    }

    private void Init()
    {
        switch ((CardValue)cardData.CardValue)
        {
            case CardValue.J:
            {
                Hp = 20;
                Atk = 10;
                break;
            }
            case CardValue.Q:
            {
                Hp = 30;
                Atk = 15;
                break;
            }
            case CardValue.K:
            {
                Hp = 40;
                Atk = 20;
                break;
            }
            case CardValue.SmallJoker:
            {
                Hp = 50;
                Atk = 25;
                break;
            }
            case CardValue.Joker:
            {
                Hp = 60;
                Atk = 35;
                break;
            }
        }

        MaxHp = Hp;
    }

    private void BeJokerAtk()
    {
        JokerAtk = true;
    }

    private int m_cacheAtk = 0;
    private void DownAtk(int value)
    {
        if (this.cardType == CardType.SPADE)
        {
            m_cacheAtk -= value;

            if (m_cacheAtk <= 0)
            {
                m_cacheAtk = 0;
            }

            if (JokerAtk)
            {
                Atk = m_cacheAtk;
                EventCenter.Instance.EventTrigger("BossDataRefresh", this);
            }
        }
        else
        {
            Atk -= value;

            if (Atk <= 0)
            {
                Atk = 0;
            }
            Debug.Log("Boss DownAtk ,Current Atk:" + Atk);
            EventCenter.Instance.EventTrigger("BossDataRefresh", this);
        }
    }

    public void Hurt(int value)
    {
        Hp -= value;
        if (Hp <= 0)
        {
            EventCenter.Instance.EventTrigger("BossDie", Hp == 0); //boss被归化，变成卡堆第一张
            Hp = 0;
        }
        else
        {
            MonoManager.Instance.StartCoroutine(BossAttack());
        }
        Debug.Log("Boss Hp:" + Hp);
        EventCenter.Instance.EventTrigger("BossDataRefresh", this);
    }

    private IEnumerator BossAttack()
    {
        yield return new WaitForSeconds(1);
        EventCenter.Instance.EventTrigger("BossAttack", Atk);
        EventCenter.Instance.EventTrigger("BossDataRefresh", this);
    }

    public void Attack()
    {
        EventCenter.Instance.EventTrigger("BossAttack");
        Debug.Log("BossAttack");
    }
}