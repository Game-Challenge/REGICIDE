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
    }

    public void Hurt(int value)
    {
        Hp -= value;
        if (Hp <= 0)
        {
            Hp = 0;
            Debug.Log("BossDie");
            EventCenter.Instance.EventTrigger("BossDie");
        }
        else
        {
            MonoManager.Instance.StartCoroutine(BossAttack());
        }
        Debug.Log("Boss Hp:" + Hp);
    }

    private IEnumerator BossAttack()
    {
        yield return new WaitForSeconds(1);
        EventCenter.Instance.EventTrigger("BossAttack", Atk);
    }

    public void Attack()
    {
        EventCenter.Instance.EventTrigger("BossAttack");
        Debug.Log("BossAttack");
    }
}