﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

class FeatureMgr : Singleton<FeatureMgr>
{

    public void GenBossFeature(BossActor bossActor,int featureCount = 1)
    {
        int totalFeaCount = FeatureConfigMgr.Instance.FeatureCount;
        int randCount = 0;
        bool complete = false;

        if (featureCount > totalFeaCount)
        {
            return;
        }

        var temp = new List<FeatureConfig>();
        while (!complete)
        {
            //Random ran = new Random((int)DateTime.Now.Ticks + randCount);

            if (temp.Count >= featureCount)
            {
                break;
            }

            //var idx = ran.Next(0, totalFeaCount);

            var feature = FeatureConfigMgr.Instance.RandFeatureConfig(); //FeatureConfigMgr.Instance.FeatureConfigList[idx];

            bool hadCard = false;

            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].ID == feature.ID)
                {
                    hadCard = true;
                    break;
                }

            }

            if (feature!= null && !hadCard)
            {
                temp.Add(feature);
#if UNITY_EDITOR
                Debug.LogError("feature add success:featureID =>" + feature.ID + feature.Name);
#endif
            }

            randCount++;

            if (temp.Count >= featureCount)
            {
                complete = true;
            }
        }

        bossActor.SetFeature(temp);
    }
}