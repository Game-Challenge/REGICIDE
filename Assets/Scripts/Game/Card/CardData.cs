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
    SmallJoker = 14,
    Joker = 15,
}

public struct CardData
{
    #region 属性
    public CardType cardType;
    public int CardInt { private set; get; }
    public int CardValue { private set; get; }

    public Sprite sprite;

    public bool IsPet
    {
        get
        {
            return CardValue == 1;
        }
    }
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
        CardValue = (cardInt == 52 || cardInt == 53) ? 0 : (cardInt % 13) + 1;
        cardType =(CardValue == 0)?CardType.JOKER:(CardType)(((cardInt) / 13)+1);
        sprite = CardMgr.Instance.GetCardSprite(cardInt);
    }
}