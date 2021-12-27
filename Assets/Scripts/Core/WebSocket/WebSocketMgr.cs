using System;
using BestHTTP.WebSocket;
using UnityEngine;
using UnityEngine.UI;

public class WebSocketMgr:UnitySingleton<WebSocketMgr>
{
    string address = "http://127.0.0.1:8080/ws";
    WebSocket webSocket;
    public Text Tips;
         
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (webSocket == null)
        {
            webSocket = new WebSocket(new Uri(address));

#if !UNITY_WEBGL
            webSocket.StartPingThread = true;
#endif

            // Subscribe to the WS events
            webSocket.OnOpen += OnOpen;
            webSocket.OnMessage += OnMessageRecv;
            webSocket.OnBinary += OnBinaryRecv;
            webSocket.OnClosed += OnClosed;
            webSocket.OnError += OnError;

            // Start connecting to the server
            webSocket.Open();
        }
    }

    public void Destroy()
    {
        if (webSocket != null)
        {
            webSocket.Close();
            webSocket = null;
        }
    }

    void OnOpen(WebSocket ws)
    {
        Debug.Log("OnOpen: ");
        Tips.text += "/n" + "OnOpen:";
        webSocket.Send("你好啊");
    }

    void OnMessageRecv(WebSocket ws, string message)
    {
        Debug.LogFormat("OnMessageRecv: msg={0}", message);
        Tips.text += "/n" + "OnMessageRecv: msg="+ message;
    }

    void OnBinaryRecv(WebSocket ws, byte[] data)
    {
        Debug.LogFormat("OnBinaryRecv: len={0}", data.Length);
    }

    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        Debug.LogFormat("OnClosed: code={0}, msg={1}", code, message);
        webSocket = null;
    }

    void OnError(WebSocket ws, string reason)
    {
        string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws.InternalRequest.Response != null)
        {
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
        }
#endif
        Debug.LogFormat("OnError: error occured: {0}\n", (reason != null ? reason : "Unknown Error " + errorMsg));
        webSocket = null;
    }

}
