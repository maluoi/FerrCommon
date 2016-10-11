using UnityEngine;
using System.Collections;

namespace Ferr {
	public abstract class Singleton<T> : MonoBehaviour where T:Component{
		private static T mInstance;
		public  static T Instance {
			get{
				if (mInstance == null) {
					mInstance = GameObject.FindObjectOfType<T>();
					if (mInstance == null) {
						GameObject go = new GameObject("_"+typeof(T).Name);
						mInstance = go.AddComponent<T>();
						go.hideFlags = HideFlags.DontSave;
					}
				}
				return mInstance;
			}
		}
		public static bool Instantiated {
			get { return mInstance != null; }
		}
	}
}