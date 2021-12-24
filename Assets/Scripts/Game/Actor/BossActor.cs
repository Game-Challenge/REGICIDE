using System.Collections;
using UnityEngine;

public class BossActor: GameActor
{
    public CardData cardData { private set; get; }

    public int Hp { private set; get; }

    public int Atk { private set; get; }

    public CardType cardType
    {
        get
        {
            return cardData.cardType;
        }
    }

    public BossActor(CardData cardData)
    {
        this.cardData = cardData;
        RegisterEvent();
        Init();
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener("Attack", Attack);
        EventCenter.Instance.AddEventListener<int>("Hurt", Hurt);
        EventCenter.Instance.AddEventListener<int>("DownAtk", DownAtk);
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
    }

    private void DownAtk(int value)
    {
        Atk -= value;

        if (Atk <= 0)
        {
            Atk = 0;
        }
        Debug.Log("Boss DownAtk ,Current Atk:" + Atk);
        EventCenter.Instance.EventTrigger("BossRefresh",this);
    }

    public void Hurt(int value)
    {
        Hp -= value;
        if (Hp <= 0)
        {
            Hp = 0;
            Debug.Log("BossDie");
            EventCenter.Instance.EventTrigger("BossDie");
            EventCenter.Instance.EventTrigger("BossRefresh", this);
        }
        else
        {
            EventCenter.Instance.EventTrigger("BossRefresh", this);
            MonoManager.Instance.StartCoroutine(BossAttack());
        }
        Debug.Log("Boss Hp:" + Hp);
    }

    private IEnumerator BossAttack()
    {
        yield return new WaitForSeconds(1);
        EventCenter.Instance.EventTrigger("BossAttack", Atk);
        EventCenter.Instance.EventTrigger("BossRefresh", this);
    }

    public void Attack()
    {
        EventCenter.Instance.EventTrigger("BossAttack");
        Debug.Log("BossAttack");
    }
}