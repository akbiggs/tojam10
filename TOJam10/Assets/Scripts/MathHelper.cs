using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.AnimatedValues;
using UnityEngine;

public static class MathHelper {
	public static float PI_BY_4 = Mathf.PI / 4;
	public static float PI_BY_2 = Mathf.PI / 2;

	public static Vector3 SetX(this Vector3 v, float x) {
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 SetY(this Vector3 v, float y) {
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 SetZ(this Vector3 v, float z) {
		return new Vector3(v.x, v.y, z);
	}

	public static Vector3 AddX(this Vector3 v, float delta) {
		return v.SetX(v.x + delta);
	}

	public static Vector3 AddY(this Vector3 v, float delta) {
		return v.SetY(v.y + delta);
	}

	public static Vector3 AddZ(this Vector3 v, float delta) {
		return v.SetZ(v.z + delta);
	}
	
	public static Quaternion SetEulerX(this Quaternion q, float x) {
		return Quaternion.Euler(q.eulerAngles.SetY(x));
	}

	public static Quaternion SetEulerY(this Quaternion q, float y) {
		return Quaternion.Euler(q.eulerAngles.SetY(y));
	}
	
	public static Quaternion SetEulerZ(this Quaternion q, float z) {
		return Quaternion.Euler(q.eulerAngles.SetY(z));
	}

	public static bool GreaterEqualThan(this Vector3 v, Vector3 other) {
		return v.x >= other.x && v.y >= other.y && v.z >= other.z;
	}
	
	public static bool LessEqualThan(this Vector3 v, Vector3 other) {
		return v.x <= other.x && v.y <= other.y && v.z <= other.z;
	}
	
	public static Vector3 ComponentMultiply(this Vector3 v, Vector3 other) {
		return new Vector3(v.x * other.x, v.y * other.y, v.z * other.z);
	}

	public static Vector3 Round(this Vector3 v) {
		return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
	}

	public static float ThereAndBackAgainLerp(float transitionTime, float value) {
		if (value < 0) {
			return 0;
		} else if (value < transitionTime) {
			return value / transitionTime;
		} else if (value < 1 - transitionTime) {
			return 1;
		} else if (value < 1) {
			return (value - (1 - transitionTime)) / transitionTime;
		} else {
			return 0;
		}
	}

	public static Vector3 Clamp(this Vector3 v, float length) {
		float m = v.magnitude;
		if (m > length) {
			return (v / m) * length;
		}
		return v;
	}

	public static Vector3 Rotate(this Vector3 v, float degrees) {
		return v.Rotate(degrees, Vector3.up);
	}

	public static Vector3 Rotate(this Vector3 v, float degrees, Vector3 up) {
		return Quaternion.AngleAxis(degrees, up) * v;
	}

	public static Vector2 ChangeToVector2(this Vector3 v) {
		return new Vector2(v.x, v.z);
	}

	public static Vector3 ChangeToVector3(this Vector2 v) {
		return new Vector3(v.x, 0, v.y);
	}

	public static Vector2 SnapVectorAngle(this Vector2 v, float increment, float offset) {
		float angle = Mathf.Atan2(v.x, v.y);
		angle = Mathf.Round((angle - offset) / increment) * increment + offset;
		return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * v.magnitude;
	}

	public static Vector2 SnapVectorAngle(this Vector2 v, float increment) {
		float angle = Mathf.Atan2(v.x, v.y);
		angle = Mathf.Round(angle / increment) * increment;
		return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * v.magnitude;
	}

	public static Vector3 SnapVectorAlongPlane(this Vector3 v, float increment, float offset = 0) {
		return v.ChangeToVector2().SnapVectorAngle(increment, offset).ChangeToVector3();
	}

    public static Vector3 SmartSnapVector(this Vector3 v, float mainAxisSnapFavour = 0.2f)
    {
        float angle = Mathf.Atan2(v.x, v.z);
        float rounded = Mathf.Round(angle / PI_BY_2) * PI_BY_2;
        if (Mathf.Abs(angle - rounded) > Mathf.Deg2Rad * mainAxisSnapFavour)
        {
            return SnapVectorAlongPlane(v, PI_BY_4);
        }
        return new Vector3(Mathf.Sin(rounded), 0, Mathf.Cos(rounded)) * v.magnitude;
    }

	public static float Sqr(this float v) {
		return v * v;
	}

	public static Vector3 SmoothStep(Vector3 a, Vector3 b, float t) {
		return new Vector3(Mathf.SmoothStep(a.x, b.x, t), Mathf.SmoothStep(a.y, b.y, t), Mathf.SmoothStep(a.z, b.z, t));
	}

    public static bool ApproximatelyEquals(this float f, float f2)
    {
        return Math.Abs(f - f2) <= float.Epsilon;
    }
}
