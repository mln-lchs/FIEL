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

public class ConditionalGet : MonoBehaviour {

	// Use this for initialization
    IEnumerator Start () {
        var uri = "http://www.differentmethods.com/packages/SpatialAudioSourceDemo.html";

        var req = new HTTP.Request("GET", uri);
        req.useCache = true;
        req.headers.Set("If-None-Match", "xyzzy");
        yield return req.Send();
        Debug.Log(req.response.headers);

        Debug.Log ("Using Etag: " + req.response.headers.Get("ETag"));
        var newReq = new HTTP.Request("GET", uri);
        newReq.useCache = true;
        newReq.headers.Set("If-None-Match", req.response.headers.Get("ETag"));
        yield return newReq.Send();
        Debug.Log(newReq.response.headers);

	
	}
	
	
}
