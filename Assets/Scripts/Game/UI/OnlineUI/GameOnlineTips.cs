using System.Collections.Generic;
using Google.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

class GameOnlineTips : UIWindow
{
    private List<ItemCard> m_list = new List<ItemCard>();
    private TipUIStatus m_status = TipUIStatus.Init;
    private float m_statusTime = 0f;

    private List<string> m_waitTip = new List<string>();

    private float m_duration = 3f;
    private float m_showTime;

    private TipUIStatus Status
    {
        set
        {
            m_status = value;
            m_statusTime = Time.time;
        }
    }

    #region 脚本工具生成的代码
    private GameObject m_goTips;
    private Text m_textTip;
    private Transform m_tfCardContent;
    private GameObject m_itemCard;
    protected override void ScriptGenerator()
    {
        m_textTip = FindChildComponent<Text>("m_goTips/Text");
        m_goTips = FindChild("m_goTips").gameObject;
        m_tfCardContent = FindChild("m_tfCardContent");
        m_itemCard = FindChild("m_itemCard").gameObject;
    }
    #endregion

    protected override void OnCreate()
    {
        m_showTime = 1f;
        Status = TipUIStatus.Init;
    }

    protected override void OnUpdate()
    {
        RefreshTip();
    }

    public void ShowTip(string tip,RepeatedField<RegicideProtocol.CardData> cards, int diyTime = 0)
    {
        if (diyTime > 0)
        {
            m_duration = diyTime;
        }
        else if (m_showTime > 0)
        {
            m_duration = m_showTime;
        }
        else
        {
            m_duration = 1;
        }
        if (m_waitTip.Contains(tip))
        {
            return;
        }

        AdjustIconNum(m_list,cards.Count,m_tfCardContent, m_itemCard);
        for (int i = 0; i < cards.Count; i++)
        {
            m_list[i].Init(cards[i]);
        }

        m_waitTip.Add(tip);
        RefreshTip();
    }

    private void RefreshTip()
    {
        ///判断当前是否在显示
        switch (m_status)
        {
            case TipUIStatus.Init:
            {
                if (m_waitTip.Count > 0)
                {
                    m_textTip.text = m_waitTip[0];
                    m_waitTip.RemoveAt(0);

                    Status = TipUIStatus.Show;
                }
            }
                break;
            case TipUIStatus.Show:
            {
                if (m_statusTime + m_duration < Time.time)
                {
                    if (m_waitTip.Count <= 0)
                    {
                        Close();
                    }
                    else
                    {
                        ///这儿应该做动画，省略，继续运行
                        m_textTip.text = string.Empty;
                        Status = TipUIStatus.Cool;
                    }
                }
            }
                break;
            case TipUIStatus.Cool:
            {
                if (m_statusTime + 0.2 < Time.time)
                {
                    ///这儿应该做动画，省略，继续运行
                    Status = TipUIStatus.Init;
                    if (m_waitTip.Count <= 0)
                    {
                        Close();
                    }
                }
            }
                break;
        }
    }

    #region 事件
    #endregion

}