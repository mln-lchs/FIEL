/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace HTTP
{
	public class RC4
	{

		const int STIR_INCREMENT_CONST = 1600000;
		static readonly RC4 instance = new RC4 ();
	
		class Arc4Stream
		{
			public byte i;
			public byte j;
			public byte[] s = new byte[256];
		
			public Arc4Stream ()
			{
				for (int n = 0; n <= byte.MaxValue; n++) {
					s [n] = (byte)n;
				}
				i = 0;
				j = 0;
			}
		}

		Arc4Stream stream = new Arc4Stream ();
		int count;
	
		public int RandomNumber ()
		{
			count -= 4;
			StirIfNeeded ();
			return GetWord ();
		}
	
		public void RandomValues (List<byte> result, int offset, int length)
		{
			StirIfNeeded ();
			while (length-- != 0) {
				count--;
				StirIfNeeded ();
				result [offset + length] = GetByte ();
			}
		}

		void AddRandomData (byte[] data)
		{
			stream.i--;
			for (int n = 0; n < 256; n++) {
				stream.i++;
				byte si = stream.s [stream.i];
				stream.j += (byte)(si + data [n % data.Length]);
				stream.s [stream.i] = stream.s [stream.j];
				stream.s [stream.j] = si;
			}
			stream.j = stream.i;
		}
	
		void Stir ()
		{
			const int length = 128;
			byte[] randomness = new byte[length];
			CryptographicallyRandomValuesFromOS (randomness);
			AddRandomData (randomness);

			// Discard early keystream, as per recommendations in:
			// http://www.wisdom.weizmann.ac.il/~itsik/RC4/Papers/Rc4_ksa.ps
			for (int i = 0; i < 256; i++)
				GetByte ();
			count = STIR_INCREMENT_CONST;
		}
	
		void StirIfNeeded ()
		{
			if (count <= 0) {
				Stir ();
			}
		}
	
		byte GetByte ()
		{
			stream.i++;
			byte si = stream.s [stream.i];
			stream.j += si;
			byte sj = stream.s [stream.j];
			stream.s [stream.i] = sj;
			stream.s [stream.j] = si;
			return (stream.s [(si + sj) & 0xff]);	
		}

		int GetWord ()
		{
			return (GetByte () << 24 | GetByte () << 16 | GetByte () << 8 | GetByte ());
		}
		
		static public int CryptographicallyRandomNumber ()
		{	
			return instance.RandomNumber ();
		}

		static public void CryptographicallyRandomValues (List<byte> buffer, int offset, int length)
		{
			instance.RandomValues (buffer, offset, length);
		}
	
		static void CryptographicallyRandomValuesFromOS (byte[] buffer)
		{
			RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider ();
			cryptoServiceProvider.GetBytes (buffer);
		}
	
	}
}
