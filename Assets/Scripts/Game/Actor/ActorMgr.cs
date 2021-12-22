using System.Collections;
using UnityEngine;

public class ActorMgr : Singleton<ActorMgr>
{
    public BossActor InstanceActor(CardData cardData)
    {
        return new BossActor(cardData);
    }
}