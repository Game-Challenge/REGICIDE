using UnityEngine;

public enum CardType
{
    NONE,  
    CLUB,   //草花
    DIAMOND,//方块
    HEART,  //红心
    SPADE,  //黑桃
    JOKER,  //
}

public enum CardValue
{
    NONE,
    J = 11, 
    Q = 12,
    K = 13,
}

public struct CardData
{
    #region 属性
    public CardType cardType;
    public int CardInt { private set; get; }
    public int CardValue { private set; get; }

    public Sprite sprite;
    public bool IsBoss
    {
        get
        {
            return CardValue > 10 && !IsJoker;
        }
    }

    public bool IsJoker
    {
        get
        {
            return cardType == CardType.JOKER;
        }
    }
    #endregion

    public CardData(int cardInt)
    {
        CardInt = cardInt;
        CardValue = (cardInt == 52 || cardInt == 53) ? 100 : (cardInt % 13) + 1;
        cardType =(CardValue == 100)?CardType.JOKER:(CardType)(((cardInt) / 13)+1);
        sprite = CardMgr.Instance.GetCardSprite(cardInt);
    }
}