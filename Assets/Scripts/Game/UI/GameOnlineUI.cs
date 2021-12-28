using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GameOnlineUI : UIWindow
{
    private InventoryUI m_inventoryUI;
    private ItemCard m_bossCard;
    private List<List<ItemCard>> m_cardList = new List<List<ItemCard>>();

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
        }
        m_btnAttack.onClick.AddListener(OnClickAttackBtn);
        m_btnAbord.onClick.AddListener(OnClickAbordBtn);
        m_btnUsed.onClick.AddListener(OnClickUsedBtn);
    }
    #endregion

    protected override void OnCreate()
    {
        base.OnCreate();
        for (int i = 0; i < 4; i++)
        {
            List<ItemCard> list = new List<ItemCard>();
            m_cardList.Add(list);
            RefreshCards(i);
        }
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
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

    }
    #endregion

}