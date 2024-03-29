using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardMgr:Singleton<CardMgr>{
    private Sprite[] m_sprites;
    private Dictionary<string,Sprite> m_spriteDic= new Dictionary<string, Sprite>();
    public Sprite BigJoker;
    public Sprite BlackJoker;
    public CardMgr(){
        m_sprites = Resources.LoadAll<Sprite>("Images/card");
        for (int i = 0; i < m_sprites.Length; i++)
        {
            var sprite = m_sprites[i];
            m_spriteDic.Add(sprite.name,sprite);
        }

        BigJoker = Resources.Load<Sprite>("Images/BigJoker");

        var bossSprite = Resources.LoadAll<Sprite>("Images/Boss/");
        for (int i = 0; i < bossSprite.Length; i++)
        {
            var sprite = bossSprite[i];
            m_spriteDic.Add(sprite.name, sprite);
        }
    }

    public Sprite GetCardSprite(string name){
        Sprite sprite = null;
        if(m_spriteDic.TryGetValue(name,out sprite)){
            return sprite;
        }
        return sprite;
    }
    public Sprite GetCardSprite(int index)
    {
        Sprite sprite = null;
        if (m_spriteDic.TryGetValue("card_" + index, out sprite))
        {
            return sprite;
        }
        return sprite;
    }

    public CardData InstanceData(int cardInt)
    {
        return new CardData(cardInt);
    }
}