using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor:GameActor
{
    private List<CardData> m_curList = new List<CardData>(GameMgr.Instance.MyMaxCardNum);    //手卡
    private List<CardData> m_choiceList = new List<CardData>();                             //当前回合选择的卡
}