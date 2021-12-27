using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed partial class GameApp : UnitySingleton<GameApp>
{
    public int TargetFrameRate = 300;

    public PlayerNum payerNum = PlayerNum.One;
    public enum PlayerNum
    {
        One,
        Two,
        Three,
        Four,
    }

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        SetTargetFrameRate();
        InitLibImp();
        RegistAllSystem();

        GameMgr.Instance.Init();
        UISys.Mgr.ShowWindow<StartUI>();

        //var str = ResourcesManager.Instance.Load<TextAsset>("regicide");
        //Debug.Log(str);
        //var jsonData = JsonHelper.Instance.Deserialize(str.text);
        //Debug.Log(jsonData);
    }
}

/*
    这片大陆上的国王们被邪恶的力量污染了，勇士们要击败他们，唤回他们心中的光明！
    玩家们必须击败的敌人是J、Q、K总共12张牌，并且他们都有着各自的血量和攻击力。
    玩家们通过出牌、连招、带上宠物，尝试对这些强力的敌人发起战斗，减少他们的血量，但是他们也会反击削减你的手牌。
    不同的花色有着不同的特殊能力，红心能恢复弃牌堆，方片可以抽牌，黑桃作为护盾，梅花造成两倍伤害！
    如果能正好将敌人的血量打空，则可以立即让他们恢复成正义的勇士，加入你的牌组和你一同作战！
 */
