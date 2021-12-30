using RegicideProtocol;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using UnityEngine;

public static class Utils
{
    public static void Show(this GameObject gameObject,bool show)
    {
        if (gameObject!= null)
        {
            gameObject.transform.localScale = show ? Vector3.one : Vector3.zero;
        }
    }

    public static bool CheckHaveError(MainPack mainPack)
    {
        bool hasError = false;
        if (mainPack == null)
        {
            var tip = string.Format("网络数据错误{0}", mainPack.Actioncode);
            //UISys.Mgr.ShowTipMsg(tip);
            Debug.LogFormat("package {0} null!", mainPack.Actioncode);
            hasError = true;
            return hasError;
        }

        if (mainPack.Returncode == ReturnCode.Fail)
        {
            hasError = true;
        }

        return hasError;
    }

    /// <summary>
    /// 存储字典的值到列表里面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="W"></typeparam>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static List<W> SaveDicValueToList<T, W>(Dictionary<T, W> dic)
    {
        List<W> listW = new List<W>();
        var ienuDic = dic.GetEnumerator();

        while (ienuDic.MoveNext())
        {
            listW.Add(ienuDic.Current.Value);
        }

        return listW;
    }

    public static List<W> ToList<T, W>(this Dictionary<T, W> dic)
    {
        List<W> listW = new List<W>();

        var ienuDic = dic.GetEnumerator();

        while (ienuDic.MoveNext())
        {
            listW.Add(ienuDic.Current.Value);
        }

        return listW;
    }

    public static List<T> ToList<T>(this RepeatedField<T> repeatedField)
    {
        List<T> list = new List<T>();

        foreach (var value in repeatedField)
        {
            list.Add(value);
        }

        return list;
    }
}