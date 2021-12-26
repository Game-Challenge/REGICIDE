using System.Collections;
using UnityEngine;

class GameOnlineMgr:Singleton<GameOnlineMgr>
{
    public int PlayerNum { private set; get; }
    public uint GameId { private set; get; }
    public uint GameIndex { private set; get; }
}