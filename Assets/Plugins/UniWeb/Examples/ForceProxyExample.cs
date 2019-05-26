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

public class ForceProxyExample : MonoBehaviour
{

    void Start ()
    {
        //This sets the proxy for ALL http requests. This address is the default address used by Charles.
        HTTP.Request.proxy = new System.Uri("http://127.0.0.1:8888");

        StartCoroutine (FetchAssetBundle());
    }


    void OnGUI ()
    {
        if (GUILayout.Button ("Restart")) {
            Start ();
        }

    }

    IEnumerator FetchAssetBundle() {
        var r = new HTTP.Request("GET", "http://differentmethods.com/~simon/uniwebtest.unity3d?xyzzy");
        yield return r.Send();
        if(r.exception == null) {
            Debug.Log(r.response.status);
            var abcr = r.response.AssetBundleCreateRequest();
            yield return abcr;
            Debug.Log(abcr.assetBundle);
        } else {
            Debug.LogError(r.exception);
        }
    }

}
