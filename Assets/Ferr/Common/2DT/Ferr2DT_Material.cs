using UnityEngine;
using System.Collections;

public class Ferr2DT_Material : ScriptableObject, IFerr2DTMaterial {
	#region Fields
	[SerializeField] Material                     _fillMaterial;
	[SerializeField] Material                     _edgeMaterial;
	[SerializeField] private Ferr2DT_SegmentDescription[] _descriptors = new Ferr2DT_SegmentDescription[4];
	[SerializeField] private bool isPixel = true;
	
	/// <summary>
    /// The material of the interior of the terrain.
    /// </summary>
	public Material fillMaterial { get{return _fillMaterial;} set{_fillMaterial = value;} }
	/// <summary>
    /// The material of the edges of the terrain.
    /// </summary>
	public Material edgeMaterial { get{return _edgeMaterial;} set{_edgeMaterial = value;} }
    #endregion
	
    #region Constructor
	public Ferr2DT_Material() {
		for (int i = 0; i < _descriptors.Length; i++) {
			_descriptors[i] = new Ferr2DT_SegmentDescription();
		}
	}
    #endregion
	
    #region Methods
    /// <summary>
    /// Gets the edge descriptor for the given edge, defaults to the Top, if none by that type exists, or an empty one, if none are defined at all.
    /// </summary>
    /// <param name="aDirection">Direction to get.</param>
    /// <returns>The given direction, or the first direction, or a default, based on what actually exists.</returns>
	public Ferr2DT_SegmentDescription GetDescriptor(Ferr2DT_TerrainDirection aDirection) {
		ConvertToPercentage();
		for (int i = 0; i < _descriptors.Length; i++) {
			if (_descriptors[i].applyTo == aDirection) return _descriptors[i];
		}
		if (_descriptors.Length > 0) {
			return _descriptors[0];
		}
		return new Ferr2DT_SegmentDescription();
	}
    /// <summary>
    /// Finds out if we actually have a descriptor for the given direction
    /// </summary>
    /// <param name="aDirection">Duh.</param>
    /// <returns>is it there, or is it not?</returns>
	public bool                       Has          (Ferr2DT_TerrainDirection aDirection) {
		for (int i = 0; i < _descriptors.Length; i++) {
			if (_descriptors[i].applyTo == aDirection) return true;
		}
		return false;
	}
    /// <summary>
    /// Sets a particular direction as having a valid descriptor. Or not. That's a bool.
    /// </summary>
    /// <param name="aDirection">The direction!</param>
    /// <param name="aActive">To active, or not to active? That is the question!</param>
	public void                       Set          (Ferr2DT_TerrainDirection aDirection, bool aActive) {
		if (aActive) {
			if (_descriptors[(int)aDirection].applyTo != aDirection) {
				_descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
				_descriptors[(int)aDirection].applyTo = aDirection;
			}
		} else if (_descriptors[(int)aDirection].applyTo != Ferr2DT_TerrainDirection.Top) {
			_descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
		}
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values Unity will recognize.
    /// </summary>
    /// <param name="aNativeRect">A UV rect, using pixels.</param>
    /// <returns>A UV rect using Unity coordinates.</returns>
	public Rect                       ToUV    (Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
		return new Rect(
			aNativeRect.x ,
			(1.0f - aNativeRect.height) - aNativeRect.y,
			aNativeRect.width,
			aNativeRect.height);
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values we can use on the screen! As 0-1.
    /// </summary>
    /// <param name="aNativeRect">A UV rect, using pixels.</param>
    /// <returns>A UV rect using standard UV coordinates.</returns>
	public Rect                       ToScreen(Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
		return aNativeRect;
	}
	
	public Rect GetBody     (Ferr2DT_TerrainDirection aDirection, int aBodyID) {
		return GetDescriptor(aDirection).body[aBodyID];
	}
	
	private void ConvertToPercentage() {
		if (isPixel) {
			for (int i = 0; i < _descriptors.Length; i++) {
				for (int t = 0; t < _descriptors[i].body.Length; t++) {
					_descriptors[i].body[t] = ToNative(_descriptors[i].body[t]);
				}
				_descriptors[i].leftCap  = ToNative(_descriptors[i].leftCap );
				_descriptors[i].rightCap = ToNative(_descriptors[i].rightCap);
			}
			isPixel = false;
		}
	}
	public Rect ToNative(Rect aPixelRect) {
		if (edgeMaterial == null) return aPixelRect;
		
		int w = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.width;
		int h = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.height;
		
		return new Rect(
			aPixelRect.x      / w,
			aPixelRect.y      / h,
			aPixelRect.width  / w,
			aPixelRect.height / h);
	}
	public Rect ToPixels(Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
		
		int w = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.width;
		int h = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.height;
		
		return new Rect(
			aNativeRect.x      * w,
			aNativeRect.y      * h,
			aNativeRect.width  * w,
			aNativeRect.height * h);
	}
    #endregion
	
	#if UNITY_EDITOR
	const string editorMenuName = "Terrain Material";
	[UnityEditor.MenuItem("GameObject/Create Ferr2D Terrain/" + editorMenuName, false, 11 ), 
	 UnityEditor.MenuItem("Assets/Create/Ferr2D Terrain/"     + editorMenuName, false, 101)]
	public static void CreateAsset() {
		Ferr.SOUtil.CreateAsset(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, editorMenuName);
	}
	#endif
}
