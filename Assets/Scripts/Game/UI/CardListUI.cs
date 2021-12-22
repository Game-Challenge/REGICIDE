using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CardListUI : UIWindow
{
    #region 脚本工具生成的代码
    private Transform m_tfContent;
    private GameObject m_itemCard;
    private List<ItemCard> cardLilst = new List<ItemCard>();
    protected override void ScriptGenerator()
    {
        m_tfContent = FindChild("ScrollView/Viewport/m_tfContent");
        m_itemCard = FindChild("m_itemCard").gameObject;
    }
    #endregion

    protected override void OnCreate()
    {
        base.OnCreate();

        GameMgr.Instance.RandomMyCards();
        GameMgr.Instance.TurnCard();
        var data = GameMgr.Instance.GetMyCard();

        AdjustIconNum(cardLilst, data.Count, m_tfContent, m_itemCard);
        m_itemCard.gameObject.SetActive(false);
        for (int i = 0; i < cardLilst.Count; i++)
        {
            cardLilst[i].Init(data[i]);
        }

        //AdjustIconNum(cardLilst,54,m_tfContent,m_itemCard);
        //m_itemCard.gameObject.SetActive(false);
        //for (int i = 0; i < cardLilst.Count; i++)
        //{
        //    cardLilst[i].Init(i);
        //}
    }

    #region 事件
    #endregion

}

class ItemCard : UIWindowWidget
{
    private CardData m_cardData;
    private bool m_choice = false;
    #region 脚本工具生成的代码
    private Image m_imgIcon;
    private GameObject m_goSelect;
    private Button m_btnChoice;
    protected override void ScriptGenerator()
    {
        m_imgIcon = FindChildComponent<Image>("m_imgIcon");
        m_goSelect = FindChild("m_goSelect").gameObject;
        m_btnChoice = FindChildComponent<Button>("m_imgIcon");
        m_btnChoice.onClick.AddListener(Choice);
    }
    #endregion

    #region 事件
    public void Init(int index){
        var sprite = CardMgr.Instance.GetCardSprite("card_"+index);
        if(sprite!= null){
            m_imgIcon.sprite = sprite;
        }
    }

    public void Init(CardData data)
    {
        m_cardData = data;
        m_imgIcon.sprite = m_cardData.sprite;
    }
    #endregion

    private void Choice()
    {
        m_choice = !m_choice;
        if (m_choice)
        {
            EventCenter.Instance.EventTrigger<CardData>("Choice",m_cardData);
            m_goSelect?.gameObject.SetActive(true);
        }
        else
        {
            EventCenter.Instance.EventTrigger<CardData>("DeChoice", m_cardData);
            m_goSelect?.gameObject.SetActive(false);
        }
    }
}
