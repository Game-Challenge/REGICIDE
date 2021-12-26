using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameOnlineMgr:Singleton<GameOnlineMgr>
{
    public int PlayerNum { private set; get; }
    public uint GameId { private set; get; }
    public uint GameIndex { private set; get; }

    private List<PlayerActor> m_players = new List<PlayerActor>();
}