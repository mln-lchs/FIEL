/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;


namespace HTTP
{
	
	public class URL
	{
		static string safeChars = "-_.~abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		public static string Encode (string value)
		{
			var result = new StringBuilder ();
			foreach (var s in value) {
				if (safeChars.IndexOf (s) != -1) {
					result.Append (s);
				} else {
					result.Append ('%' + String.Format ("{0:X2}", (int)s));
				}
			}
			return result.ToString ();
		}
		
		public static string Decode(string s) {
			return WWW.UnEscapeURL(s);
		}
		
		public static Dictionary<string,string> KeyValue(string queryString) {
			
			var kv = new Dictionary<string,string>();
			if(queryString.Length == 0) return kv;
			var pairs = queryString.Split('&');
			foreach(var i in pairs) {
				var t = i.Split('=');
				if(t.Length < 2) continue;
				kv[Decode(t[0])] = Decode(t[1]);
			}
			return kv;
		}
		
	}
}


