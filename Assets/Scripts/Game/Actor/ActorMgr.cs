using System.Collections;
using UnityEngine;

public class ActorMgr : Singleton<ActorMgr>
{
    public GameActor InstanceActor(CardData cardData)
    {
        return new BossActor(cardData);
    }

    public BossActor InstanceBossActor(CardData cardData)
    {
        if (GameMgr.Instance.BossActor == null)
        {
            return new BossActor(cardData);
        }
        else
        {
            GameMgr.Instance.BossActor = null;
            var actor = new BossActor(cardData);
            GameMgr.Instance.BossActor = actor;
            return actor;
        }
    }

    public BossActor InstanceBossActor(int cardInt)
    {
        var cardData = new CardData(cardInt);

        if (GameMgr.Instance.BossActor == null)
        {
            return new BossActor(cardData);
        }
        else
        {
            GameMgr.Instance.BossActor = null;
            var actor = new BossActor(cardData);
            GameMgr.Instance.BossActor = actor;
            return actor;
        }
    }
}