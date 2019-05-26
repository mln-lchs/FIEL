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
using System.Diagnostics;

public class RequestBenchmark : MonoBehaviour
{

    
    IEnumerator Start ()
    {
        var uniwebSeconds = 0.0;
        var wwwSeconds = 0.0;

        //Warm up engines.
        yield return new HTTP.Request ("GET", "http://www.differentmethods.com/").Send();
        yield return new WWW ("http://www.differentmethods.com/");

        for (var i=0; i<10; i++) {
            var clock = new Stopwatch ();
            var req = new HTTP.Request ("GET", "http://www.differentmethods.com/");
            clock.Start ();
            yield return req.Send ();
            clock.Stop ();
            uniwebSeconds += clock.Elapsed.TotalSeconds;
        }
        for (var i=0; i<10; i++) {
            var clock = new Stopwatch ();
            var req = new WWW ("http://www.differentmethods.com/");
            clock.Start ();
            yield return req;
            clock.Stop ();
            wwwSeconds += clock.Elapsed.TotalSeconds;
        }

        UnityEngine.Debug.Log("UniWeb: " + (uniwebSeconds/10));
        UnityEngine.Debug.Log("WWW: " + (wwwSeconds/10));
    }
    
}
