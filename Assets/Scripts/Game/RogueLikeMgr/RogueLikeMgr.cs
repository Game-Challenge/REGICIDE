using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData
{
    public int ExAtk;
    public int ExDef;
    public bool NeedCheckCards = true;

    public int LeftFeatureSlot = 3;

    public int Talent1;
    public int Talent2;
    public int Talent3;

    public void Clear()
    {
        ExAtk = 0;
        ExDef = 0;
        NeedCheckCards = true;
        LeftFeatureSlot = 3;
    }
}


partial class RogueLikeMgr:Singleton<RogueLikeMgr>
{
    private Dictionary<int,PlayerFeatureConfig> m_dicFeatures = new Dictionary<int, PlayerFeatureConfig>();

    public Dictionary<int, PlayerFeatureConfig> DicFeatures
    {
        get { return m_dicFeatures; }
    }

    public List<PlayerFeatureConfig> FeaturesList = new List<PlayerFeatureConfig>();


    public PlayerData playerData;
    public RogueLikeMgr()
    {
        playerData = new PlayerData();
        buffMgr = new PlayerBuffMgr();
    }

    ~RogueLikeMgr()
    {
        buffMgr.ClearBuff();
        buffMgr = null;
    }

    public PlayerBuffMgr buffMgr;

    public void AddFeature(PlayerFeatureConfig playerFeature)
    {
        if (!m_dicFeatures.ContainsKey(playerFeature.ID))
        {
            playerData.LeftFeatureSlot -= playerFeature.Slot;

            m_dicFeatures.Add(playerFeature.ID,playerFeature);

            buffMgr.AddFeature(playerFeature);

            FeaturesList.Add(playerFeature);
        }
        EventCenter.Instance.EventTrigger("RefreshFeature");
    }

    public void RemoveFeature(int featureId)
    {
        var feature = PlayerFeatureConfigMgr.Instance.GetPlayerFeatureConfig(featureId);

        if (feature == null)
        {
            return;
        }

        FeaturesList.Remove(feature);

        if (m_dicFeatures.ContainsKey(featureId))
        {
            m_dicFeatures.Add(featureId, feature);

            for (int i = 0; i < feature.BuffIDArray.Length; i++)
            {
                buffMgr.RemoveBuff(feature.BuffIDArray[i]);
            }
        }
    }

    public void Clear()
    {
        m_dicFeatures.Clear();
        buffMgr.ClearBuff();
        playerData.Clear();
        FeaturesList.Clear();
    }
}

public class PlayerBuffMgr
{
    #region 属性
    Dictionary<int, PlayerBuffConfig> m_buffDic = new Dictionary<int, PlayerBuffConfig>();
    List<PlayerBuffConfig> m_buffList = new List<PlayerBuffConfig>();

    public List<PlayerBuffConfig> buffList
    {
        get
        {
            return m_buffList;
        }
    }

    public Dictionary<int, PlayerBuffConfig> buffDic
    {
        get
        {
            return m_buffDic;
        }
    }

    public int BuffCount
    {
        get
        {
            return m_buffDic.Count;
        }
    }
    #endregion

    #region 接口
    public void ClearBuff()
    {
        buffDic.Clear();
        buffList.Clear();
    }

    public void RemoveBuff(PlayerBuffConfig buff)
    {
        if (buff == null)
        {
            return;
        }
        RemoveBuff(buff.BuffID);
    }

    public void RemoveBuff(int buffID)
    {
        if (m_buffDic.ContainsKey(buffID))
        {
            for (int i = 0; i < m_buffList.Count; i++)
            {
                if (m_buffList[i].BuffID == buffID)
                {
                    m_buffList.Remove(m_buffList[i]);
                    break;
                }
            }
            m_buffDic.Remove(buffID);
        }
    }

    public void AddFeature(PlayerFeatureConfig feature)
    {
        if (feature == null)
        {
            return;
        }

        var buffList = feature.BuffIDArray;

        for (int i = 0; i < buffList.Length; i++)
        {
            AddBuff(buffList[i]);
        }
    }

    public void AddBuff(int buffID)
    {
        if (!m_buffDic.ContainsKey(buffID))
        {
            bool hadBuff = false;
            for (int i = 0; i < m_buffList.Count; i++)
            {
                if (m_buffList[i].BuffID == buffID)
                {
                    hadBuff = true;
                    break;
                }
            }

            if (hadBuff)
            {
                return;
            }

            var buff = BuffConfigMgr.Instance.GetPlayerBuff(buffID);
            if (buff!=null)
            {
                m_buffDic.Add(buffID,buff);
                InitBuff(buff);
            }
            else
            {
                Debug.LogError("Buff is null buffID => "+ buffID);
            }
        }
    }

    public void InitBuff(PlayerBuffConfig buff)
    {
        var handleState = (GameMgr.GameState)buff.HandleState;

        if (handleState != GameMgr.GameState.NONE)
        {
            return;
        }

        switch ((PlayerBuffType)buff.BuffType)
        {
            case PlayerBuffType.BUFF_ADD_DEMAGE:
               
                break;
            case PlayerBuffType.BUFF_KUXINGSENG:
                RogueLikeMgr.Instance.playerData.NeedCheckCards = false;
                break;
        }
#if UNITY_EDITOR
        Debug.LogFormat("Player Init Buff:{0}", buff.BuffName);
#endif
        //EventCenter.Instance.EventTrigger("BossDataRefresh", bossActor);
    }
    #endregion

}