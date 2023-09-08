using System;
using UnityEngine;

namespace TeamMingo.MTween {

  using InterpolationFunc = Func<float[], float, float>;
	
	public static class Interpolation {

		public static float Linear(float[] v, float k) {
			int m = v.Length - 1;
			float f = m * k;
			int i = Mathf.FloorToInt(f);
			if (k < 0) {
				return InterpolationUtils.Linear(v[0], v[1], f);
			}
			if (k > 1) {
				return InterpolationUtils.Linear(v[m], v[m - 1], m - f);
			}
			return InterpolationUtils.Linear(v[i], v[i + 1 > m ? m : i + 1], f - i);
		}

		public static float Bezier(float[] v, float k) {
			float b = 0;
			int n = v.Length - 1;
			for (int i = 0; i <= n; i++) {
				b += Mathf.Pow(1 - k, n - i) * Mathf.Pow(k, i) * v[i] * InterpolationUtils.Bernstein(n, i);
			}
			return b;
		}

		public static float CatmullRom(float[] v, float k) {
			int m = v.Length - 1;
			float f = m * k;
			int i = Mathf.FloorToInt(f);
			if (v[0] == v[m]) {
				if (k < 0) {
					i = Mathf.FloorToInt(f = m * (1 + k));
				}
				return InterpolationUtils.CatmullRom(v[(i - 1 + m) % m], v[i], v[(i + 1) % m], v[(i + 2) % m], f - i);
			} else {
				if (k < 0) {
					return v[0] - (InterpolationUtils.CatmullRom(v[0], v[0], v[1], v[1], -f) - v[0]);
				}
				if (k > 1) {
					return v[m] - (InterpolationUtils.CatmullRom(v[m], v[m], v[m - 1], v[m - 1], f - m) - v[m]);
				}
				return InterpolationUtils.CatmullRom(v[i > 0 ? i - 1 : 0], v[i], v[m < i + 1 ? m : i + 1], v[m < i + 2 ? m : i + 2], f - i);
			}
		}

		private static class InterpolationUtils {

			private static int[] a = new int[] { 1 };

			public static float Linear(float p0, float p1, float t) {
				return (p1 - p0) * t + p0;
			}

			public static float Bernstein(int n, int i) {
				return InterpolationUtils.Factorial(n) / InterpolationUtils.Factorial(i) / InterpolationUtils.Factorial(n - i);
			}

			public static float Factorial (int n) {
				var s = 1;
				if (a[n] == 1) {
					return a[n];
				}
				for (var i = n; i > 1; i--) {
					s *= i;
				}
				a[n] = s;
				return s;
			}

			public static float CatmullRom(float p0, float p1, float p2, float p3, float t) {
				var v0 = (p2 - p0) * 0.5f;
				var v1 = (p3 - p1) * 0.5f;
				var t2 = t * t;
				var t3 = t * t2;
				return (2 * p1 - 2 * p2 + v0 + v1) * t3 + (- 3 * p1 + 3 * p2 - 2 * v0 - v1) * t2 + v0 * t + p1;
			}

		}
    
	}

}