/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace HTTP {
	public class UniWeb : MonoBehaviour {
		
        static UniWeb _instance = null;
		
        static public UniWeb Instance {
			
            get {
				if(_instance == null) {
					_instance = new GameObject("UniWeb", typeof(UniWeb)).GetComponent<UniWeb>();
					_instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				return _instance;
			}
		}

		void Awake()
		{
			if (_instance != null)
			{
				Destroy(gameObject);
			}
		}
		
		public void Send(Request request, System.Action<HTTP.Request> requestDelegate) {
			StartCoroutine(_Send(request, requestDelegate));
		}
		
		public void Send(Request request, System.Action<HTTP.Response> responseDelegate) {
			StartCoroutine(_Send(request, responseDelegate));
		}
		
		IEnumerator _Send(Request request, System.Action<HTTP.Response> responseDelegate) {
			request.Send();
			while(!request.isDone)
				yield return new WaitForEndOfFrame();
			if(request.exception != null) {
				Debug.LogError(request.exception);	
			} else {
				responseDelegate(request.response);
			}
		}
		
		IEnumerator _Send(Request request, System.Action<HTTP.Request> requestDelegate) {
			request.Send();
			while(!request.isDone)
				yield return new WaitForEndOfFrame();
			requestDelegate(request);
		}
		
		List<System.Action> onQuit = new List<System.Action>();
		public void OnQuit(System.Action fn) {
			onQuit.Add(fn);	
		}

		void OnApplicationQuit() {

			foreach(var fn in onQuit) {
				try {
					fn();
				} catch(System.Exception e) {
					Debug.LogError(e);	
				}
			}
			
			
		}
	}
}
