using System.Collections;
using UnityEngine;

class GameOnlineMgr:Singleton<GameOnlineMgr>
{
    public uint GameId { private set; get; }
    public uint GameIndex { private set; get; }
}