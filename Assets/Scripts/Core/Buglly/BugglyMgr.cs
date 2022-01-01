﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugglyMgr : Singleton<BugglyMgr>
{
    public BugglyMgr()
    {
        Application.logMessageReceived += LogCallback;
        MonoManager.Instance.AddUpdateListener(Update);
    }

    void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            //Push(string.Format("报错啦,请截图发QQ群761857971{0}\n{1}", condition, stackTrace));
        }
    }

    public new void OnInit()
    {
        Debug.Log("Use Buggly!!!");
    }

    private Stack<string> m_errorMsgs = new Stack<string>();
    private int m_count = 0;
    public void Push(string errorMsg)
    {
        m_errorMsgs.Push(errorMsg);
        m_count++;
    }

    public void Pop()
    {
        if (m_errorMsgs.Count <= 0)
        {
            return;
        }

        var msg = m_errorMsgs.Pop();
        m_count--;
        Debug.LogError(msg);
        var ui = UISys.Mgr.ShowWindow<ERRORUI>(UI_Layer.System);
        ui.Init(msg);
    }

    private void Update()
    {
        if (m_count>0)
        {
            Pop();
        }
    }
}