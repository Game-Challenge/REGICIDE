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
            var actor = GameMgr.Instance.BossActor;
            actor.ReInit(cardData);
            return actor;
        }
    }
}