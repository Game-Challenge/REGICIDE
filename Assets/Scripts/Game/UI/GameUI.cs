using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GameUI : UIWindow
{
    private InventoryUI m_inventoryUI;
    private ItemCard m_bossCard;
    private List<ItemCard> m_cardLilst = new List<ItemCard>();
    #region 脚本工具生成的代码
    private GameObject m_goContent;
    private GameObject m_goMiddle;
    private GameObject m_goBottom;
    private GameObject m_itemCard;
    private Transform m_tfContent;
    private Transform m_tfCardContent;
    private Transform m_tfBossContent;
    private Button m_btnAttack;
    private Button m_btnAbord;
    private Transform m_goInventoryRoot;
    private Button m_btnUsed;
    protected override void ScriptGenerator()
    {
        m_goContent = FindChild("m_goContent").gameObject;
        m_goMiddle = FindChild("m_goContent/m_goMiddle").gameObject;
        m_goBottom = FindChild("m_goContent/m_goBottom").gameObject;
        m_itemCard = FindChild("m_goContent/m_goBottom/m_itemCard").gameObject;
        m_tfContent = FindChild("m_goContent/m_goBottom/ScrollView/Viewport/m_tfContent");
        m_tfCardContent = FindChild("m_goContent/m_goBottom/m_tfCardContent");
        m_tfBossContent = FindChild("m_goContent/m_goMiddle/m_tfBossContent");
        m_btnAttack = FindChildComponent<Button>("m_goContent/m_goMiddle/m_btnAttack");
        m_btnAbord = FindChildComponent<Button>("m_goContent/m_goMiddle/m_btnAbord");
        m_goInventoryRoot = FindChild("m_goContent/m_goMiddle/m_goInventoryRoot");
        m_btnUsed = FindChildComponent<Button>("m_goContent/m_goMiddle/m_btnUsed");
        m_btnAttack.onClick.AddListener(Attack);
        m_btnAbord.onClick.AddListener(Abord);
        m_btnUsed.onClick.AddListener(ShowUsed);
        m_inventoryUI = CreateWidgetByType<InventoryUI>(m_goInventoryRoot);
    }
    #endregion

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        EventCenter.Instance.AddEventListener<BossActor>("RefreshBoss", RefreshBoss);
        EventCenter.Instance.AddEventListener("RefreshGameUI", RefreshGameUI);
    }

    private void RefreshBoss(BossActor bossActor)
    {
        if (m_bossCard == null)
        {
            m_bossCard = CreateWidgetByPrefab<ItemCard>(m_itemCard, m_tfBossContent);
        }
        
        m_bossCard.Init(bossActor);
    }

    private void Attack()
    {
        GameMgr.Instance.Attack();
    }

    private void Abord()
    {
        EventCenter.Instance.EventTrigger("AbordCard");
    }

    private void ShowUsed()
    {
        UISys.Mgr.ShowWindow<UsedListUI>();
    }

    #region 事件
    protected override void OnCreate()
    {
        base.OnCreate();
        GameMgr.Instance.InitBoss();
        GameMgr.Instance.TurnCard();
        RefreshGameUI();
    }

    private void RefreshGameUI()
    {
        var data = GameMgr.Instance.CurrentCards;

        AdjustIconNum(m_cardLilst, data.Count, m_tfCardContent, m_itemCard);
        m_itemCard.gameObject.SetActive(false);
        for (int i = 0; i < m_cardLilst.Count; i++)
        {
            m_cardLilst[i].Init(data[i]);
        }
    }
    #endregion

}

