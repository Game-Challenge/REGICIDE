using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;
using UnityEngine.UI;

class GameOnlineUI : UIWindow
{
    private InventoryUI m_inventoryUI;
    private ItemCard m_bossCard;
    private List<List<ItemCard>> m_cardList = new List<List<ItemCard>>();
    private List<Text> m_textPlayerList = new List<Text>();

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

    protected override void OnCreate()
    {
        base.OnCreate();
        m_itemCard.gameObject.Show(false);
        for (int i = 0; i < 4; i++)
        {
            List<ItemCard> list = new List<ItemCard>();
            m_cardList.Add(list);
            m_textPlayerList[i].gameObject.Show(false);
        }
    }

    public void Init(MainPack mainPack)
    {
        if (mainPack == null)
        {
            return;
        }

        var players = GameOnlineMgr.Instance.ActorPacks;
        for (int i = 0; i < players.Count; i++)
        {
            m_textPlayerList[i].gameObject.Show(true);
            //m_tfCardContent[2].
                //m_bossCard = CreateWidgetByPrefab<ItemCard>(m_itemCard, m_tfBossContent);
        }


        RefreshBoss(GameOnlineMgr.Instance.BossActor);


        var temp = new List<CardData>();
        foreach (var card in players[0].CuttrntCards)
        {
            var cardData = CardMgr.Instance.InstanceData(card.CardInt);
            temp.Add(cardData);
        }

        AdjustIconNum(m_cardList[0], temp.Count, m_tfCardContent[0], m_itemCard);
        for (int i = 0; i < m_cardList[0].Count; i++)
        {
            m_cardList[0][i].Init(temp[i]);
        }
    }

    private void RefreshCards()
    {

    }

    private void RefreshBoss(BossActor bossActor)
    {
        if (m_bossCard == null)
        {
            m_bossCard = CreateWidgetByPrefab<ItemCard>(m_itemCard, m_tfBossContent);
        }

        m_bossCard.Init(bossActor);
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
    }

    protected override void DeRegisterEvent()
    {
        base.DeRegisterEvent();
    }

    private void RefreshCards(int index = 0)
    {
        if (index >= 4)
        {
            return;
        }
        AdjustIconNum(m_cardList[index],8,m_tfCardContent[index], m_itemCard);
    }

    #region 事件
    private void OnClickAttackBtn()
    {
        GameOnlineMgr.Instance.AttackReq();
    }
    private void OnClickAbordBtn()
    {

    }
    private void OnClickUsedBtn()
    {
        var ui = UISys.Mgr.ShowWindow<UsedListUI>();
        ui.InitOnlineUI();
    }
    #endregion

}