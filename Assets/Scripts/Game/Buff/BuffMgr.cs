using System.Collections.Generic;
using UnityEngine;


public class BuffMgr
{
    Dictionary<int,BuffConfig> m_buffDic = new Dictionary<int, BuffConfig>();
    List<BuffConfig> m_buffList = new List<BuffConfig>();

    public Dictionary<int, BuffConfig> buffDic
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

    public void ClearBuff()
    {
        m_buffDic.Clear();
    }

    public void HandleBuff(BossActor bossActor,BuffConfig buff)
    {
        if (bossActor == null||buff == null)
        {
            return;
        }

        switch ((BuffType)buff.BuffType)
        {
            case BuffType.BUFF_ADD_DEMAGE:
                bossActor.Atk += (int)(bossActor.Atk * buff.BuffValue);
                break;
            case BuffType.BUFF_ADD_HP:
                bossActor.Hp += (int)(bossActor.Hp * buff.BuffValue);
                break;
        }
        EventCenter.Instance.EventTrigger("BossDataRefresh",bossActor);
    }

    public void CheckBuffValue(BossActor bossActor)
    {
        if (bossActor == null)
        {
            return;
        }

        var buffMgr = bossActor.ActorBuffMgr;

        if (buffMgr == null)
        {
            return;
        }

        foreach (var buff in m_buffList)
        {
            HandleBuff(bossActor, buff);
        }
    }

    #region 设置BUFF

    public void SetBuffByFeatures(List<FeatureConfig> featureConfigs, BossActor bossActor = null)
    {
        if (featureConfigs == null)
        {
            return;
        }

        for (int i = 0; i < featureConfigs.Count; i++)
        {
            var feature = featureConfigs[i];
            SetBuffByFeature(feature,bossActor);
        }
    }

    public void SetBuffByFeature(FeatureConfig featureConfig, BossActor bossActor = null)
    {
        if (featureConfig == null)
        {
            return;
        }

        var buffs = featureConfig.BuffIDArray;

        for (int i = 0; i < buffs.Length; i++)
        {
            var buffID = buffs[i];
            if (buffID == 0)
            {
                continue;
            }
            if (!m_buffDic.ContainsKey(buffs[i]))
            {
                var buff = BuffConfigMgr.Instance.GetCfg(buffID);
                if (buff == null)
                {
                    Debug.LogError("buff error: had no buff buffID =>" + buffID);
                    continue;
                }
#if UNITY_EDITOR
                Debug.LogError("buff add success:buffID =>" + buffID + buff.BuffName);
#endif
                m_buffDic.Add(buffID, buff);
                m_buffList.Add(buff);
                CheckBuffValue(bossActor);
            }
        }
    }


    #endregion
}