using System.Collections.Generic;
using RegicideProtocol;
using UnityEngine;
using UnityEngine.UI;

class JokerChoiceUI : UIWindow
{
    private List<JokerChoiceItem> m_items = new List<JokerChoiceItem>();
    #region 脚本工具生成的代码
    private Transform m_tfPlayerContent;
    private GameObject m_itemPlayer;
    private Text m_textInfo;
    protected override void ScriptGenerator()
    {
        m_tfPlayerContent = FindChild("m_tfPlayerContent");
        m_itemPlayer = FindChild("m_tfPlayerContent/m_itemPlayer").gameObject;
        m_textInfo = FindChildComponent<Text>("m_textInfo");
    }
	#endregion

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        EventCenter.Instance.AddEventListener("CloseJokerChoice", CloseUI);
    }

    protected override void DeRegisterEvent()
    {
        base.RegisterEvent();
        EventCenter.Instance.RemoveEventListener("CloseJokerChoice", CloseUI);
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        var actorPack = GameOnlineMgr.Instance.ActorPacks;

        AdjustIconNum(m_items, actorPack.Count, m_tfPlayerContent, m_itemPlayer);

        for (int i = 0; i < actorPack.Count; i++)
        {
            m_items[i].Init(actorPack[i],i);
        }
        m_itemPlayer.gameObject.SetActive(false);
    }

    private void CloseUI()
    {
        Close();
    }
    #region 事件
	#endregion

}

class JokerChoiceItem : UIWindowWidget
{
    #region 脚本工具生成的代码
    private Image m_imgIsMe;
    private Text m_textName;
    private Text m_textID;
    private Image m_imgHeadIcon;
    private Button m_btnChoice;
    protected override void ScriptGenerator()
    {
        m_imgIsMe = FindChildComponent<Image>("m_imgIsMe");
        m_textName = FindChildComponent<Text>("m_textName");
        m_textID = FindChildComponent<Text>("m_textID");
        m_imgHeadIcon = FindChildComponent<Image>("m_imgHeadIcon");
        m_btnChoice = FindChildComponent<Button>("m_btnChoice");
        m_btnChoice.onClick.AddListener(OnClickChoiceBtn);
    }
    #endregion

    private int m_Index;
    public void Init(ActorPack actorPack,int index)
    {
        if (actorPack == null)
        {
            return;
        }

        m_textName.text = actorPack.ActorName;

        m_textID.text = actorPack.ActorId.ToString();

        if (actorPack.ActorId == GameOnlineMgr.Instance.MyActorId)
        {
            m_imgIsMe.gameObject.SetActive(true);
        }

        m_Index = index;
    }

    #region 事件
    private void OnClickChoiceBtn()
    {
        GameOnlineMgr.Instance.AttackReq(true, m_Index);
        EventCenter.Instance.EventTrigger("CloseJokerChoice");
    }
    #endregion

}
