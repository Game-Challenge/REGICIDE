public enum CardType
{
    NONE,  
    CLUB,   //草花
    DIAMOND,//方块
    HEART,  //红心
    SPADE,  //黑桃
    JOKER,  //
}

public class CardData{
    public CardType cardType = CardType.NONE;

    public int CardInt;
}