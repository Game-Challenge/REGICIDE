using System;
using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;
using UnityEngine.UI;

class GameOnlineUI : UIWindow
{
    private InventoryUI m_inventoryUI;
    private ItemCard m_bossCard;
    private List<Text> m_textPlayerList = new List<Text>();
    private List<ItemCard> m_cardList = new List<ItemCard>();


    private ItemCard m_player2Cards;
    private ItemCard m_player3Cards;
    private ItemCard m_player4Cards;

    #region 脚本工具生成的代码
    private GameObject m_goContent;
    private GameObject m_goMiddle;
    private Transform m_tfBossContent;
    private Button m_btnAttack;
    private Button m_btnAbord;
    private GameObject m_goInventoryRoot;
    private Button m_btnUsed;
    private GameObject m_goCardsRoot;
    private GameObject m_itemCard;
    private List<Transform> m_tfCardContent = new List<Transform>();

    protected override void ScriptGenerator()
    {
        m_goContent = FindChild("m_goContent").gameObject;
        m_goMiddle = FindChild("m_goContent/m_goMiddle").gameObject;
        m_tfBossContent = FindChild("m_goContent/m_goMiddle/m_tfBossContent");
        m_btnAttack = FindChildComponent<Button>("m_goContent/m_goMiddle/m_btnAttack");
        m_btnAbord = FindChildComponent<Button>("m_goContent/m_goMiddle/m_btnAbord");
        m_goInventoryRoot = FindChild("m_goContent/m_goMiddle/m_goInventoryRoot").gameObject;
        m_btnUsed = FindChildComponent<Button>("m_goContent/m_goMiddle/m_btnUsed");
        m_goCardsRoot = FindChild("m_goContent/m_goCardsRoot").gameObject;
        m_itemCard = FindChild("m_goContent/m_goCardsRoot/m_itemCard").gameObject;
        for (int i = 1; i <=4; i++)
        {
            m_tfCardContent.Add(FindChild("m_goContent/m_goCardsRoot/m_tfCardContent"+i));
            m_textPlayerList.Add(FindChildComponent<Text>("m_goContent/m_goCardsRoot/m_textPlayer"+i));
        }
        m_btnAttack.onClick.AddListener(OnClickAttackBtn);
        m_btnAbord.onClick.AddListener(OnClickAbordBtn);
        m_btnUsed.onClick.AddListener(OnClickUsedBtn);
    }
    #endregion

    #region Create Destroy生命周期
    protected override void OnCreate()
    {
        m_itemCard.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            m_textPlayerList[i].gameObject.SetActive(false);
        }

        GameOnlineMgr.Instance.IsOnlineGameIng = true;
    }

    protected override void OnDestroy()
    {
        GameOnlineMgr.Instance.IsOnlineGameIng = false;
    }

    #endregion

    public void Init(MainPack mainPack)
    {
        if (mainPack == null)
        {
            return;
        }

        var players = GameOnlineMgr.Instance.ActorPacks;

        if (players.Count > 4)
        {
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            m_textPlayerList[i].gameObject.SetActive(true);
            m_textPlayerList[i].text = string.Format("玩家{0} {1}", players[i].ActorId, players[i].ActorId);
            //Todo Ugly code
            if (i == 1)
            {
                m_player2Cards = CreateWidgetByPrefab<ItemCard>(m_itemCard, m_tfCardContent[1]);
                m_player2Cards.gameObject.SetActive(true);
            }
            else if (i == 2)
            {
                m_player3Cards = CreateWidgetByPrefab<ItemCard>(m_itemCard, m_tfCardContent[2]);
                m_player3Cards.gameObject.SetActive(true);
            }
            else if (i == 3)
            {
                m_player4Cards = CreateWidgetByPrefab<ItemCard>(m_itemCard, m_tfCardContent[3]);
                m_player4Cards.gameObject.SetActive(true);
            }
        }


        RefreshGameUI();
    }

    private void RefreshMyCards()
    {
        var myCardData = GameOnlineMgr.Instance.GetMyCardData();

        AdjustIconNum(m_cardList, myCardData.Count, m_tfCardContent[0], m_itemCard);

        for (int i = 0; i < m_cardList.Count; i++)
        {
            m_cardList[i].Init(myCardData[i]);
        }
    }

    private void RefeshOthersCard()
    {
        var MainPlayerIndex = GameOnlineMgr.Instance.MyGameIndex;   //0,1,2,3

        var players = GameOnlineMgr.Instance.ActorPacks;

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];

            if (player.ActorId == GameOnlineMgr.Instance.MyActorId)
            {
                continue;
            }

            var cardData = GameOnlineMgr.Instance.GetCardDataByActorId(player.ActorId);

            var currentIndex = 0;

            currentIndex = MainPlayerIndex + i <= (players.Count-1) ? MainPlayerIndex + i : (players.Count-1) - MainPlayerIndex + i;

            if (currentIndex == 1)
            {
                m_player2Cards.Init(cardData);
            }
            else if (currentIndex == 2)
            {
                m_player3Cards.Init(cardData);
            }
            else if (currentIndex == 3)
            {
                m_player4Cards.Init(cardData);
            }
        }
    }

    private void RefreshBoss(BossActor bossActor)
    {
        if (m_bossCard == null)
        {
            m_bossCard = CreateWidgetByPrefab<ItemCard>(m_itemCard, m_tfBossContent);
        }

        m_bossCard.Init(bossActor);
    }

    /// <summary>
    /// 刷新UI事件
    /// </summary>
    private void RefreshGameUI()
    {
        RefreshMyCards();
        RefeshOthersCard();
        RefreshBoss(GameOnlineMgr.Instance.BossActor);
    }

    private void UpdateGameState()
    {

    }

    #region 注册消息事件
    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        EventCenter.Instance.AddEventListener<BossActor>("RefreshBoss", RefreshBoss);
        EventCenter.Instance.AddEventListener("RefreshGameUI", RefreshGameUI);
        EventCenter.Instance.AddEventListener("UpdateGameState", UpdateGameState);
    }

    protected override void DeRegisterEvent()
    {
        base.DeRegisterEvent();
        EventCenter.Instance.RemoveEventListener<BossActor>("RefreshBoss", RefreshBoss);
        EventCenter.Instance.RemoveEventListener("RefreshGameUI", RefreshGameUI);
        EventCenter.Instance.RemoveEventListener("UpdateGameState", UpdateGameState);
    }
    #endregion


    private void RefreshCards(int index = 0)
    {
        if (index >= 4)
        {
            return;
        }
        //AdjustIconNum(m_cardList[index],8,m_tfCardContent[index], m_itemCard);
    }

    #region 事件
    private void OnClickAttackBtn()
    {
        GameOnlineMgr.Instance.AttackReq();
    }
    private void OnClickAbordBtn()
    {
        GameOnlineMgr.Instance.AbordReq();
    }
    private void OnClickUsedBtn()
    {
        var ui = UISys.Mgr.ShowWindow<UsedListUI>();
        ui.InitOnlineUI();
    }
    #endregion

}