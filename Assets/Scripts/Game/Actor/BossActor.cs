using System.Collections;
using UnityEngine;

public class BossActor
{
    public CardData cardData { private set; get; }

    public int Hp { private set; get; }

    public int Atk { private set; get; }

    public BossActor(CardData cardData)
    {
        this.cardData = cardData;
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
        }
    }

    public void Hurt(int value)
    {
        Hp -= value;
        if (Hp <= 0)
        {
            Hp = 0;
            Debug.Log("BossDie");
        }
        Debug.Log("Boss Hp:" + Hp);
    }

    public void Attack()
    {
        EventCenter.Instance.EventTrigger("BossAttack");
        Debug.Log("BossAttack");
    }
}