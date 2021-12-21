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
        AdjustIconNum(cardLilst,10,m_tfContent,m_itemCard);
        m_itemCard.gameObject.SetActive(false);
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
    #endregion

}
