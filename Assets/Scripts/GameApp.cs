﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

sealed partial class GameApp : UnitySingleton<GameApp>
{
    public int TargetFrameRate = 300;

    public bool UseWebSocket;

    public bool UseBuggly;

    public override void Awake()
    {
        base.Awake();

        Init();
    }

    private void Init()
    {
#if !UNITY_WEBGL
        if (UseBuggly)
        {
            BugglyMgr.Instance.OnInit();
        }
#endif

        SetTargetFrameRate();
        InitLibImp();
        RegistAllSystem();

        GameMgr.Instance.Init();
        MonoManager.Instance.StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        var config = FeatureConfigMgr.Instance.GetFeatureBaseConfig();

        yield return new WaitForSeconds(0.3f);
        if (GameMgr.Instance.IsLandScape)
        {
            Debug.Log("是横屏");
            UISys.Mgr.ShowWindow<StartUI>();
            UISys.Mgr.ShowWindow<MsgUI>(UI_Layer.Top);
        }
        else
        { 
            var canvasScale = UnityUtil.GetComponent<CanvasScaler>(UISys.Mgr.canvas.gameObject);
            canvasScale.referenceResolution = new Vector2(1170,2532);

            Debug.Log("是竖屏");

            var ui = UISys.Mgr.ShowWindow<MsgUI>(UI_Layer.Top);
            ui.gameObject.transform.localScale = new Vector3(2, 2, 1);
            UISys.Mgr.ShowWindow<StartUILand>();
        }
    }
}