using UnityEngine;
public static class Utils {
	public static float RoundNumberToNearMultiple(float number, float fixedBase) {
		if (fixedBase != 0 && number != 0) {
			float sign = number > 0 ? 1 : -1;
			number *= sign;
			number /= fixedBase;
			int fixedPoint = Mathf.RoundToInt(number);
			number = fixedPoint * fixedBase;
			number *= sign;
		}
		return number;
	}
}