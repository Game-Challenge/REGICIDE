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
    #region 脚本工具生成的代码
    private Image m_imgIcon;
    protected override void ScriptGenerator()
    {
        m_imgIcon = FindChildComponent<Image>("m_imgIcon");
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
        m_imgIcon.sprite = data.sprite;
    }
    #endregion

}
