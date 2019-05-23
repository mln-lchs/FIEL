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

public class KeepAliveExample : MonoBehaviour
{

    void Start ()
    {
        StartCoroutine (TestKeepAlive ());

    }

    void OnGUI ()
    {
        if (GUILayout.Button ("Restart")) {
            Start ();
        }

    }

    IEnumerator TestKeepAlive ()
    {
        for (var i = 0; i < 3; i++) {

            var r = new HTTP.Request ("GET", "http://google.com/");
            yield return r.Send ();
            if (r.exception == null) {
                Debug.Log (r.response.status);
            } else {
                Debug.LogError (r.exception);
            }
            yield return new WaitForSeconds(1);

        }

    }

}
