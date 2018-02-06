using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

    /// <summary>
    /// Clamps the X Velocity of the given Rigidbody2D to the value given if it is greater.
    /// </summary>
    /// <param name="rb">The Rigidbody2D that will have its velocity checked.</param>
    /// <param name="maxX">The maximum X speed (regardless of direction) for the object.</param>
    public static void ClampXVelocity(this Rigidbody2D rb, float maxX) {
        if (Mathf.Abs(rb.velocity.x) > maxX) {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxX, rb.velocity.y);
        }
    }

	/// <summary>
	/// Checks whether the Collision2d's gameobject is on the given layer
	/// </summary>
	/// <returns><c>true</c>, if the gameobject is on the given layer, <c>false</c> otherwise.</returns>
	/// <param name="col">The Collision2D to be checked.</param>
	/// <param name="layer">The string representing the layer name.</param>
	public static bool IsOnLayer(this Collision2D col, string layer) {
        return col.gameObject.IsOnLayer(layer);
    }

	/// <summary>
	/// Checks whether the gameobject is on the given layer
	/// </summary>
	/// <returns><c>true</c>, if the gameobject is on the given layer, <c>false</c> otherwise.</returns>
	/// <param name="go">The gameobject to be checked.</param>
	/// <param name="layer">The string representing the layer name.</param>
	public static bool IsOnLayer(this GameObject go, string layer)
	{
		return LayerMask.LayerToName(go.layer) == layer;
	}

    /// <summary>
    /// Returns a random element from the list.
    /// </summary>
    /// <returns>A random element from the list.</returns>
    /// <param name="list">The list.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T RandomElement<T>(this List<T> list)
	{
		return list[Random.Range(0, list.Count - 1)];
	}

    /// <summary>
    /// Determines if the float is between min and max.  (Exclusive)
    /// </summary>
    /// <returns><c>true</c>, if the value is between min and max, <c>false</c> otherwise.</returns>
    /// <param name="f">The value to be checked.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
	public static bool BetweenEx(this float f, float min, float max)
	{
		return f > min && f < max;
	}

	/// <summary>
	/// Determines if the float is between min and max.
	/// </summary>
	/// <returns><c>true</c>, if the value is between min and max, <c>false</c> otherwise.</returns>
	/// <param name="f">The value to be checked.</param>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	public static bool Between(this float f, float min, float max)
	{
		return f >= min && f <= max;
	}

    /// <summary>
    /// Converts the angle from radians to degrees and returns it.
    /// </summary>
    /// <returns>The angle in radians.</returns>
    /// <param name="f">The angle in degrees.</param>
    public static float ToDegrees(this float f) {
        return f * Mathf.Rad2Deg;
    }

	/// <summary>
	/// Converts the angle from degrees to radians and returns it.
	/// </summary>
	/// <returns>The angle in degrees.</returns>
	/// <param name="f">The angle in degrees.</param>
	public static float ToRadians(this float f) {
        return f * Mathf.Deg2Rad;
    }

    /// <summary>
    /// Rounds to nearest multiple of the given value.
    /// </summary>
    /// <returns>The given float rounded to the nearet multiple of val.</returns>
    /// <param name="f">The float to be rounded.</param>
    /// <param name="val">The value whose multiple f will be rounded to.</param>
    public static float RoundToNearest(this float f, float val) {
        return Mathf.Round(f / val) * val;
    }

    /// <summary>
    /// Calculates a Vector2 from the sin and cos of the angle.
    /// </summary>
    /// <returns>A unit vector pointing in the direction of the angle.</returns>
    /// <param name="ang">The angle in radians.</param>
    public static Vector2 FromRadianToVector(this float ang) {
        return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
    }

	/// <summary>
	/// Calculates a Vector2 from the sin and cos of the angle.
	/// </summary>
	/// <returns>A unit vector pointing in the direction of the angle.</returns>
	/// <param name="ang">The angle in degrees.</param>
	public static Vector2 FromDegreesToVector(this float deg) {
        return deg.ToRadians().FromRadianToVector();
    }

    /// <summary>
    /// Returns a new Vector3 with the given X value and YZ values that match the Vector3.
    /// </summary>
    /// <returns>The x.</returns>
    /// <param name="vec">Vec.</param>
    /// <param name="x">The x coordinate.</param>
    public static Vector3 SetX(this Vector3 vec, float x) {
        return new Vector3(x, vec.y, vec.z);
    }

    /// <summary>
    /// Sets the y.
    /// </summary>
    /// <returns>The y.</returns>
    /// <param name="vec">Vec.</param>
    /// <param name="y">The y coordinate.</param>
	public static Vector3 SetY(this Vector3 vec, float y)
	{
		return new Vector3(vec.x, y, vec.z);
	}

    /// <summary>
    /// Sets the z.
    /// </summary>
    /// <returns>The z.</returns>
    /// <param name="vec">Vec.</param>
    /// <param name="z">The z coordinate.</param>
	public static Vector3 SetZ(this Vector3 vec, float z)
	{
		return new Vector3(vec.x, vec.y, z);
	}

    /// <summary>
    /// Sets the x.
    /// </summary>
    /// <returns>The x.</returns>
    /// <param name="vec">Vec.</param>
    /// <param name="x">The x coordinate.</param>
	public static Vector2 SetX(this Vector2 vec, float x)
	{
        return new Vector2(x, vec.y);
	}

    /// <summary>
    /// Sets the y.
    /// </summary>
    /// <returns>The y.</returns>
    /// <param name="vec">Vec.</param>
    /// <param name="y">The y coordinate.</param>
	public static Vector2 SetY(this Vector2 vec, float y)
	{
		return new Vector2(vec.x, y);
	}

    /// <summary>
    /// Calculates the angle of the vector
    /// </summary>
    /// <returns>The angle calculated from the x and y of the vector.</returns>
    /// <param name="vec">The vector.</param>
    public static float Angle(this Vector2 vec) {
        return Mathf.Atan2(vec.y, vec.x);
    }

}
