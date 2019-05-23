/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;




public class WebsocketClientEcho : MonoBehaviour {

    public string url = "http://echo.websocket.org";
	
	IEnumerator Start () {
        var ws = new HTTP.WebSocket ();

        //This lets the websocket connection work asynchronously.
        StartCoroutine (ws.Dispatcher ());

        //Connect the websocket to the server.
        ws.Connect (url);

        //Wait for the connection to complete.
        yield return ws.Wait();

        //Always check for an exception here!
        if(ws.exception != null) {
            Debug.Log("An exception occured when connecting: " + ws.exception);
        }

        //Display connection status.
        Debug.Log("Connected? : " + ws.connected);

        //If websocket connected succesfully, we can send and receive messages.
        if(ws.connected) {
            //This specified that our OnStringReceived method will be called when a message is received.
            ws.OnTextMessageRecv += OnStringReceived;
            //This sends a message to the server.
            ws.Send ("Hello!");

            ws.Send ("Goodbye");
        }
	}

    void OnStringReceived(string msg) {
        Debug.Log ("From server -> " + msg);
    }
}
