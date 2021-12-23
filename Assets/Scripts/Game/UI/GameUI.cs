using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GameUI : UIWindow
{
    private ItemCard m_bossCard;
    private List<ItemCard> cardLilst = new List<ItemCard>();
    #region 脚本工具生成的代码
    private GameObject m_goContent;
    private GameObject m_goMiddle;
    private GameObject m_goBottom;
    private GameObject m_itemCard;
    private Transform m_tfContent;
    private Transform m_tfBossContent;
    private Button m_btnAttack;
    protected override void ScriptGenerator()
    {
        m_goContent = FindChild("m_goContent").gameObject;
        m_goMiddle = FindChild("m_goContent/m_goMiddle").gameObject;
        m_goBottom = FindChild("m_goContent/m_goBottom").gameObject;
        m_itemCard = FindChild("m_goContent/m_goBottom/m_itemCard").gameObject;
        m_tfContent = FindChild("m_goContent/m_goBottom/ScrollView/Viewport/m_tfContent");
        m_tfBossContent = FindChild("m_goContent/m_goMiddle/m_tfBossContent");
        m_btnAttack = FindChildComponent<Button>("m_goContent/m_goMiddle/m_btnAttack");
        m_btnAttack.onClick.AddListener(Attack);
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
        
        m_bossCard.Init(bossActor.cardData);
    }

    private void Attack()
    {
        GameMgr.Instance.Attack();
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
        var data = GameMgr.Instance.GetMyCard();

        AdjustIconNum(cardLilst, data.Count, m_tfContent, m_itemCard);
        m_itemCard.gameObject.SetActive(false);
        for (int i = 0; i < cardLilst.Count; i++)
        {
            cardLilst[i].Init(data[i]);
        }
    }
    #endregion

}