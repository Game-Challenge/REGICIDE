using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class JsonHelper : Singleton<JsonHelper>
{
    private JsonImp jsonImp;

    public JsonHelper()
    {
        jsonImp = new JsonImp();
    }

    public DJsonData Deserialize(string json)
    {
        return jsonImp.Deserialize(json);
    }

    public string Serialize(DJsonData jsonData)
    {
        return jsonImp.Serialize(jsonData);
    }

    public T Deserialize<T>(string json)
    {
        return JsonMapper.ToObject<T>(json);
    }

    public string Serialize(Object jsonData)
    {
        return JsonMapper.ToJson(jsonData);
    }
}