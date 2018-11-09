using System;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	public static void RotateTowardsVector (this Transform trans_, Vector2 vel_, float rotSpeed_ = 4.0f, float angleOffset_ = 0.0f)
	{
		float angle = Mathf.Atan2 (vel_.y, vel_.x) * Mathf.Rad2Deg;
		trans_.rotation = Quaternion.Lerp (trans_.rotation, Quaternion.AngleAxis (angle + angleOffset_, Vector3.forward), Time.deltaTime * rotSpeed_);
	}

	public static float AngleBetweenTwoPoints (this Transform float_, Vector3 a, Vector3 b)
	{
		return Mathf.Atan2 (a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}

	public static Vector3 ToInt (this Vector3 vec_)
	{
		vec_.x.ToInt ();
		vec_.y.ToInt ();
		vec_.z.ToInt ();
		return vec_;
	}

	public static int ToInt (this float float_)
	{
		int i = Mathf.RoundToInt (float_);
		return i;
	}

	public static bool IsEven (this int int_)
	{
		if (int_ % 2 != 0) return false;
		return true;
	}

	public static T RandomFromList<T> (this List<T> list_)
	{
		return list_[UnityEngine.Random.Range (0, list_.Count)];
	}

	public static float Remap (this float value_, float min_, float max_, float newMin_, float newMax_)
	{
		return (value_ - min_) / (max_ - min_) * (newMax_ - newMin_) + newMin_;
	}

	public static Vector3 Remap (this Vector3 value_, float min_, float max_, float newMin_, float newMax_)
	{
		return value_ = new Vector3 (Remap (value_.x, min_, max_, newMin_, newMax_),
			Remap (value_.y, min_, max_, newMin_, newMax_),
			Remap (value_.z, min_, max_, newMin_, newMax_));
	}

	public static T Next<T> (this IList<T> list_, T item_)
	{
		var indexOf = list_.IndexOf (item_);
		return list_[indexOf == list_.Count - 1 ? 0 : indexOf + 1];
	}

	public static T Next<T> (this IList<T> list_, int index)
	{
		return list_[index == list_.Count - 1 ? 0 : index + 1];
	}

	public static T Previous<T> (this IList<T> list_, int index)
	{
		return list_[index == 0 ? list_.Count - 1 : index - 1];
	}
}