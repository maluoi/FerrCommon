using UnityEditor;
using UnityEngine;

namespace Ferr {
	public class UnlitVertexColorEditor : ShaderGUI {
		public override void OnGUI (MaterialEditor aMaterialEditor, MaterialProperty[] aProperties) {
			base.OnGUI (aMaterialEditor, aProperties);
			
			Material targetMat = aMaterialEditor.target as Material;
			string[] keyWords  = targetMat.shaderKeywords;
			
			bool noTex = System.Array.IndexOf(keyWords, "NO_TEX") != -1;
			EditorGUI.BeginChangeCheck();
			noTex = EditorGUILayout.Toggle ("Don't use texture", noTex);
			if (EditorGUI.EndChangeCheck()) {
				string[] keywords = new string[] { noTex ? "NO_TEX" : "USE_TEX" };
				targetMat.shaderKeywords = keywords;
				EditorUtility.SetDirty (targetMat);
			}
		}
	}
}