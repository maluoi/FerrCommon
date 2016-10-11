using UnityEngine;
using System.Collections;

/// <summary>
/// A direction used to describe the surface of terrain.
/// </summary>
public enum Ferr2DT_TerrainDirection
{
	Top    = 0,
	Left   = 1,
	Right  = 2,
	Bottom = 3,
	None   = 100
}

/// <summary>
/// Describes a terrain segment, and how it should be drawn.
/// </summary>
[System.Serializable]
public class Ferr2DT_SegmentDescription {
    /// <summary>
    /// Applies only to terrain segments facing this direction.
    /// </summary>
	public Ferr2DT_TerrainDirection applyTo;
    /// <summary>
    /// Z Offset, for counteracting depth issues.
    /// </summary>
	public float  zOffset;
    /// <summary>
    /// Just in case you want to adjust the height of the segment
    /// </summary>
	public float  yOffset;
    /// <summary>
    /// UV coordinates for the left ending cap.
    /// </summary>
	public Rect   leftCap;
	/// <summary>
    /// UV coordinates for the left ending cap.
    /// </summary>
	public Rect   innerLeftCap;
    /// <summary>
    /// UV coordinates for the right ending cap.
    /// </summary>
	public Rect   rightCap;
	/// <summary>
    /// UV coordinates for the right ending cap.
    /// </summary>
	public Rect   innerRightCap;
    /// <summary>
    /// A list of body UVs to randomly pick from.
    /// </summary>
	public Rect[] body;
    /// <summary>
    /// How much should the end of the path slide to make room for the caps? (Unity units)
    /// </summary>
	public float  capOffset = 0f;
	
	public Ferr2DT_SegmentDescription() {
		body    = new Rect[] { new Rect(0,0,50,50) };
		applyTo = Ferr2DT_TerrainDirection.Top;
	}
}