using UnityEngine;
using BestHTTP.WebSocket;
using System;
using RegicideProtocol;

public class WebSocketMgr : UnitySingleton<WebSocketMgr>
{
    //string address = "wss://echo.websocket.org";
    public string address = "ws://127.0.0.1:12345/ws";
    WebSocket webSocket;

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
        //RoomDataMgr.Instance.StartGameReq();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

            MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.StartGame);
            RoomPack roomPack = new RoomPack();
            roomPack.RoomID = 123;
            mainPack.Roompack.Add(roomPack);
            Send(mainPack);
        }
    }

    void OnMessageRecv(WebSocket ws, string message)
    {
        Debug.LogFormat("OnMessageRecv: msg={0}", message);
    }

    void OnBinaryRecv(WebSocket ws, byte[] data)
    {
        Debug.LogFormat("OnBinaryRecv: len={0}", data.Length);
        RecivePack(ws, data);
    }

    private void RecivePack(WebSocket webSocket, byte[] bufBytes)
    {
        var length = bufBytes.Length;

        int count = length - 4;

        int bufferAllCount = count + 4;    //整条消息的长度

        MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(bufBytes, 4, count);

        //if (handleResponse != null)
        //{
        //    handleResponse(pack);
        //}
        Debug.Log(pack);

        Array.Copy(bufBytes, bufferAllCount, bufBytes, 0, 4 - bufferAllCount);
    }

    public void Send(MainPack pack)
    {
        if (webSocket == null || !webSocket.IsOpen)
        {
            Debug.LogError("Socket Connect => false");
            return;
        }
        webSocket.Send(Message.PackData(pack));
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
        Debug.LogFormat("OnError: error occured: {0}\n", ("Unknown Error " + errorMsg));
        Debug.Log(reason);
        webSocket = null;
    }

}
