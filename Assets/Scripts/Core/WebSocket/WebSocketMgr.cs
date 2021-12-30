using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;
using System;
using BestHTTP.Examples;
using UnityEngine.UI;
using System.Text;
using RegicideProtocol;

public class WebSocketMgr : UnitySingleton<WebSocketMgr>
{
    public string url = "ws://localhost:12345/ping";
    public InputField msg;
    public Text console;

    private WebSocket webSocket;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        webSocket = new WebSocket(new Uri(url));
        webSocket.OnOpen += OnOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnBinary += RecivePack;
        webSocket.OnError += OnError;
        webSocket.OnClosed += OnClosed;
        Connect();
    }

    private void DeInit()
    {
        webSocket.OnOpen = null;
        webSocket.OnMessage = null;
        webSocket.OnError = null;
        webSocket.OnClosed = null;
        webSocket = null;
    }

    private void RecivePack(WebSocket webSocket, byte[] bufBytes)
    {
        var length = bufBytes.Length;

        int count = length - 4;

        int bufferAllCount = count + 4;    //整条消息的长度

        while (true)
        {
            if (4 >= (count + 4))
            {
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(bufBytes, 4, count);

                //if (handleResponse != null)
                //{
                //    handleResponse(pack);
                //}
                Debug.Log(pack);

                Array.Copy(bufBytes, bufferAllCount, bufBytes, 0, 4 - bufferAllCount);
            }
            else
            {
                break;
            }
        }
    }

    public void Send(MainPack pack)
    {
        if (webSocket == null || webSocket.IsOpen)
        {
            Debug.LogError("Socket Connect => false");
            return;
        }
        webSocket.Send(Message.PackData(pack));
    }

    private void setConsoleMsg(string msg)
    {
        console.text = "Message: " + msg;
    }

    public void Connect()
    {
        webSocket.Open();
    }

    private byte[] getBytes(string message)
    {

        byte[] buffer = Encoding.Default.GetBytes(message);
        return buffer;
    }

    public void Send()
    {
        webSocket.Send(msg.text);
    }

    public void Send(string str)
    {
        webSocket.Send(str);
    }

    public void Close()
    {
        webSocket.Close();
    }

    #region WebSocket Event Handlers

    /// <summary>
    /// Called when the web socket is open, and we are ready to send and receive data
    /// </summary>
    void OnOpen(WebSocket ws)
    {
        Debug.Log("connected");
        setConsoleMsg("Connected");
    }

    /// <summary>
    /// Called when we received a text message from the server
    /// </summary>
    void OnMessageReceived(WebSocket ws, string message)
    {
        Debug.Log(message);
        setConsoleMsg(message);
    }

    /// <summary>
    /// Called when the web socket closed
    /// </summary>
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        Debug.Log(message);
        setConsoleMsg(message);
        DeInit();
        Init();
    }

    private void OnDestroy()
    {
        if (webSocket != null && webSocket.IsOpen)
        {
            webSocket.Close();
            DeInit();
        }
    }

    /// <summary>
    /// Called when an error occured on client side
    /// </summary>
    void OnError(WebSocket ws,string reason)
    {
        string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
#endif
        Debug.Log(errorMsg);
        Debug.Log(reason);
        setConsoleMsg(errorMsg);
        DeInit();
        Init();
    }

    #endregion
}